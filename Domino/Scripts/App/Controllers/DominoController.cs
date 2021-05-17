using Dominos.AppUtil;
using Dominos.AppUtil.Enums;
using Dominos.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Dominos.App
{
    public class DominoController : MonoBehaviour
    {
        [SerializeField]
        private Domino _dominoPrefab;

        [SerializeField]
        private StartDomino _start;

        [SerializeField]
        private GoalDomino _goal1;

        [SerializeField]
        private GoalDomino _goal2;

        [SerializeField]
        private Stick _stick;

        [SerializeField]
        private float _ngArea = 0.1f;

        private Dictionary<Collider, Domino> _dominosDic = new Dictionary<Collider, Domino>();
        private List<GoalDomino> _goalDominos;

        private DominoStatus _dominoStatus = DominoStatus.None;
        private readonly ReactiveProperty<Domino> _targetDomino = new ReactiveProperty<Domino>();

        private float _playTime = 0;
        private Vector3 _prevPos = Vector3.zero;

        private Subject<float> _goalTimeSubject = new Subject<float>();
        public IObservable<float> GoalTimeObservable => _goalTimeSubject;

        private void Start()
        {
            _goalDominos = new List<GoalDomino> { _goal1, _goal2 };

            // ドミノ回転or新規作成
            this.UpdateAsObservable()
            .Where(_ => Input.GetMouseButtonDown(0))
            .Subscribe(_ =>
            {
                if(TapController.GetTapClosestObject(out var hit))
                {
                    if(hit.collider.tag == Defines.DominoTag)
                    {
                        // 回転or複製開始
                        if(GetTapDomino(hit.collider, out var targetDomino))
                        {
                            if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                            {
                                DuplicateDomino(targetDomino.transform);
                            }
                            else
                            {
                                _targetDomino.Value = targetDomino;
                                _dominoStatus = DominoStatus.Rotate;
                            }
                        }
                    }
                    else if(hit.collider.tag == Defines.StageTag)
                    {
                        // 新規作成
                        CreateDomino(hit.point);
                    }
                }
            })
            .AddTo(this);

            this.UpdateAsObservable()
            .Where(_ => Input.GetMouseButtonDown(1))
            .Subscribe(_ =>
            {
                if(!TapController.GetTapClosestObject(out var hit))
                {
                    return;
                }

                if(hit.collider.tag != Defines.DominoTag)
                {
                    return;
                }

                if(!GetStageTapPosition(out _prevPos))
                {
                    return;
                }

                // 移動開始
                if(GetTapDomino(hit.collider, out var targetDomino))
                {
                    _targetDomino.Value = targetDomino;
                    _dominoStatus = DominoStatus.Move;
                }
            })
            .AddTo(this);

            // 操作完了
            this.UpdateAsObservable()
            .Where(_ => Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
            .Subscribe(_ =>
            {
                _dominoStatus = DominoStatus.None;
                _targetDomino.Value = null;
            })
            .AddTo(this);

            // 回転
            this.UpdateAsObservable()
            .Where(_ => Input.GetMouseButton(0) && _dominoStatus == DominoStatus.Rotate)
            .Subscribe(_ => DominoRotate())
            .AddTo(this);

            // 移動
            this.UpdateAsObservable()
            .Where(_ => Input.GetMouseButton(1) && _dominoStatus == DominoStatus.Move)
            .Subscribe(_ => DominoMove())
            .AddTo(this);

            // 操作対象ドミノの変更
            _targetDomino.Subscribe(_ => TargetDominoChanged())
            .AddTo(this);
        }

        public void Reset()
        {
            foreach(var domino in _dominosDic.Values)
            {
                domino.Reset();
            }
            _dominosDic.Clear();

            _start.Reset();
            _goal1.Reset();
            _goal2.Reset();
            _stick.Reset();
        }

        private bool GetStageTapPosition(out Vector3 position)
        {
            position = Vector3.zero;

            if(EventSystem.current.IsPointerOverGameObject())
            {
                return false;
            }

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            int layerMask = (1 << LayerMask.NameToLayer(Defines.StageLayerName) | 1 << LayerMask.NameToLayer(Defines.DefaultLayerName));
            if(Physics.Raycast(ray, out var hit, Mathf.Infinity, layerMask))
            {
                position = hit.point;
                return true;
            }
            return false;
        }

        private bool DominoMove()
        {
            if(GetStageTapPosition(out var position))
            {
                if(_targetDomino.Value)
                {
                    if(GetNewPositionEnable(_targetDomino.Value.transform.position + (position - _prevPos)))
                    {
                        _targetDomino.Value.transform.position += (position - _prevPos);
                        _prevPos = position;
                        return true;
                    }
                    else
                    {
                        AudioController.Instance.Play(Defines.DominoCreateErrorSoundName, false);

                        AllDominoUnSelected();

                        _dominoStatus = DominoStatus.None;
                        _targetDomino.Value = null;
                        return false;
                    }
                }
            }
            return false;
        }

        private void DominoRotate()
        {
            var x = Input.GetAxis("Mouse X");

            _targetDomino.Value?.transform.Rotate(Vector3.up, -x * Defines.DominoRotateSpeed);
        }

        private bool GetTapDomino(Collider collider, out Domino domino)
        {
            domino = null;
            if(!_dominosDic.ContainsKey(collider))
            {
                return false;
            }

            domino = _dominosDic[collider];
            return true;
        }

        private void TargetDominoChanged()
        {
            foreach(var d in _dominosDic.Values)
            {
                d.SetSelected(false);
            }

            _targetDomino.Value?.SetSelected(true);
        }

        private void AllDominoUnSelected()
        {
            // すべてのドミノを選択解除
            foreach(var domino in _dominosDic.Values)
            {
                domino.SetSelected(false);
            }
        }

        private void CreateDomino(Vector3 position)
        {
            AllDominoUnSelected();

            if(!GetNewPositionEnable(position))
            {
                AudioController.Instance.Play(Defines.DominoCreateErrorSoundName);
                return;
            }

            var newDomino = Instantiate(_dominoPrefab, position, Quaternion.identity);
            newDomino.Initialize(_dominosDic.Count);
            newDomino.EraseObservable
            .Subscribe(_ =>
            {
                _dominosDic.Remove(newDomino.Collider);
            })
            .AddTo(this);

            _dominosDic.Add(newDomino.Collider, newDomino);

            _dominoStatus = DominoStatus.None;
            AudioController.Instance.Play(Defines.DominoCreateSoundName, true);
        }

        private void DuplicateDomino(Transform origin)
        {
            AllDominoUnSelected();

            var add = origin.rotation * Vector3.back;
            var newPosition = origin.position + add * Defines.DuplicateDistance;

            if(!GetNewPositionEnable(newPosition))
            {
                AudioController.Instance.Play(Defines.DominoCreateErrorSoundName);
                return;
            }

            var newDomino = Instantiate(_dominoPrefab, newPosition, origin.rotation);
            newDomino.Initialize(_dominosDic.Count);
            newDomino.EraseObservable
            .Subscribe(_ =>
            {
                _dominosDic.Remove(newDomino.Collider);
            })
            .AddTo(this);

            _dominosDic.Add(newDomino.Collider, newDomino);

            _dominoStatus = DominoStatus.None;
            AudioController.Instance.Play(Defines.DominoCreateSoundName, true);
        }

        private bool GetNewPositionEnable(Vector3 position)
        {
            var distance1 = (position - _goal1.transform.position).magnitude;
            var distance2 = (position - _goal2.transform.position).magnitude;

            if(distance1 < _ngArea || distance2 < _ngArea)
            {
                return false;
            }
            return true;
        }

        public void PlayStart()
        {
            _start.PlayStart();

            var goalObservables = new List<IObservable<float>>();
            foreach(var goal in _goalDominos)
            {
                goal.PlayStart();
                goalObservables.Add(goal.GoalObservable);
            }

            goalObservables[0].Zip(goalObservables[1], (time1, time2) => Mathf.Abs(time1 - time2))
            .Subscribe(time =>
            {
                _goalTimeSubject.OnNext(time);
            })
            .AddTo(this);

            foreach(var domino in _dominosDic.Values)
            {
                domino.PlayStart();
            }

            _stick.PlayStart();
        }
    }
}

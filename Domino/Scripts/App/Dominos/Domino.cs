using Dominos.AppUtil;
using Dominos.Util;
using Dominos.Util.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Dominos.App
{
    public class Domino : MonoBehaviour
    {
        [SerializeField]
        private LineRenderer _lineRenderer;

        [SerializeField]
        protected Collider _collider;
        public Collider Collider => _collider;

        [SerializeField]
        protected Rigidbody _rigidbody;
        public Rigidbody Rigidbody => _rigidbody;

        private Subject<Unit> _eraseSubject = new Subject<Unit>();
        public IObservable<Unit> EraseObservable => _eraseSubject;

        private int _index = -1;
        public int Index => _index;

        private bool _isSelected = false;
        public bool IsSelected => _isSelected;

        private void Start()
        {
            this.UpdateAsObservable()
            .Where(_ => transform.position.y < Defines.DominoEraseThreshold)
            .Subscribe(_ =>
            {
                AudioController.Instance.Play(Defines.DominoDeleteSoundName, true);
                _eraseSubject.OnNext(Unit.Default);
                Destroy(gameObject);
            })
            .AddTo(this);
        }

        public void Initialize(int index)
        {
            _index = index;
            _lineRenderer.enabled = false;
        }

        public void SetSelected(bool value)
        {
            _isSelected = value;
            _lineRenderer.enabled = value;
        }

        private void OnCollisionEnter(Collision collision)
        {
            // 倒れ始めたら、次に倒すべきドミノよりも重たくする
            // そうすることで次のドミノが確実に倒れる
            if(collision.gameObject.layer != LayerMask.NameToLayer(Defines.StageLayerName))
            {
                _rigidbody.mass *= 3;
            }
        }

        public virtual void PlayStart()
        {
            SetLayerExtension.SetLayerRecursively(gameObject, LayerMask.NameToLayer(Defines.CollisionDominoLayerName));

        }

        public virtual void Reset()
        {
            Destroy(gameObject);
        }
    }
}

using Dominos.AppUtil;
using Dominos.AppUtil.Enums;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Dominos.App
{
    public class CameraController : MonoBehaviour
    {
        private CameraStatus _status = CameraStatus.None;
        private Pose _initialPose;

        [SerializeField]
        private bool _isRotatePitch = false;

        [SerializeField]
        private float _rotateXMax = 0;

        [SerializeField]
        private float _rotateXMin = 0;


        private void Start()
        {
            _initialPose = new Pose(transform.position, transform.rotation);

            this.UpdateAsObservable()
            .Where(_ => Input.GetMouseButtonDown(0))
            .Subscribe(_ =>
            {
                // タップ対象を取得
                if(!TapController.GetTapClosestObject(out var hit))
                {
                    // タップ対象がいなければカメラ回転開始
                    _status = CameraStatus.Rotate;
                }
            })
            .AddTo(this);

            this.UpdateAsObservable()
            .Where(_ => Input.GetMouseButton(0))
            .Subscribe(_ =>
            {
                if(_status == CameraStatus.Rotate)
                {
                    // 回転
                    CameraRotate();
                }
            })
            .AddTo(this);

            this.UpdateAsObservable()
            .Where(_ => Input.GetMouseButtonUp(0))
            .Subscribe(_ =>
            {
                // 操作完了
                _status = CameraStatus.None;
            })
            .AddTo(this);
        }

        private void CameraRotate()
        {
            if(EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            var x = Input.GetAxis("Mouse Y") * Defines.CmaeraRotateSpeed + transform.eulerAngles.x;
            var y = Input.GetAxis("Mouse X") * Defines.CmaeraRotateSpeed + transform.eulerAngles.y;

            if(x > 180)
            {
                x -= 360;
            }

            x = x > _rotateXMax ? _rotateXMax : x;
            x = x < _rotateXMin ? _rotateXMin : x;

            if(_isRotatePitch)
            {
                transform.eulerAngles = new Vector3(x, y, 0);
            }
            else
            {
                transform.eulerAngles = new Vector3(0, y, 0);
            }
        }

        public void Reset()
        {
            transform.position = _initialPose.position;
            transform.rotation = _initialPose.rotation;
        }
    }
}

using Dominos.AppUtil;
using UnityEngine;

namespace Dominos.App
{
    public class Stick : MonoBehaviour
    {
        [SerializeField]
        private Rigidbody _rigidbody;

        [SerializeField]
        private Collider _collider;

        private Pose _initialPose;

        private void Start()
        {
            // TODO:初期値は外部から取得
            _initialPose = new Pose(transform.position, transform.rotation);

            _collider.enabled = false;
        }

        public void PlayStart()
        {
            _collider.enabled = true;
            _rigidbody.AddForce(transform.forward * Defines.StickAddForce);
        }

        public void Reset()
        {
            transform.position = _initialPose.position;
            transform.rotation = _initialPose.rotation;

            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;

            _collider.enabled = false;
        }

        private void OnCollisionEnter(Collision collision)
        {
            Reset();
        }

    }
}

using Dominos.AppUtil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Dominos.App
{
    public class StartDomino : Domino
    {
        private Pose _initialPose;

        private void Start()
        {
            _initialPose = new Pose(transform.position, transform.rotation);
        }

        public override void PlayStart()
        {
            base.PlayStart();
        }

        public override void Reset()
        {
            _rigidbody.mass = Defines.DefaultDominoMass;

            transform.position = _initialPose.position;
            transform.rotation = _initialPose.rotation;
        }
    }
}

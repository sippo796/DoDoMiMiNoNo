using Dominos.AppUtil;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Dominos.App
{
    public class GoalDomino : Domino
    {
        private bool _isPlayStart = false;
        private float _startTime;
        private float _goalTime;

        private Subject<float> _goalSubject = new Subject<float>();
        public IObservable<float> GoalObservable => _goalSubject;

        private IDisposable _disposable;

        private Pose _initialPose;

        private void Start()
        {
            _initialPose = new Pose(transform.position, transform.rotation);
        }

        public override void PlayStart()
        {
            base.PlayStart();

            _isPlayStart = true;
            _startTime = Time.realtimeSinceStartup;

            _disposable = this.UpdateAsObservable()
            .Select( _=> transform.eulerAngles.x)
            .Select(x => x > 180 ? x - 360 : x)
            .FirstOrDefault(x => (0 < x && Defines.DominoDownThreshold < x) || (x < 0 && x < -Defines.DominoDownThreshold))
            .Subscribe(x =>
            {
                Debug.Log($"rotate x = {x}"); ;
                _goalTime = Time.realtimeSinceStartup;
                _goalSubject.OnNext(_goalTime - _startTime);
            })
            .AddTo(this);
        }

        public override void Reset()
        {
            _rigidbody.mass = Defines.DefaultDominoMass;
            _disposable?.Dispose();

            transform.position = _initialPose.position;
            transform.rotation = _initialPose.rotation;

        }
    }
}

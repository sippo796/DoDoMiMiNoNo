using DG.Tweening;
using Dominos.AppUtil;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Dominos.App
{
    public class UnlimitedManager : MonoBehaviour
    {
        [SerializeField]
        private Button _startButton;

        [SerializeField]
        private Button _resetButton;

        [SerializeField]
        private Button _giveupButton;

        [SerializeField]
        private Button _tweetButton;

        [SerializeField]
        private CanvasGroup _canvasGroup;

        [SerializeField]
        private Text _clearText;

        [SerializeField]
        private Text _clearTimeText;

        private DominoController _dominoController;
        private CameraController _cameraController;
        private bool _isGoal = false;

        private void Start()
        {
            _clearText.enabled = false;

            // ステージロード
            var stageLoad = Resources.Load(string.Format(Defines.UnlimitedStagePath, SaveParametors.TimeAtackStageNo));
            var stage = (GameObject)Instantiate(stageLoad);

            _dominoController = stage.GetComponentInChildren<DominoController>();
            _cameraController = stage.GetComponentInChildren<CameraController>();

            // ドミノ倒し開始
            _startButton.OnClickAsObservable()
            .Subscribe(_ => _dominoController.PlayStart())
            .AddTo(this);

            _resetButton.OnClickAsObservable()
            .Subscribe(_ =>
            {
                _dominoController.Reset();
                _cameraController.Reset();
            })
            .AddTo(this);

            _dominoController.GoalTimeObservable
            .Subscribe(time =>
            {
                _isGoal = true;
                _clearText.enabled = true;

                Observable.Timer(TimeSpan.FromSeconds(Defines.StartTime))
                .Subscribe(_ =>
                {
                    _clearTimeText.text = time.ToString();
                })
                .AddTo(this);
            })
            .AddTo(this);

            _giveupButton.OnClickAsObservable()
            .Subscribe(_ =>
            {
                _canvasGroup.DOFade(1, Defines.FadeTime)
                .OnComplete(() => SceneManager.LoadSceneAsync(Defines.SelectSceneName));
            })
            .AddTo(this);

            _tweetButton.OnClickAsObservable()
            .Subscribe(_ =>
            {
                // TODO:ツイート
            })
            .AddTo(this);
        }
    }
}

using Cysharp.Threading.Tasks;
using DG.Tweening;
using Dominos.AppUtil;
using Dominos.Util;
using naichilab;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Dominos.App
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        private Button _startButton;

        [SerializeField]
        private Text _playTimeText;

        [SerializeField]
        private Button _resetButton;

        [SerializeField]
        private Button _giveupButton;

        [SerializeField]
        private Button _tweetButton;

        [SerializeField]
        private Button _stageBackButton;

        [SerializeField]
        private Button _retryButton;

        [SerializeField]
        private CanvasGroup _fadeCanvasGroup;

        [SerializeField]
        private Text _resultTimeText;

        [SerializeField]
        private Text _resultDiffTimeText;

        [SerializeField]
        private Text _scoreText;

        [SerializeField]
        private CameraController _roomCameraController;

        [SerializeField]
        private RankingSceneManager _rankingManager;

        [SerializeField]
        private Canvas _rankingCanvas;

        [SerializeField]
        private ParticleSystem _confetti;

        [SerializeField]
        private Canvas _mainCanvas;

        private DominoController _dominoController;
        private CameraController _cameraController;
        private bool _isGoal = false;
        private float _playTime = 0f;

        private void Start()
        {
            // ステージロード
            var stageLoad = Resources.Load(string.Format(Defines.TimeAtackStagePath, SaveParametors.TimeAtackStageNo));
            var stage = (GameObject)Instantiate(stageLoad);

            _dominoController = stage.GetComponentInChildren<DominoController>();
            _cameraController = stage.GetComponentInChildren<CameraController>();

            if(!AudioController.Instance.IsPlay(Defines.GameMusicSoundName))
            {
                AudioController.Instance.FadeIn(Defines.GameMusicSoundName, Defines.FadeTime);
            }

            // フェードインし終えたら、時間計測開始
            _fadeCanvasGroup.DOFade(0, Defines.FadeTime)
            .OnComplete(() =>
            {
                this.UpdateAsObservable()
                .Where(_ => !_isGoal)
                .Subscribe(_ =>
                {
                    _playTime += Time.deltaTime;
                    _playTimeText.text = string.Format("{0:0.0}", _playTime);
                })
                .AddTo(this);
            });

            // ドミノ倒し開始
            _startButton.OnClickAsObservable()
            .Subscribe(_ =>
            {
                _startButton.enabled = false;
                _startButton.interactable = false;
                _dominoController.PlayStart();
                AudioController.Instance.Play(Defines.DominoStartSoundName, true);
                AudioController.Instance.Stop(Defines.GameMusicSoundName);
                AudioController.Instance.Play(Defines.DuringSoundName, true);
            })
            .AddTo(this);

            _resetButton.OnClickAsObservable()
            .Subscribe(_ =>
            {
                _startButton.enabled = true;
                _startButton.interactable = true;
                _dominoController.Reset();
                _cameraController.Reset();
                _roomCameraController.Reset();
                AudioController.Instance.Play(Defines.ButtonSoundName, true);
                AudioController.Instance.Stop(Defines.DuringSoundName);
                if(!AudioController.Instance.IsPlay(Defines.GameMusicSoundName))
                {
                    AudioController.Instance.FadeIn(Defines.GameMusicSoundName, Defines.FadeTime);
                }
            })
            .AddTo(this);

            var goalDisposable =  _dominoController.GoalTimeObservable
            .Subscribe(time =>
            {
                _confetti?.Play(true);

                AudioController.Instance.Stop(Defines.DominoStartSoundName);
                AudioController.Instance.Stop(Defines.DuringSoundName);
                AudioController.Instance.Play(Defines.ClearSoundName, true);

                time = time < 0.1f ? 0.1f : time;

                _isGoal = true;
                _resetButton.enabled = false;
                _resetButton.interactable = false;
                _resultTimeText.text = string.Format("{0:0.0}", _playTime);
                _resultDiffTimeText.text = string.Format("{0:0.0}", time);

                var score = (100000f/ time) / _playTime;
                _scoreText.text = ((int)score).ToString();

                DOVirtual.DelayedCall(Defines.StartTime, () =>
                {
                    _confetti.Stop();
                    AudioController.Instance.FadeIn(Defines.GameMusicSoundName, Defines.FadeTime);
                    _fadeCanvasGroup.DOFade(1, Defines.FadeTime)
                    .OnComplete(() =>
                    {
                        ShowRanking((int)score);
                    });
                });
            })
            .AddTo(_rankingCanvas.gameObject);

            _giveupButton.OnClickAsObservable()
            .Subscribe(_ =>
            {
                goalDisposable?.Dispose();

                AudioController.Instance.Play(Defines.ButtonSoundName, true);
                AudioController.Instance.FadeOut(Defines.GameMusicSoundName, Defines.FadeTime);
                AudioController.Instance.Stop(Defines.DuringSoundName);

                _fadeCanvasGroup.DOFade(1, Defines.FadeTime)
                .OnComplete(() =>
                {
                    SceneManager.LoadSceneAsync(Defines.SelectSceneName);
                });
            })
            .AddTo(this);

            _tweetButton.OnClickAsObservable()
            .Subscribe(_ =>
            {
                AudioController.Instance.Play(Defines.ButtonSoundName, true);
                StartCoroutine(TweetWithScreenShot.TweetManager.TweetWithScreenShot("ドミノを並べたよ！"));
            })
            .AddTo(this);

            _stageBackButton.OnClickAsObservable()
            .Subscribe(_ =>
            {
                AudioController.Instance.Play(Defines.ButtonSoundName, true);
                AudioController.Instance.FadeOut(Defines.GameMusicSoundName, Defines.FadeTime);
                _fadeCanvasGroup.DOFade(1, Defines.FadeTime)
                .OnComplete(() =>
                {
                    SceneManager.LoadSceneAsync(Defines.SelectSceneName);
                });
            })
            .AddTo(this);

            _retryButton.OnClickAsObservable()
            .Subscribe(_ =>
            {
                AudioController.Instance.Play(Defines.ButtonSoundName, true);
                _fadeCanvasGroup.DOFade(1, Defines.FadeTime)
                .OnComplete(() => SceneManager.LoadSceneAsync(Defines.TimeAtackSceneName));
            })
            .AddTo(this);
        }

        private void ShowRanking(int score)
        {
            _rankingCanvas.gameObject.SetActive(true);
            _rankingManager.gameObject.SetActive(true);

            RankingLoader.Instance.SendScoreAndShowRanking(score, SaveParametors.TimeAtackStageNo);

            _fadeCanvasGroup.DOFade(0, Defines.FadeTime);
        }
    }
}

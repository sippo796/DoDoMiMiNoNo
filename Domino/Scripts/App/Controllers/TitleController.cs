using DG.Tweening;
using Dominos.AppUtil;
using Dominos.Util;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Dominos.App
{
    public class TitleController : MonoBehaviour
    {
        [SerializeField]
        private Button _startButton;

        [SerializeField]
        private CanvasGroup _canvasGroup;

        [SerializeField]
        private CanvasGroup _fadeCanvasGroup;

        private void Start()
        {
            _fadeCanvasGroup.DOFade(0, Defines.FadeTime);
            AudioController.Instance.FadeIn(Defines.OpeningSoundName, Defines.FadeTime);

            _startButton.OnClickAsObservable()
            .Subscribe(_ => PlayStart())
            .AddTo(this);
        }

        private void PlayStart()
        {
            AudioController.Instance.Play(Defines.ButtonSoundName, true);
            _fadeCanvasGroup
            .DOFade(1, Defines.FadeTime)
            .OnComplete(() => SceneManager.LoadSceneAsync(Defines.SelectSceneName));
        }
    }
}

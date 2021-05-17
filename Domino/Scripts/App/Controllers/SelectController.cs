using System.Collections;
using System.Collections.Generic;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;
using Dominos.AppUtil;
using UnityEngine.SceneManagement;
using System.Linq;
using Dominos.Util;

namespace Dominos.App
{
    public class SelectController : MonoBehaviour
    {
        [SerializeField]
        private List<Button> _timeAtackButtons = new List<Button>();

        [SerializeField]
        private CanvasGroup _fadeCanvas;

        private void Start()
        {
            if(!AudioController.Instance.IsPlay(Defines.OpeningSoundName))
            {
                AudioController.Instance.FadeIn(Defines.OpeningSoundName, Defines.FadeTime);
            }

            _fadeCanvas.DOFade(0, Defines.FadeTime);

            var timeAtackButtons = _timeAtackButtons.Select((button, index) => new { Value = button, Index = index });
            foreach(var button in timeAtackButtons)
            {
                button.Value.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    SaveParametors.TimeAtackStageNo = button.Index;

                    AudioController.Instance.Play(Defines.ButtonSoundName, true);
                    AudioController.Instance.FadeOut(Defines.OpeningSoundName, Defines.FadeTime);

                    _fadeCanvas.DOFade(1, Defines.FadeTime)
                    .OnComplete(() => SceneManager.LoadSceneAsync(Defines.TimeAtackSceneName));
                })
                .AddTo(this);
            }
        }
    }
}

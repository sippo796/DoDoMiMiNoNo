using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dominos.Util
{
    public class AudioController : MonoBehaviour
    {
        private static AudioController _instance;
        public static AudioController Instance => _instance;

        private void Awake()
        {
            if(!_instance)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// AudioSouceの管理マップ
        /// </summary>
        private Dictionary<string, AudioSource> _audios = new Dictionary<string, AudioSource>();

        public void Initialize()
        {
            if(_audios.Count == 0)
            {
                this.transform.GetComponentsInChildren<Transform>().ToList().ForEach(o =>
                {
                    _audios.Add(o.gameObject.name, o.gameObject.GetComponent<AudioSource>());
                });
            }
        }

        private void ContainsKey(string name)
        {
            if(!_audios.ContainsKey(name))
            {
                _audios.Add(name, this.transform.Find(name).GetComponent<AudioSource>());
            }
        }

        public void Play(string name, bool overlap = true)
        {
            ContainsKey(name);

            var audio = _audios[name];
            // オーバーラップ可の音は、そのまま再生
            if(overlap)
            {
                audio?.Play();
            }
            else
            {
                // オーバーラップ不可の音は、確認してから再生
                if(audio && !audio.isPlaying)
                {
                    audio.Play();
                }
            }
        }

        public void FadeIn(string name, float time, bool overlap = true)
        {
            ContainsKey(name);

            var audio = this._audios[name];
            Play(name, true);
            audio.DOFade(0, 0);
            audio.DOFade(1, time);
        }

        public void FadeOut(string name, float time, bool isStop = true)
        {
            ContainsKey(name);

            var audio = _audios[name];
            if(audio)
            {
                audio.DOFade(0, time)
                .OnComplete(() =>
                {
                    if(isStop) { audio.Stop(); }
                });
            }
        }

        public bool IsPlay(string name)
        {
            ContainsKey(name);

            var audio = _audios[name];
            return audio ? audio.isPlaying : false;
        }

        public void Stop(string name)
        {
            ContainsKey(name);
            _audios[name]?.Stop();
        }

        public void Pause(string name)
        {
            ContainsKey(name);
            _audios[name]?.Pause();
        }

        public void UnPause(string name)
        {
            ContainsKey(name);
            _audios[name]?.UnPause();
        }
    }
}

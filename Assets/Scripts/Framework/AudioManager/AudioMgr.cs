using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JLXB.Framework;
using UnityEngine;
using UnityEngine.Events;

namespace JLXB.Framework.Audio
{

    /// <summary>
    /// 音频管理者
    /// </summary>
    public class AudioMgr : Singleton<AudioMgr>
    {
        /// <summary>
        /// 是否静音初始值
        /// </summary>
        public bool MuteDefault = false;

        /// <summary>
        /// 背景音乐优先级初始值
        /// </summary>
        public int BackgroundPriorityDefault = 0;

        /// <summary>
        /// 单通道音效优先级初始值
        /// </summary>
        public int SinglePriorityDefault = 10;

        /// <summary>
        /// 多通道音效优先级初始值
        /// </summary>
        public int MultiplePriorityDefault = 20;

        /// <summary>
        /// 世界音效优先级初始值
        /// </summary>
        public int WorldPriorityDefault = 30;

        /// <summary>
        /// 背景音乐音量初始值
        /// </summary>
        public float BackgroundVolumeDefault = 0.6f;

        /// <summary>
        /// 单通道音效音量初始值
        /// </summary>
        public float SingleVolumeDefault = 1;

        /// <summary>
        /// 多通道音效音量初始值
        /// </summary>
        public float MultipleVolumeDefault = 1;

        /// <summary>
        /// 世界音效音量初始值
        /// </summary>
        public float WorldVolumeDefault = 1;

        /// <summary>
        /// 单通道音效播放结束事件
        /// </summary>
        public UnityAction SingleSoundEndOfPlayEvent;

        private GameObject _audioSourceGo;

        // 背景音乐：全局只有一个，不同的场景不同的时间不同的天气都会有不同的背景音乐，其主要作用是为了烘托氛围。所以大部分的背景音乐都是2d的且音量是所有音效中最低的，目的为不突兀不刺激为主。
        private AudioSource _backgroundAudio;

        // 人声配音：全局只有一个，经常会用在人物对话，npc讲话等人声场景下。当播放新的声音时会把当前的声音覆盖掉。
        private AudioSource _singleAudio;

        // 技能ui音效：全局可以多个，在触发ui的时候或者释放技能的时候我们的配音是可以多个可重叠的。
        private List<AudioSource> _multipleAudios;

        // 世界3d音效：全局可以多个，世界音效其实就是3d音，我们一般用在固定位置范围内的音效，通过设置触发距离听到音量不一样的空间声音
        private Dictionary<GameObject, AudioSource> _worldAudios;

        private bool _singleSoundPlayDetector;
        private bool _isMute;
        private int _backgroundPriority;
        private int _singlePriority;
        private int _multiplePriority;
        private int _worldPriority;
        private float _backgroundVolume;
        private float _singleVolume;
        private float _multipleVolume;
        private float _worldVolume;

        public void Init()
        {
            _audioSourceGo = new GameObject("AudioSources");
            Object.DontDestroyOnLoad(_audioSourceGo);

            _backgroundAudio =
                CreateAudioSource("BackgroundAudio", BackgroundPriorityDefault, BackgroundVolumeDefault, 1, 0);
            _singleAudio = CreateAudioSource("SingleAudio", SinglePriorityDefault, SingleVolumeDefault, 1, 0);
            _multipleAudios = new List<AudioSource>();
            _worldAudios = new Dictionary<GameObject, AudioSource>();
            Mute = MuteDefault;
            BackgroundPriority = BackgroundPriorityDefault;
            SinglePriority = SinglePriorityDefault;
            MultiplePriority = MultiplePriorityDefault;
            WorldPriority = WorldPriorityDefault;
            BackgroundVolume = BackgroundVolumeDefault;
            SingleVolume = SingleVolumeDefault;
            MultipleVolume = MultipleVolumeDefault;
            WorldVolume = WorldVolumeDefault;
            SingleSoundEndOfPlayEvent = null;
            StopBackgroundMusic();
            StopSingleSound();
            StopAllMultipleSound();
            StopAllWorldSound();
            MonoMgr.Instance.AddUpdateListener(UpdateEvent);
        }

        private AudioMgr()
        {
        }

        private void UpdateEvent()
        {
            if (!_singleSoundPlayDetector || _singleAudio.isPlaying) return;
            _singleSoundPlayDetector = false;
            SingleSoundEndOfPlayEvent?.Invoke();
        }

        /// <summary>
        /// 静音
        /// </summary>
        public bool Mute
        {
            get => _isMute;
            set
            {
                if (_isMute == value) return;
                _isMute = value;
                _backgroundAudio.mute = _isMute;
                _singleAudio.mute = _isMute;
                foreach (var t in _multipleAudios)
                {
                    t.mute = _isMute;
                }

                foreach (var audio in _worldAudios)
                {
                    audio.Value.mute = _isMute;
                }
            }
        }

        /// <summary>
        /// 背景音乐优先级
        /// </summary>
        public int BackgroundPriority
        {
            get => _backgroundPriority;
            set
            {
                if (_backgroundPriority == value) return;
                _backgroundPriority = value;
                _backgroundAudio.priority = _backgroundPriority;
            }
        }

        /// <summary>
        /// 单通道音效优先级
        /// </summary>
        public int SinglePriority
        {
            get => _singlePriority;
            set
            {
                if (_singlePriority == value) return;
                _singlePriority = value;
                _singleAudio.priority = _singlePriority;
            }
        }

        /// <summary>
        /// 多通道音效优先级
        /// </summary>
        public int MultiplePriority
        {
            get => _multiplePriority;
            set
            {
                if (_multiplePriority == value) return;
                _multiplePriority = value;
                foreach (var t in _multipleAudios)
                {
                    t.priority = _multiplePriority;
                }
            }
        }

        /// <summary>
        /// 世界音效优先级
        /// </summary>
        public int WorldPriority
        {
            get => _worldPriority;
            set
            {
                if (_worldPriority == value) return;
                _worldPriority = value;
                foreach (var audio in _worldAudios)
                {
                    audio.Value.priority = _worldPriority;
                }
            }
        }

        /// <summary>
        /// 背景音乐音量
        /// </summary>
        public float BackgroundVolume
        {
            get => _backgroundVolume;
            set
            {
                if (Mathf.Approximately(_backgroundVolume, value)) return;
                _backgroundVolume = value;
                _backgroundAudio.volume = _backgroundVolume;
            }
        }

        /// <summary>
        /// 单通道音效音量
        /// </summary>
        public float SingleVolume
        {
            get => _singleVolume;
            set
            {
                if (Mathf.Approximately(_singleVolume, value)) return;
                _singleVolume = value;
                _singleAudio.volume = _singleVolume;
            }
        }

        /// <summary>
        /// 多通道音效音量
        /// </summary>
        public float MultipleVolume
        {
            get => _multipleVolume;
            set
            {
                if (Mathf.Approximately(_multipleVolume, value)) return;
                _multipleVolume = value;
                foreach (var t in _multipleAudios)
                {
                    t.volume = _multipleVolume;
                }
            }
        }

        /// <summary>
        /// 世界音效音量
        /// </summary>
        public float WorldVolume
        {
            get => _worldVolume;
            set
            {
                if (Mathf.Approximately(_worldVolume, value)) return;
                _worldVolume = value;
                foreach (var audio in _worldAudios)
                {
                    audio.Value.volume = _worldVolume;
                }
            }
        }

        /// <summary>
        /// 播放背景音乐
        /// </summary>
        /// <param name="clip">音乐剪辑</param>
        /// <param name="isLoop">是否循环</param>
        /// <param name="speed">播放速度</param>
        public void PlayBackgroundMusic(AudioClip clip, bool isLoop = true, float speed = 1)
        {
            if (_backgroundAudio.isPlaying)
            {
                _backgroundAudio.Stop();
            }

            _backgroundAudio.clip = clip;
            _backgroundAudio.loop = isLoop;
            _backgroundAudio.pitch = speed;
            _backgroundAudio.Play();
        }

        /// <summary>
        /// 暂停播放背景音乐
        /// </summary>
        /// <param name="isGradual">是否渐进式</param>
        public void PauseBackgroundMusic(bool isGradual = true)
        {
            if (isGradual)
            {
                FadeVolume(_backgroundAudio, 0f, 2f, () =>
                {
                    _backgroundAudio.volume = BackgroundVolume;
                    _backgroundAudio.Pause();
                });
            }
            else
            {
                _backgroundAudio.Pause();
            }
        }

        /// <summary>
        /// 恢复播放背景音乐
        /// </summary>
        /// <param name="isGradual">是否渐进式</param>
        public void UnPauseBackgroundMusic(bool isGradual = true)
        {
            if (isGradual)
            {
                _backgroundAudio.UnPause();
                _backgroundAudio.volume = 0;
                FadeVolume(_backgroundAudio, BackgroundVolume, 2f);
            }
            else
            {
                _backgroundAudio.UnPause();
            }
        }

        /// <summary>
        /// 停止播放背景音乐
        /// </summary>
        public void StopBackgroundMusic()
        {
            if (_backgroundAudio.isPlaying)
            {
                _backgroundAudio.Stop();
            }
        }

        /// <summary>
        /// 播放单通道音效
        /// </summary>
        /// <param name="clip">音乐剪辑</param>
        /// <param name="isLoop">是否循环</param>
        /// <param name="speed">播放速度</param>
        public void PlaySingleSound(AudioClip clip, bool isLoop = false, float speed = 1)
        {
            if (_singleAudio.isPlaying)
            {
                _singleAudio.Stop();
            }

            _singleAudio.clip = clip;
            _singleAudio.loop = isLoop;
            _singleAudio.pitch = speed;
            _singleAudio.Play();
            _singleSoundPlayDetector = true;
        }

        /// <summary>
        /// 暂停播放单通道音效
        /// </summary>
        /// <param name="isGradual">是否渐进式</param>
        public void PauseSingleSound(bool isGradual = true)
        {
            if (isGradual)
            {
                FadeVolume(_singleAudio, 0, 2f, () =>
                {
                    _singleAudio.volume = SingleVolume;
                    _singleAudio.Pause();
                });
            }
            else
            {
                _singleAudio.Pause();
            }
        }

        /// <summary>
        /// 恢复播放单通道音效
        /// </summary>
        /// <param name="isGradual">是否渐进式</param>
        public void UnPauseSingleSound(bool isGradual = true)
        {
            if (isGradual)
            {
                _singleAudio.UnPause();
                _singleAudio.volume = 0;
                FadeVolume(_singleAudio, SingleVolume, 2);
            }
            else
            {
                _singleAudio.UnPause();
            }
        }

        /// <summary>
        /// 停止播放单通道音效
        /// </summary>
        public void StopSingleSound()
        {
            if (_singleAudio.isPlaying)
            {
                _singleAudio.Stop();
            }
        }

        /// <summary>
        /// 播放多通道音效
        /// </summary>
        /// <param name="clip">音乐剪辑</param>
        /// <param name="isLoop">是否循环</param>
        /// <param name="speed">播放速度</param>
        public void PlayMultipleSound(AudioClip clip, bool isLoop = false, float speed = 1)
        {
            var audio = ExtractIdleMultipleAudioSource();
            audio.clip = clip;
            audio.loop = isLoop;
            audio.pitch = speed;
            audio.Play();
        }

        /// <summary>
        /// 停止播放指定的多通道音效
        /// </summary>
        /// <param name="clip">音乐剪辑</param>
        public void StopMultipleSound(AudioClip clip)
        {
            foreach (var audioSource in _multipleAudios.Where(audioSource => audioSource.isPlaying)
                         .Where(audioSource => audioSource.clip == clip))
            {
                audioSource.Stop();
            }
        }

        /// <summary>
        /// 停止播放所有多通道音效
        /// </summary>
        public void StopAllMultipleSound()
        {
            foreach (var audioSource in _multipleAudios.Where(t => t.isPlaying))
            {
                audioSource.Stop();
            }
        }

        /// <summary>
        /// 销毁所有闲置中的多通道音效的音源
        /// </summary>
        public void ClearIdleMultipleAudioSource()
        {
            for (var i = 0; i < _multipleAudios.Count; i++)
            {
                if (_multipleAudios[i].isPlaying) continue;
                var audio = _multipleAudios[i];
                _multipleAudios.RemoveAt(i);
                i -= 1;
                Object.Destroy(audio.gameObject);
            }
        }

        /// <summary>
        /// 播放世界音效
        /// </summary>
        /// <param name="attachTarget">附加目标</param>
        /// <param name="clip">音乐剪辑</param>
        /// <param name="isLoop">是否循环</param>
        /// <param name="speed">播放速度</param>
        public void PlayWorldSound(GameObject attachTarget, AudioClip clip, bool isLoop = false, float speed = 1)
        {
            if (_worldAudios.TryGetValue(attachTarget, out var worldAudio))
            {
                if (worldAudio.isPlaying)
                {
                    worldAudio.Stop();
                }

                worldAudio.clip = clip;
                worldAudio.loop = isLoop;
                worldAudio.pitch = speed;
                worldAudio.Play();
            }
            else
            {
                var audio = AttachAudioSource(attachTarget, WorldPriority, WorldVolume, 1, 1);
                _worldAudios.Add(attachTarget, audio);
                audio.clip = clip;
                audio.loop = isLoop;
                audio.pitch = speed;
                audio.Play();
            }
        }

        /// <summary>
        /// 暂停播放指定的世界音效
        /// </summary>
        /// <param name="attachTarget">附加目标</param>
        /// <param name="isGradual">是否渐进式</param>
        public void PauseWorldSound(GameObject attachTarget, bool isGradual = true)
        {
            if (!_worldAudios.TryGetValue(attachTarget, out var audio)) return;
            if (isGradual)
            {
                FadeVolume(audio, 0, 2f, () =>
                {
                    audio.volume = WorldVolume;
                    audio.Pause();
                });
            }
            else
            {
                audio.Pause();
            }
        }

        /// <summary>
        /// 恢复播放指定的世界音效
        /// </summary>
        /// <param name="attachTarget">附加目标</param>
        /// <param name="isGradual">是否渐进式</param>
        public void UnPauseWorldSound(GameObject attachTarget, bool isGradual = true)
        {
            if (!_worldAudios.TryGetValue(attachTarget, out var audio)) return;
            if (isGradual)
            {
                audio.UnPause();
                audio.volume = 0;
                FadeVolume(audio, WorldVolume, 2f);
            }
            else
            {
                audio.UnPause();
            }
        }

        /// <summary>
        /// 停止播放所有的世界音效
        /// </summary>
        public void StopAllWorldSound()
        {
            foreach (var audio in _worldAudios.Where(audio => audio.Value.isPlaying))
            {
                audio.Value.Stop();
            }
        }

        /// <summary>
        /// 销毁所有闲置中的世界音效的音源
        /// </summary>
        public void ClearIdleWorldAudioSource()
        {
            var removeSet = new HashSet<GameObject>();
            foreach (var audio in _worldAudios.Where(audio => !audio.Value.isPlaying))
            {
                removeSet.Add(audio.Key);
                if (audio.Value != null)
                    Object.Destroy(audio.Value);
            }

            foreach (var item in removeSet)
            {
                _worldAudios.Remove(item);
            }
        }

        //创建一个音源
        private AudioSource CreateAudioSource(string name, int priority, float volume, float speed, float spatialBlend)
        {
            var audioObj = new GameObject(name);
            audioObj.transform.SetParent(_audioSourceGo.transform);
            audioObj.transform.localPosition = Vector3.zero;
            audioObj.transform.localRotation = Quaternion.identity;
            audioObj.transform.localScale = Vector3.one;
            var audio = audioObj.AddComponent<AudioSource>();
            audio.playOnAwake = false;
            audio.priority = priority;
            audio.volume = volume;
            audio.pitch = speed;
            audio.spatialBlend = spatialBlend;
            audio.mute = _isMute;
            return audio;
        }

        //附加一个音源
        private AudioSource AttachAudioSource(GameObject target, int priority, float volume, float speed,
            float spatialBlend)
        {
            var audio = target.AddComponent<AudioSource>();
            audio.playOnAwake = false;
            audio.priority = priority;
            audio.volume = volume;
            audio.pitch = speed;
            audio.spatialBlend = spatialBlend;
            audio.mute = _isMute;
            return audio;
        }

        //提取闲置中的多通道音源
        private AudioSource ExtractIdleMultipleAudioSource()
        {
            foreach (var audioSource in _multipleAudios.Where(t => !t.isPlaying))
            {
                return audioSource;
            }

            var audio = CreateAudioSource("MultipleAudio", MultiplePriority, MultipleVolume, 1, 0);
            _multipleAudios.Add(audio);
            return audio;
        }

        private static IEnumerator FadeCoroutine(AudioSource source, float targetVolume, float duration,
            UnityAction onComplete = null)
        {
            var startVolume = source.volume;
            float time = 0;
            while (time < duration)
            {
                time += Time.deltaTime;
                source.volume = Mathf.Lerp(startVolume, startVolume, time / duration);
                yield return null;
            }

            source.volume = targetVolume;
            onComplete?.Invoke();
        }

        private static void FadeVolume(AudioSource source, float targetVolume, float duration,
            UnityAction onComplete = null)
        {
            MonoMgr.Instance.StartCoroutine(FadeCoroutine(source, targetVolume, duration, onComplete));
        }
    }
}
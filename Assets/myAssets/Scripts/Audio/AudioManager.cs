using System.Collections.Generic;
using System.Collections;
using UnityEngine.Audio;
using UnityEngine;

namespace Towy.Utilities.Audio
{
    [RequireComponent(typeof(AudioListener))]
    public class AudioManager : GenericGameSettingsManager<AudioManager, AudioGameSettingsData>
    {
        private static List<Coroutine> loopCoroutines = new List<Coroutine>();
        [HideInInspector] public List<AudioSource> m_OneShotSoundSources;
        [HideInInspector] public List<AudioSource> m_LoopSoundSources;
        [HideInInspector] public List<AudioSource> m_MusicSources; public static int lastAddIndex = 0;
        
        /// <summary>
        /// Reference to audio mixer for volume changing
        /// </summary>
        public AudioMixer audioMixer;

        public static bool isThemeStarted = false;
        
        /// <summary>
        /// Just for enable / disable component
        /// </summary>
        public void Start(){}

        /// <summary>
        /// Initialize volumes
        /// </summary>
        protected override void OnAwake()
        {
            base.OnAwake();

            AudioManager[] objs = FindObjectsOfType<AudioManager>();

            if (objs.Length > 1)
            {
                Destroy(this.gameObject);
            }

            DontDestroyOnLoad(this.gameObject);

            if (m_MusicSources == null)
                m_MusicSources = new List<AudioSource>();

            if (m_OneShotSoundSources == null)
                m_OneShotSoundSources = new List<AudioSource>();

            if (m_LoopSoundSources == null)
                m_LoopSoundSources = new List<AudioSource>();

            GameObject startMusic = new GameObject("MusicSource", typeof(AudioSource));
            startMusic.transform.parent = transform;
            m_MusicSources.Add(startMusic.GetComponent<AudioSource>());
            m_MusicSources[0].volume = 100f;
            m_MusicSources[0].mute = GameSettingsData.musicMuted;
            m_MusicSources[0].loop = true;
            m_MusicSources[0].playOnAwake = false;
            m_MusicSources[0].outputAudioMixerGroup = audioMixer.FindMatchingGroups("Music")[0];

            SetVolumes(GameSettingsData.masterVolume, GameSettingsData.musicVolume, GameSettingsData.soundVolume);
            MuteVolumes(GameSettingsData.musicMuted, GameSettingsData.soundMuted);
        }
        
        private void FadeMusicOut(float duration)
        {
            float delay = 0f;
            float volume = 0f;
            StartCoroutine(FadeMusic(volume, delay, duration, FadeMusicType.FadeOut));
        }
        private void FadeMusicIn(AudioClip clip, float delay, float duration, SongOverrideMethod method, float volume = 1f, AudioPlayMethod playMethod = AudioPlayMethod.Loop, bool startPaused = false)
        {
            if (method == SongOverrideMethod.AsNewAudioClip)
            {
                Instance.m_MusicSources[0].volume = volume;
                Instance.m_MusicSources[0].mute = GameSettingsData.musicMuted;
                Instance.m_MusicSources[0].clip = clip;
                Instance.m_MusicSources[0].loop = playMethod == AudioPlayMethod.Loop ? true : false;
                Instance.m_MusicSources[0].Play();

                if(startPaused)
                {
                    Instance.m_MusicSources[0].Pause();
                }

                StartCoroutine(FadeMusic(volume, delay, duration, FadeMusicType.FadeIn));
            }
            else if (method == SongOverrideMethod.AsNewAudioSource)
            {
                GameObject audioSource = new GameObject("MusicSource", typeof(AudioSource));

                audioSource.transform.parent = transform;
                m_MusicSources.Add(audioSource.GetComponent<AudioSource>());

                lastAddIndex++;

                Instance.m_MusicSources[lastAddIndex].volume = volume;
                Instance.m_MusicSources[lastAddIndex].mute = GameSettingsData.musicMuted;
                Instance.m_MusicSources[lastAddIndex].clip = clip;
                Instance.m_MusicSources[lastAddIndex].loop = playMethod == AudioPlayMethod.Loop ? true : false;
                Instance.m_MusicSources[lastAddIndex].Play();

                if(startPaused)
                {
                    Instance.m_MusicSources[lastAddIndex].Pause();
                }

                StartCoroutine(FadeMusic(volume, delay, duration, FadeMusicType.FadeIn));
            }
        }
        private IEnumerator FadeMusic(float fadeToVolume, float delay, float duration, FadeMusicType method)
        {
            yield return new WaitForSeconds(delay);

            float elapsed = 0f;

            //
            if (method == FadeMusicType.FadeIn)
            {
                while (duration > 0)
                {
                    float t = elapsed / duration;
                    float volume = Mathf.Lerp(0f, fadeToVolume, t);

                    Instance.m_MusicSources[lastAddIndex > 0 ? lastAddIndex - 1 : 0].volume = volume;

                    elapsed += Time.deltaTime;
                    duration -= Time.deltaTime;
                    yield return 0;
                }
            }
            else if (method == FadeMusicType.FadeOut)
            {

                float cVoume = Instance.m_MusicSources[0].volume;

                while (duration > 0)
                {
                    float t = elapsed / duration;
                    float volume = Mathf.Lerp(cVoume, fadeToVolume, t);
                    Instance.m_MusicSources[lastAddIndex > 0 ? lastAddIndex - 1 : 0].volume = volume;

                    elapsed += Time.deltaTime;
                    duration -= Time.deltaTime;
                    yield return 0;
                }

                StopMusic(lastAddIndex > 0 ? lastAddIndex - 1 : 0);
            }


        }
    
        public static void PlayMusicWithIntro(AudioClip intro, AudioClip loop, float volume = 100f, bool fade = false, float fadeDuration = 0f, SongOverrideMethod overrideMethod = SongOverrideMethod.AsNewAudioSource)
        {
            loopCoroutines.Add(Instance.StartCoroutine(PlayMusicWithIntroCoroutine(intro, loop, volume, false, 0f, SongOverrideMethod.AsNewAudioClip)));
        }
        private static IEnumerator PlayMusicWithIntroCoroutine(AudioClip intro, AudioClip loop, float volume = 100f, bool fade = false, float fadeDuration = 0f, SongOverrideMethod overrideMethod = SongOverrideMethod.AsNewAudioSource)
        {
            Instance.FadeMusicIn(intro, 0f, fadeDuration / 2, SongOverrideMethod.AsNewAudioSource, volume / 100f, AudioPlayMethod.OneShot, false);
            PlayMusic(loop, volume, false, 0f, AudioPlayMethod.Loop, SongOverrideMethod.AsNewAudioSource, true);

            yield return new WaitForSeconds(intro.length - 0.05f);

            ResumeMusic(0);
        }
        
        public static void ResumeMusic(int index)
        {
            Instance.m_MusicSources[index].Play();
        }

        public static void PlayMusic(AudioClip clip, float volume = 100f, bool fade = false, float fadeDuration = 0f, AudioPlayMethod playMethod = AudioPlayMethod.Loop, SongOverrideMethod overrideMethod = SongOverrideMethod.AsNewAudioSource, bool startPaused = false)
        {
            isThemeStarted = true;
            
            if (fade)
            {
                if (Instance.m_MusicSources[lastAddIndex].isPlaying && overrideMethod == SongOverrideMethod.AsNewAudioSource)
                {
                    Instance.FadeMusicOut(fadeDuration / 2);
                    Instance.FadeMusicIn(clip, fadeDuration / 2, fadeDuration / 2, SongOverrideMethod.AsNewAudioSource, volume / 100f, playMethod, startPaused);
                }
                else
                {
                    float delay = 0f;
                    Instance.FadeMusicIn(clip, delay, fadeDuration, SongOverrideMethod.AsNewAudioClip, volume / 100f, playMethod, startPaused);
                }
            }
            else
            {
                Instance.m_MusicSources[0].outputAudioMixerGroup = Instance.audioMixer.FindMatchingGroups("Music")[0];
                Instance.m_MusicSources[0].loop = playMethod == AudioPlayMethod.Loop ? true : false;

                Instance.m_MusicSources[0].mute = GameSettingsData.musicMuted;
                Instance.m_MusicSources[0].volume = volume / 100f;
                Instance.m_MusicSources[0].clip = clip;
                Instance.m_MusicSources[0].Play();

                if(startPaused)
                {
                    Instance.m_MusicSources[0].Pause();
                }
            }
        }

        public static void PlayMusicWithImmediateFadeIn(AudioClip clip, float volume = 100f, float fadeDuration = 0f)
        {
            if (Instance.m_MusicSources[lastAddIndex].isPlaying)
            {
                Instance.FadeMusicOut(fadeDuration / 2);
                Instance.FadeMusicIn(clip, 0f, fadeDuration / 2, SongOverrideMethod.AsNewAudioSource, volume / 100f);
            }
            else
            {
                float delay = 0f;
                Instance.FadeMusicIn(clip, delay, fadeDuration, SongOverrideMethod.AsNewAudioClip, volume / 100f);
            }
        }
        public static void PlayMusicWithImmediateFadeInAndIntro(AudioClip intro, AudioClip loop, float volume = 100f, float fadeDuration = 0f)
        {
            if (Instance.m_MusicSources[lastAddIndex].isPlaying)
            {
                Instance.FadeMusicOut(fadeDuration / 2);
                PlayMusicWithIntro(intro, loop, volume, true, fadeDuration / 2, SongOverrideMethod.AsNewAudioSource);
            }
            else
            {
                PlayMusicWithIntro(intro, loop, volume, true, fadeDuration, SongOverrideMethod.AsNewAudioClip);
            }
        }
        
        public static IEnumerator StopMenuMusic()
        {
            // Prevent menu song start
            foreach (Coroutine loopCoroutine in loopCoroutines)
            {
                Instance.StopCoroutine(loopCoroutine);
            }
            
            // Destory previous menu song source gameobject
            Destroy(Instance.m_MusicSources[lastAddIndex].gameObject);

            // Wait for an eventual end of a song swap
            yield return new WaitForSeconds(0.1f);

            // Clear music sources from null values and updating the lastAddedIndex
            ClearMusicSources();

            // Clear the menu song loop coroutines
            loopCoroutines.Clear();

            // End
            yield break;
        }
        public static void StopMusic(int index)
        {
            StopMusic(index, false, 0f);
        }
        public static void StopMusic(int index, bool fade, float fadeDuration)
        {
            if (Instance.m_MusicSources[index].isPlaying)
            {
                if (fade)
                {
                    Instance.FadeMusicOut(fadeDuration);
                }
                else
                {
                    Instance.m_MusicSources[index].Stop();
                    GameObject source = Instance.m_MusicSources[index].gameObject;
                    Instance.m_MusicSources.Remove(Instance.m_MusicSources[index]);
                    Destroy(source);

                    lastAddIndex--;
                }
            }
        }
        
        public static void StopEffect(int index)
        {
            StopLoopOfEffectAt(index, false, 0f);
        }
        public static void StopLoopOfEffectAt(int index, bool fade, float fadeDuration)
        {

            if (Instance.m_LoopSoundSources[index].isPlaying)
            {
                if (fade)
                {
                    //Instance.FadeMusicOut(fadeDuration);
                    //bisogna creare un fade out dei souni in loop
                    Instance.m_LoopSoundSources[index].Stop();
                    Destroy(Instance.m_LoopSoundSources[index].gameObject);
                    Instance.m_LoopSoundSources.RemoveAt(index);
                }
                else
                {
                    Instance.m_LoopSoundSources[index].Stop();
                    Destroy(Instance.m_LoopSoundSources[index].gameObject);
                    Instance.m_LoopSoundSources.RemoveAt(index);
                }
            }
        }
        
        private AudioSource CreateSoundSource(AudioPlayMethod method)
        {
            GameObject go = new GameObject(method.ToString() + "Source", typeof(AudioSource));
            go.transform.parent = transform;

            AudioSource soundSource = go.GetComponent<AudioSource>();

            if (method == AudioPlayMethod.OneShot)
            {

                soundSource.mute = GameSettingsData.soundMuted;
                soundSource.loop = false;
                soundSource.playOnAwake = false;
                soundSource.outputAudioMixerGroup = Instance.audioMixer.FindMatchingGroups("Sound")[0];

                if (m_OneShotSoundSources == null)
                    m_OneShotSoundSources = new List<AudioSource>();
                m_OneShotSoundSources.Add(soundSource);
            }
            else if (method == AudioPlayMethod.Loop)
            {
                soundSource.mute = GameSettingsData.soundMuted;
                soundSource.loop = true;
                soundSource.playOnAwake = false;
                soundSource.outputAudioMixerGroup = Instance.audioMixer.FindMatchingGroups("Sound")[0];

                if (m_LoopSoundSources == null)
                    m_LoopSoundSources = new List<AudioSource>();
            }

            return soundSource;
        }
        private IEnumerator RemoveSoundSource(AudioSource sfxSource)
        {
            yield return new WaitForSeconds(sfxSource.clip.length);
            m_OneShotSoundSources.Remove(sfxSource);
            Destroy(sfxSource.gameObject);
        }
        private IEnumerator RemoveSoundSourceFixedLength(AudioSource sfxSource, float length)
        {
            yield return new WaitForSeconds(length);
            m_OneShotSoundSources.Remove(sfxSource);
            Destroy(sfxSource.gameObject);
        }
        
        public static void PlaySoundWithDelay(AudioClip sfxClip, float volume = 100f, float delay = 0f)
        {
            Instance.StartCoroutine(Instance.PlaySoundWithDelayCoroutine(sfxClip, volume, delay));
        }
        private IEnumerator PlaySoundWithDelayCoroutine(AudioClip sfxClip, float volume = 100f, float delay = 0f)
        {
            yield return new WaitForSeconds(delay);

            PlaySound(sfxClip, volume, AudioPlayMethod.OneShot);
        }
        
        public static void PlaySound(AudioClip sfxClip, float volume = 100f, AudioPlayMethod method = AudioPlayMethod.OneShot)
        {
            if (method == AudioPlayMethod.OneShot)
            {
                AudioSource source = Instance.CreateSoundSource(method);
                source.outputAudioMixerGroup = Instance.audioMixer.FindMatchingGroups("Sound")[0];
                source.mute = GameSettingsData.soundMuted;
                source.volume = volume / 100f;

                source.clip = sfxClip;
                source.Play();

                Instance.StartCoroutine(Instance.RemoveSoundSource(source));
            }
            else if (method == AudioPlayMethod.Loop)
            {
                AudioSource source = Instance.CreateSoundSource(method);
                source.outputAudioMixerGroup = Instance.audioMixer.FindMatchingGroups("Sound")[0];
                source.mute = GameSettingsData.soundMuted;
                source.volume = volume / 100f;

                source.clip = sfxClip;
                source.Play();

                Instance.m_LoopSoundSources.Insert(0, source);
            }
        }
        public static void PlaySoundRandomized(AudioClip clip, float volume)
        {
            AudioSource source = Instance.CreateSoundSource(AudioPlayMethod.OneShot);

            source.mute = GameSettingsData.soundMuted;
            source.pitch = Random.Range(0.85f, 1.2f);
            source.volume = volume / 100f;
            source.clip = clip;

            source.outputAudioMixerGroup = Instance.audioMixer.FindMatchingGroups("Sound")[0];
            source.Play();

            Instance.StartCoroutine(Instance.RemoveSoundSource(source));
        }
        public static void PlaySoundFixedDuration(AudioClip clip, float volume, float duration)
        {
            AudioSource source = Instance.CreateSoundSource(AudioPlayMethod.OneShot);
            source.mute = GameSettingsData.soundMuted;
            source.volume = volume / 100f;
            source.clip = clip;
            source.loop = true;

            source.outputAudioMixerGroup = Instance.audioMixer.FindMatchingGroups("Sound")[0];
            source.Play();

            Instance.StartCoroutine(Instance.RemoveSoundSourceFixedLength(source, duration));
        }
        
        private static void ClearMusicSources()
        {
            for(int i = 0; i < Instance.m_MusicSources.Count; i++)
            {
                if(Instance.m_MusicSources[i] == null)
                {
                    Instance.m_MusicSources.RemoveAt(i);
                    // Only if we're sure that the removed source was before the lastAddedElement
                    lastAddIndex--;
                }
            }
        }
        public static bool IsChannelMuted(AudioVolumeChannel audioType)
        {
            switch (audioType)
            {
                case AudioVolumeChannel.Master:
                    return GameSettingsData.musicMuted && GameSettingsData.soundMuted;
                case AudioVolumeChannel.Music:
                    return GameSettingsData.musicMuted;
                case AudioVolumeChannel.Sound:
                    return GameSettingsData.soundMuted;
            }

            return false;
        }
        /// <summary>
        /// Set and persist master volume
        /// </summary>
        public static void SetVolume(AudioVolumeChannel audioType, float volume, bool save = false)
        {
            if (Instance.audioMixer == null)
                return;

            switch (audioType)
            {
                case AudioVolumeChannel.Master:
                    Instance.audioMixer.SetFloat("MasterVolume", LogarithmicDbTransform(Mathf.Clamp01(volume)));
                    break;

                case AudioVolumeChannel.Music:
                    Instance.audioMixer.SetFloat("MusicVolume", LogarithmicDbTransform(Mathf.Clamp01(volume)));
                    break;

                case AudioVolumeChannel.Sound:
                    Instance.audioMixer.SetFloat("SoundVolume", LogarithmicDbTransform(Mathf.Clamp01(volume)));
                    break;
            }

            if (save)
            {
                switch (audioType)
                {
                    case AudioVolumeChannel.Master:
                        GameSettingsData.masterVolume = volume;
                        break;

                    case AudioVolumeChannel.Music:
                        GameSettingsData.musicVolume = volume;
                        break;

                    case AudioVolumeChannel.Sound:
                        GameSettingsData.soundVolume = volume;
                        break;
                }

                Instance.SaveData();
            }
        }
        /// <summary>
        /// Set and persist game volumes
        /// </summary>
        public static void SetVolumes(float masterVolume, float musicVolume, float soundVolume, bool save = false)
        {
            SetVolume(AudioVolumeChannel.Master, masterVolume, save);
            SetVolume(AudioVolumeChannel.Music, musicVolume, save);
            SetVolume(AudioVolumeChannel.Sound, soundVolume, save);
        }
        /// <summary>
        /// Mute or unmute
        /// </summary>
        public static void MuteVolume(AudioMuteChannel audioType, bool mute, bool save = false)
        {
            if (Instance.audioMixer == null)
                return;

            if (audioType == AudioMuteChannel.Music)
            {
                for(int i = 0; i < Instance.m_MusicSources.Count; i++)
                {
                    Instance.m_MusicSources[i].mute = mute;
                }
            }
            else
            {
                foreach (AudioSource source in Instance.m_OneShotSoundSources)
                    source.mute = mute;

                foreach (AudioSource source in Instance.m_LoopSoundSources)
                    source.mute = mute;
            }

            if (save)
            {
                if (audioType == AudioMuteChannel.Music)
                    GameSettingsData.musicMuted = mute;
                else
                    GameSettingsData.soundMuted = mute;

                Instance.SaveData();
            }
        }
        /// <summary>
        /// Set and persist game volumes
        /// </summary>
        public static void MuteVolumes(bool muteMusic, bool muteSound, bool save = false)
        {
            MuteVolume(AudioMuteChannel.Music, muteMusic, save);
            MuteVolume(AudioMuteChannel.Sound, muteSound, save);
        }
        /// <summary>
        /// Transform volume from linear to logarithmic
        /// </summary>
        protected static float LogarithmicDbTransform(float volume)
        {
            volume = Mathf.Log(89 * volume + 1) / Mathf.Log(90) * 80;
            return volume - 80;
        }
    }
}
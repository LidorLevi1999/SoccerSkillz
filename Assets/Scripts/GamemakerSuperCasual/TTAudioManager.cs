using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GamemakerSuperCasual
{
	public class TTAudioManager : MonoBehaviour
	{
		private List<AudioSource> bgAudioSources;

		private List<AudioSource> sfxAudioSources;

		public GameObject bgAudioHolder;

		public GameObject sfxAudioHolder;

		private AudioSource _currentBgSource;

		public ScriptableSounds soundsData;

		private Dictionary<SoundId, AudioSource> _musicDict;

		public float fadeInTime = 0.5f;

		public float fadeOutTime = 0.5f;

		public float crossFadeTime = 0.5f;

		public bool allowSfx = true;

		public static TTAudioManager Instance;

		private bool _isPaused;

		public bool IsPaused => _isPaused;

		private void Awake()
		{
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			Instance = this;
			_musicDict = new Dictionary<SoundId, AudioSource>();
			bgAudioSources = bgAudioHolder.GetComponentsInChildren<AudioSource>().ToList();
			sfxAudioSources = sfxAudioHolder.GetComponentsInChildren<AudioSource>().ToList();
			AddListeners();
		}

		private void AddListeners()
		{
			UserSettings.SettingsChangedEvent += UserSettings_SettingsChangedEvent;
			PSDKWrapper.PauseGameMusicEvent += PSDKWrapper_PauseGameMusicEvent;
		}

		private void PSDKWrapper_PauseGameMusicEvent(bool pause)
		{
			if (UserSettings.MusicOn)
			{
				if (pause)
				{
					PauseMusic();
				}
				else
				{
					UnPauseMusic();
				}
			}
		}

		private void RemoveListeners()
		{
			PSDKWrapper.PauseGameMusicEvent -= PSDKWrapper_PauseGameMusicEvent;
			UserSettings.SettingsChangedEvent -= UserSettings_SettingsChangedEvent;
		}

		private void UserSettings_SettingsChangedEvent(SettingType type, bool value)
		{
			switch (type)
			{
			case SettingType.Sound:
				if (!value)
				{
					PauseMusic();
					StopAllSFX();
				}
				else if (_isPaused)
				{
					UnPauseMusic();
				}
				break;
			case SettingType.Music:
				if (!value)
				{
					PauseMusic();
				}
				else if (_isPaused)
				{
					UnPauseMusic();
				}
				break;
			case SettingType.Sfx:
				if (!value)
				{
					StopAllSFX();
				}
				break;
			}
		}

		public bool IsPlayingMusic(SoundId id)
		{
			if (_musicDict.TryGetValue(id, out AudioSource value))
			{
				return value.isPlaying;
			}
			return false;
		}

		public bool IsBgMusicPlaying()
		{
			return _currentBgSource != null && _currentBgSource.isPlaying && _currentBgSource.volume > 0f;
		}

		public void PauseMusic()
		{
			_isPaused = true;
			bgAudioSources.ForEach(delegate(AudioSource x)
			{
				x.Pause();
			});
			sfxAudioSources.ForEach(delegate(AudioSource x)
			{
				x.Pause();
			});
		}

		public void StopAllMusic()
		{
			bgAudioSources.ForEach(delegate(AudioSource x)
			{
				x.Stop();
			});
			_musicDict.Clear();
		}

		public void StopAllSounds()
		{
			StopAllMusic();
			StopAllSFX();
		}

		public void StopAllSFX()
		{
			sfxAudioSources.ForEach(delegate(AudioSource x)
			{
				x.Stop();
			});
		}

		public void UnPauseMusic()
		{
			_isPaused = false;
			bgAudioSources.ForEach(delegate(AudioSource x)
			{
				x.UnPause();
			});
		}

		public void DoPlaySFX(SoundId soundId, PlayFlags playMode, float delay = 0f)
		{
			AudioClip sound = GetSound(soundId);
			PlaySFX(sound, playMode, delay);
		}

		public void StopSFX(SoundId soundId)
		{
			AudioClip clip = GetSound(soundId);
			AudioSource audioSource = sfxAudioSources.FirstOrDefault((AudioSource x) => x.clip == clip && x.isPlaying);
			if (audioSource != null)
			{
				audioSource.clip = null;
				audioSource.Stop();
			}
		}

		public void DoPlaySFX(string soundName, PlayFlags playMode = PlayFlags.Normal)
		{
			AudioClip sound = GetSound(soundName);
			PlaySFX(sound, playMode);
		}

		public void DoPlayMusic(SoundId soundId, PlayFlags playMode, float delay = 0f)
		{
			if (_musicDict.TryGetValue(soundId, out AudioSource value) && value.isPlaying)
			{
				UnityEngine.Debug.LogWarning("music with id is already playing: " + soundId);
				return;
			}
			value = PlayMusic(GetSound(soundId), playMode, delay);
			if (value != null && !_musicDict.ContainsKey(soundId))
			{
				_musicDict.Add(soundId, value);
			}
		}

		public void DoPlayMusic(string soundName, PlayFlags playMode, float delay = 0f)
		{
			PlayMusic(GetSound(soundName), playMode, delay);
		}

		public void StopMusic(SoundId id)
		{
			DoStopMusic(id);
		}

		private AudioSource PlayMusic(AudioClip clip, PlayFlags playMode, float delay = 1f)
		{
			if (clip == null)
			{
				return null;
			}
			if (!UserSettings.MusicOn || !UserSettings.SoundOn)
			{
				return null;
			}
			bool loop = playMode.Is(PlayFlags.Loop);
			AudioSource audioSource = bgAudioSources.FirstOrDefault((AudioSource x) => x.isPlaying);
			_currentBgSource = bgAudioSources.FirstOrDefault((AudioSource x) => !x.isPlaying);
			_currentBgSource.clip = clip;
			_currentBgSource.loop = loop;
			if (audioSource != null)
			{
				if (audioSource.clip == clip)
				{
					UnityEngine.Debug.LogError("Trying to play again a bg music clip : " + clip.name);
					return null;
				}
				if (playMode.Is(PlayFlags.Solo))
				{
					if (playMode.Is(PlayFlags.CrossFade))
					{
						CrossFade(audioSource, _currentBgSource, clip, loop);
					}
					else
					{
						StopAllMusic();
						DoStopMusic(audioSource.clip);
						if (playMode.Is(PlayFlags.FadeIn))
						{
							FadeIn(_currentBgSource, fadeInTime);
						}
					}
				}
				else if (playMode.Is(PlayFlags.FadeIn))
				{
					FadeIn(_currentBgSource, fadeInTime);
				}
				else if (delay == 0f)
				{
					_currentBgSource.Play();
				}
				else
				{
					_currentBgSource.PlayDelayed(delay);
				}
			}
			else if (playMode.Is(PlayFlags.FadeIn))
			{
				FadeIn(_currentBgSource, fadeInTime);
			}
			else if (delay == 0f)
			{
				_currentBgSource.Play();
			}
			else
			{
				_currentBgSource.PlayDelayed(delay);
			}
			return _currentBgSource;
		}

		private void CrossFade(AudioSource fromSource, AudioSource toSource, AudioClip clip, bool loop = true, float volume = 1f)
		{
			FadeOut(fromSource, crossFadeTime);
			FadeIn(toSource, crossFadeTime, volume);
		}

		public void FadeOutCurrentPlayingAudio(float time, bool force = false)
		{
			FadeOut(SoundId.none, time, force);
		}

		public void FadeOut(SoundId soundId, float time, bool force = false)
		{
			AudioSource audioSource = null;
			if (soundId != 0)
			{
				AudioClip clip = GetSound(soundId);
				audioSource = bgAudioSources.FirstOrDefault((AudioSource arg) => arg.clip == clip);
				if (!audioSource.isPlaying)
				{
					return;
				}
			}
			else
			{
				audioSource = _currentBgSource;
			}
			FadeOut(audioSource, time, force);
		}

		private void FadeOut(AudioSource source, float time, bool force = false)
		{
			if (!(source == null))
			{
				if (!force)
				{
					source.volume = 0f;
					source.Stop();
				}
				else
				{
					LeanTween.value(source.gameObject, delegate(float f)
					{
						source.volume = f;
					}, source.volume, 0f, time).setEaseInOutSine().setOnComplete((Action)delegate
					{
						source.Stop();
					});
				}
			}
		}

		private void FadeIn(AudioSource source, float time, float volume = 1f)
		{
			if (!(source == null))
			{
				source.volume = 0f;
				source.Play();
				LeanTween.value(source.gameObject, delegate(float f)
				{
					source.volume = f;
				}, 0f, volume, time).setEaseInOutSine();
			}
		}

		private void DoStopMusic(AudioClip clip)
		{
			AudioSource audioSource = bgAudioSources.FirstOrDefault((AudioSource x) => x.clip == clip);
			if (audioSource != null)
			{
				audioSource.Stop();
			}
		}

		private void DoStopMusic(SoundId id)
		{
			if (_musicDict.TryGetValue(id, out AudioSource value))
			{
				value.Stop();
				_musicDict.Remove(id);
				return;
			}
			ScriptableClip clip = soundsData.GetClip(id);
			for (int i = 0; i < clip.Count; i++)
			{
				AudioClip audioClip = clip[i];
				AudioSource audioSource = bgAudioSources.FirstOrDefault((AudioSource a) => a.clip == audioClip);
				if (audioSource != null)
				{
					audioSource.Stop();
				}
			}
		}

		private AudioClip GetSound(string soundName)
		{
			ScriptableClip clip = soundsData.GetClip(soundName);
			if (clip == null)
			{
				UnityEngine.Debug.LogError("no clip found with name: " + soundName);
				return null;
			}
			return clip.GetSound();
		}

		private AudioClip GetSound(SoundId soundId)
		{
			ScriptableClip clip = soundsData.GetClip(soundId);
			if (clip == null)
			{
				UnityEngine.Debug.LogError("GetSound: no clip found with id: " + soundId);
				return null;
			}
			return clip.GetSound();
		}

		public float GetSoundLength(SoundId soundId)
		{
			AudioClip sound = GetSound(soundId);
			if (sound == null)
			{
				return 0f;
			}
			return sound.length;
		}

		private AudioSource PlaySFX(AudioClip clip, PlayFlags playMode, float delay = 0f)
		{
			if (!UserSettings.SfxOn || !UserSettings.SoundOn)
			{
				return null;
			}
			if (!allowSfx)
			{
				return null;
			}
			if (clip == null)
			{
				UnityEngine.Debug.LogError("no clip found");
				return null;
			}
			AudioSource audioSource = sfxAudioSources.FirstOrDefault((AudioSource x) => !x.isPlaying);
			if (audioSource == null)
			{
				AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position, 1f);
			}
			else
			{
				audioSource.loop = playMode.Is(PlayFlags.Loop);
				audioSource.volume = 1f;
				audioSource.clip = clip;
				if (delay > 0f)
				{
					audioSource.PlayDelayed(delay);
				}
				else
				{
					audioSource.Play();
				}
			}
			return audioSource;
		}

		private void OnDestroy()
		{
			StopAllSounds();
		}
	}
}

using UnityEngine;

namespace GamemakerSuperCasual
{
	public static class AudioManagerExtensions
	{
		public static bool Is(this PlayFlags current, PlayFlags value)
		{
			return (current & value) == value;
		}

		public static void PlayMusic(this Object mo, SoundId id, PlayFlags flags = PlayFlags.Normal, float delay = 0f)
		{
			TTAudioManager.Instance.DoPlayMusic(id, flags, delay);
		}

		public static void StopMusic(this Object mo, SoundId id)
		{
			TTAudioManager.Instance.StopMusic(id);
		}

		public static void PlaySFX(this Object mo, SoundId id, PlayFlags flags = PlayFlags.Normal, float delay = 0f)
		{
			TTAudioManager.Instance.DoPlaySFX(id, flags, delay);
		}

		public static float GetSoundLength(this Object mo, SoundId id)
		{
			return TTAudioManager.Instance.GetSoundLength(id);
		}
	}
}

using System;
using System.Collections.Generic;
using UnityEngine;

namespace GamemakerSuperCasual
{
	[Serializable]
	public class ScriptableClip
	{
		public string soundName;

		public SoundId soundId;

		[SerializeField]
		public List<AudioClip> clips;

		public AudioClip this[int i] => clips[i];

		public int Count => (!clips.IsNullOrEmpty()) ? clips.Count : 0;

		public AudioClip RandomElement => clips.RandomElement();

		public ScriptableClip()
		{
		}

		public ScriptableClip(SoundId soundId, AudioClip defaultSound = null)
		{
			this.soundId = soundId;
			soundName = soundId.ToString();
			clips = new List<AudioClip>();
			if (defaultSound != null)
			{
				clips.Add(defaultSound);
			}
		}

		public ScriptableClip(string soundName)
		{
			this.soundName = soundName;
		}

		public AudioClip GetSound()
		{
			if (clips.Count == 0)
			{
				return null;
			}
			return RandomElement;
		}

		public void RefreshSoundName(bool overrideName)
		{
			if (string.IsNullOrEmpty(soundName) || overrideName)
			{
				soundName = soundId.ToString();
			}
		}
	}
}

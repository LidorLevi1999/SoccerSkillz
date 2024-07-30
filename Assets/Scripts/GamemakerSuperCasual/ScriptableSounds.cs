using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GamemakerSuperCasual
{
	[CreateAssetMenu(fileName = "SoundsData", menuName = "Gamemaker Super Casual/Sounds Data")]
	public class ScriptableSounds : ScriptableObject
	{
		[SerializeField]
		public List<ScriptableClip> sounds;

		private Dictionary<SoundId, ScriptableClip> _soundDict;

		public bool copySoundsOnValidate;

		public bool overrideExistingIds;

		public bool createListFromEnum;

		[SerializeField]
		public AudioClip defaultSound;

		private void OnEnable()
		{
			if (sounds != null)
			{
				_soundDict = sounds.ToDictionary((ScriptableClip x) => x.soundId);
			}
		}

		public ScriptableClip GetClip(SoundId id)
		{
			if (_soundDict != null)
			{
				return _soundDict[id];
			}
			return sounds.FirstOrDefault((ScriptableClip x) => x.soundId == id);
		}

		public ScriptableClip GetClip(string name)
		{
			return sounds.FirstOrDefault((ScriptableClip x) => x.soundName == name);
		}

		private void OnValidate()
		{
			if (createListFromEnum)
			{
				List<SoundId> list = Enum.GetValues(typeof(SoundId)).Cast<SoundId>().ToList();
				sounds = new List<ScriptableClip>();
				list.ForEach(delegate(SoundId x)
				{
					ScriptableClip item = new ScriptableClip(x, defaultSound);
					sounds.Add(item);
				});
			}
			if (copySoundsOnValidate)
			{
				sounds.ForEach(delegate(ScriptableClip x)
				{
					x.RefreshSoundName(overrideExistingIds);
				});
			}
		}
	}
}

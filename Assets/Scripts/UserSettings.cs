using System;
using GamemakerSuperCasual;

public static class UserSettings
{
	public const string keyVibrates = "vibrates";

	public const string keyMusicOn = "music";

	public const string keySfxOn = "sfx";

	public const string keySoundOn = "sound";

	public const string keyNotifications = "notifications";

	public static bool Vibrates
	{
		get
		{
			return TTPlayerPrefs.GetBool("vibrates", defaultValue: true);
		}
		set
		{
			if (Vibrates != value)
			{
				TTPlayerPrefs.SetBool("vibrates", value);
			}
		}
	}

	public static bool MusicOn
	{
		get
		{
			return TTPlayerPrefs.GetBool("music", defaultValue: true);
		}
		set
		{
			if (MusicOn != value)
			{
				TTPlayerPrefs.SetBool("music", value);
			}
		}
	}

	public static bool SfxOn
	{
		get
		{
			return TTPlayerPrefs.GetBool("sfx", defaultValue: true);
		}
		set
		{
			if (SfxOn != value)
			{
				TTPlayerPrefs.SetBool("sfx", value);
			}
		}
	}

	public static bool SoundOn
	{
		get
		{
			return TTPlayerPrefs.GetBool("sound", defaultValue: true);
		}
		set
		{
			if (SoundOn != value)
			{
				TTPlayerPrefs.SetBool("sound", value);
			}
		}
	}

	public static bool NotificationsOn
	{
		get
		{
			return TTPlayerPrefs.GetBool("notifications", defaultValue: true);
		}
		set
		{
			if (NotificationsOn != value)
			{
				TTPlayerPrefs.SetBool("notifications", value);
			}
		}
	}

	public static event Action<SettingType, bool> SettingsChangedEvent;

	public static bool GetSettingsValue(SettingType key)
	{
		switch (key)
		{
		case SettingType.Vibrate:
			return Vibrates;
		case SettingType.Music:
			return MusicOn;
		case SettingType.Sfx:
			return SfxOn;
		case SettingType.Sound:
			return SoundOn;
		case SettingType.Notifications:
			return NotificationsOn;
		default:
			return false;
		}
	}

	public static void SetSettingsValue(SettingType key, bool val)
	{
		switch (key)
		{
		case SettingType.Vibrate:
			Vibrates = val;
			break;
		case SettingType.Music:
			MusicOn = val;
			break;
		case SettingType.Sfx:
			SfxOn = val;
			break;
		case SettingType.Sound:
			SoundOn = val;
			break;
		case SettingType.Notifications:
			NotificationsOn = val;
			break;
		}
		UserSettings.SettingsChangedEvent(key, val);
	}

	static UserSettings()
	{
		UserSettings.SettingsChangedEvent = delegate
		{
		};
	}
}

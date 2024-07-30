using System;

namespace GamemakerSuperCasual
{
	[Flags]
	public enum PlayFlags
	{
		Normal = 0x1,
		Loop = 0x2,
		FadeIn = 0x4,
		CrossFade = 0x8,
		Solo = 0x10
	}
}

using System;

namespace GamemakerSuperCasual.Runtime.Extensions
{
	public static class ObjectExtensions
	{
		public static void Raise(this object sender, EventHandler handler, EventArgs args = null)
		{
			handler?.Invoke(sender, args);
		}

		public static void Raise<T>(this object sender, EventHandler<T> handler, T args) where T : EventArgs
		{
			handler?.Invoke(sender, args);
		}
	}
}

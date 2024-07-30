using UnityEngine;

namespace GamemakerSuperCasual.Runtime.Extensions
{
	public static class ComponentExtensions
	{
		public static T AddComponent<T>(this Component component) where T : Component
		{
			return component.gameObject.AddComponent<T>();
		}

		public static T SetActive<T>(this T component, bool value) where T : Component
		{
			component.gameObject.SetActive(value);
			return component;
		}
	}
}

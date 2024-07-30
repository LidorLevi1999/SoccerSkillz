using System;
using System.Collections;
using UnityEngine;

namespace Soccerpass
{
	public class PlayerFormation : MonoBehaviour
	{
		public int GetNumberOfHomePlayers()
		{
			int num = 0;
			IEnumerator enumerator = base.transform.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Transform transform = (Transform)enumerator.Current;
					PlayerSpawnPoint component = transform.GetComponent<PlayerSpawnPoint>();
					if (component != null && component.isHomeTeam)
					{
						num++;
					}
				}
				return num;
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
		}
	}
}

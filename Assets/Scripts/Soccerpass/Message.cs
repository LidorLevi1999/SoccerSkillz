using System;
using System.Collections.Generic;
using UnityEngine;

namespace Soccerpass
{
	[Serializable]
	public class Message
	{
		public FailureType messageType;

		[SerializeField]
		private List<string> strings;

		public string GetMessage => strings.RandomElement();
	}
}

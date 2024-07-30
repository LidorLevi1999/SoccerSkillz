using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Soccerpass
{
	[CreateAssetMenu(fileName = "Messages", menuName = "Data/Messages")]
	public class Messages : ScriptableObject
	{
		public List<Message> messages;

		public string GetMessageByType(FailureType messageType)
		{
			return messages.FirstOrDefault((Message m) => m.messageType == messageType).GetMessage;
		}
	}
}

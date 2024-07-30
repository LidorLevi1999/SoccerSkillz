using System;

namespace RSG
{
	public struct RejectHandler
	{
		public Action<Exception> callback;

		public IRejectable rejectable;
	}
}

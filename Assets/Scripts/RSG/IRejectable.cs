using System;

namespace RSG
{
	public interface IRejectable
	{
		void Reject(Exception ex);
	}
}

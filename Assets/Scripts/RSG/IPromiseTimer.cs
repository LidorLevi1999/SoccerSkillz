using System;

namespace RSG
{
	public interface IPromiseTimer
	{
		IPromise WaitFor(float seconds);

		IPromise WaitUntil(Func<TimeData, bool> predicate);

		IPromise WaitWhile(Func<TimeData, bool> predicate);

		void Update(float deltaTime);
	}
}

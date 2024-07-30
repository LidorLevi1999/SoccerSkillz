using System;
using System.Collections.Generic;

namespace RSG
{
	public class PromiseTimer : IPromiseTimer
	{
		private float curTime;

		private List<PredicateWait> waiting = new List<PredicateWait>();

		public IPromise WaitFor(float seconds)
		{
			return WaitUntil((TimeData t) => t.elapsedTime >= seconds);
		}

		public IPromise WaitWhile(Func<TimeData, bool> predicate)
		{
			return WaitUntil((TimeData t) => !predicate(t));
		}

		public IPromise WaitUntil(Func<TimeData, bool> predicate)
		{
			Promise promise = new Promise();
			PredicateWait predicateWait = new PredicateWait();
			predicateWait.timeStarted = curTime;
			predicateWait.pendingPromise = promise;
			predicateWait.timeData = default(TimeData);
			predicateWait.predicate = predicate;
			PredicateWait item = predicateWait;
			waiting.Add(item);
			return promise;
		}

		public void Update(float deltaTime)
		{
			curTime += deltaTime;
			int num = 0;
			while (num < waiting.Count)
			{
				PredicateWait predicateWait = waiting[num];
				float num2 = curTime - predicateWait.timeStarted;
				predicateWait.timeData.deltaTime = num2 - predicateWait.timeData.elapsedTime;
				predicateWait.timeData.elapsedTime = num2;
				bool flag;
				try
				{
					flag = predicateWait.predicate(predicateWait.timeData);
				}
				catch (Exception ex)
				{
					predicateWait.pendingPromise.Reject(ex);
					waiting.RemoveAt(num);
					continue;
				}
				if (flag)
				{
					predicateWait.pendingPromise.Resolve();
					waiting.RemoveAt(num);
				}
				else
				{
					num++;
				}
			}
		}
	}
}

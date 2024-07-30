using System;
using System.Collections.Generic;

namespace RSG
{
	public interface IPromise<PromisedT>
	{
		IPromise<PromisedT> WithName(string name);

		void Done(Action<PromisedT> onResolved, Action<Exception> onRejected);

		void Done(Action<PromisedT> onResolved);

		void Done();

		IPromise<PromisedT> Catch(Action<Exception> onRejected);

		IPromise<ConvertedT> Then<ConvertedT>(Func<PromisedT, IPromise<ConvertedT>> onResolved);

		IPromise Then(Func<PromisedT, IPromise> onResolved);

		IPromise<PromisedT> Then(Action<PromisedT> onResolved);

		IPromise<ConvertedT> Then<ConvertedT>(Func<PromisedT, IPromise<ConvertedT>> onResolved, Action<Exception> onRejected);

		IPromise Then(Func<PromisedT, IPromise> onResolved, Action<Exception> onRejected);

		IPromise<PromisedT> Then(Action<PromisedT> onResolved, Action<Exception> onRejected);

		IPromise<ConvertedT> Then<ConvertedT>(Func<PromisedT, ConvertedT> transform);

		[Obsolete("Use Then instead")]
		IPromise<ConvertedT> Transform<ConvertedT>(Func<PromisedT, ConvertedT> transform);

		IPromise<IEnumerable<ConvertedT>> ThenAll<ConvertedT>(Func<PromisedT, IEnumerable<IPromise<ConvertedT>>> chain);

		IPromise ThenAll(Func<PromisedT, IEnumerable<IPromise>> chain);

		IPromise<ConvertedT> ThenRace<ConvertedT>(Func<PromisedT, IEnumerable<IPromise<ConvertedT>>> chain);

		IPromise ThenRace(Func<PromisedT, IEnumerable<IPromise>> chain);
	}
	public interface IPromise
	{
		IPromise WithName(string name);

		void Done(Action onResolved, Action<Exception> onRejected);

		void Done(Action onResolved);

		void Done();

		IPromise Catch(Action<Exception> onRejected);

		IPromise<ConvertedT> Then<ConvertedT>(Func<IPromise<ConvertedT>> onResolved);

		IPromise Then(Func<IPromise> onResolved);

		IPromise Then(Action onResolved);

		IPromise<ConvertedT> Then<ConvertedT>(Func<IPromise<ConvertedT>> onResolved, Action<Exception> onRejected);

		IPromise Then(Func<IPromise> onResolved, Action<Exception> onRejected);

		IPromise Then(Action onResolved, Action<Exception> onRejected);

		IPromise ThenAll(Func<IEnumerable<IPromise>> chain);

		IPromise<IEnumerable<ConvertedT>> ThenAll<ConvertedT>(Func<IEnumerable<IPromise<ConvertedT>>> chain);

		IPromise ThenSequence(Func<IEnumerable<Func<IPromise>>> chain);

		IPromise ThenRace(Func<IEnumerable<IPromise>> chain);

		IPromise<ConvertedT> ThenRace<ConvertedT>(Func<IEnumerable<IPromise<ConvertedT>>> chain);
	}
}

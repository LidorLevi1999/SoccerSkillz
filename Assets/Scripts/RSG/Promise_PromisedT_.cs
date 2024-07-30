using RSG.Promises;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RSG
{
	public class Promise<PromisedT> : IPromise<PromisedT>, IPendingPromise<PromisedT>, IPromiseInfo, IRejectable
	{
		private Exception rejectionException;

		private PromisedT resolveValue;

		private List<RejectHandler> rejectHandlers;

		private List<Action<PromisedT>> resolveCallbacks;

		private List<IRejectable> resolveRejectables;

		public int Id
		{
			get;
			private set;
		}

		public string Name
		{
			get;
			private set;
		}

		public PromiseState CurState
		{
			get;
			private set;
		}

		public Promise()
		{
			CurState = PromiseState.Pending;
			Id = ++Promise.nextPromiseId;
			if (Promise.EnablePromiseTracking)
			{
				Promise.pendingPromises.Add(this);
			}
		}

		public Promise(Action<Action<PromisedT>, Action<Exception>> resolver)
		{
			CurState = PromiseState.Pending;
			Id = ++Promise.nextPromiseId;
			if (Promise.EnablePromiseTracking)
			{
				Promise.pendingPromises.Add(this);
			}
			try
			{
				resolver(delegate(PromisedT value)
				{
					Resolve(value);
				}, delegate(Exception ex)
				{
					Reject(ex);
				});
			}
			catch (Exception ex2)
			{
				Reject(ex2);
			}
		}

		private void AddRejectHandler(Action<Exception> onRejected, IRejectable rejectable)
		{
			if (rejectHandlers == null)
			{
				rejectHandlers = new List<RejectHandler>();
			}
			rejectHandlers.Add(new RejectHandler
			{
				callback = onRejected,
				rejectable = rejectable
			});
		}

		private void AddResolveHandler(Action<PromisedT> onResolved, IRejectable rejectable)
		{
			if (resolveCallbacks == null)
			{
				resolveCallbacks = new List<Action<PromisedT>>();
			}
			if (resolveRejectables == null)
			{
				resolveRejectables = new List<IRejectable>();
			}
			resolveCallbacks.Add(onResolved);
			resolveRejectables.Add(rejectable);
		}

		private void InvokeHandler<T>(Action<T> callback, IRejectable rejectable, T value)
		{
			try
			{
				callback(value);
			}
			catch (Exception ex)
			{
				rejectable.Reject(ex);
			}
		}

		private void ClearHandlers()
		{
			rejectHandlers = null;
			resolveCallbacks = null;
			resolveRejectables = null;
		}

		private void InvokeRejectHandlers(Exception ex)
		{
			if (rejectHandlers != null)
			{
				rejectHandlers.Each(delegate(RejectHandler handler)
				{
					InvokeHandler(handler.callback, handler.rejectable, ex);
				});
			}
			ClearHandlers();
		}

		private void InvokeResolveHandlers(PromisedT value)
		{
			if (resolveCallbacks != null)
			{
				int i = 0;
				for (int count = resolveCallbacks.Count; i < count; i++)
				{
					InvokeHandler(resolveCallbacks[i], resolveRejectables[i], value);
				}
			}
			ClearHandlers();
		}

		public void Reject(Exception ex)
		{
			if (CurState != 0)
			{
				throw new ApplicationException("Attempt to reject a promise that is already in state: " + CurState + ", a promise can only be rejected when it is still in state: " + PromiseState.Pending);
			}
			rejectionException = ex;
			CurState = PromiseState.Rejected;
			if (Promise.EnablePromiseTracking)
			{
				Promise.pendingPromises.Remove(this);
			}
			InvokeRejectHandlers(ex);
		}

		public void Resolve(PromisedT value)
		{
			if (CurState != 0)
			{
				throw new ApplicationException("Attempt to resolve a promise that is already in state: " + CurState + ", a promise can only be resolved when it is still in state: " + PromiseState.Pending);
			}
			resolveValue = value;
			CurState = PromiseState.Resolved;
			if (Promise.EnablePromiseTracking)
			{
				Promise.pendingPromises.Remove(this);
			}
			InvokeResolveHandlers(value);
		}

		public void Done(Action<PromisedT> onResolved, Action<Exception> onRejected)
		{
			Then(onResolved, onRejected).Catch(delegate(Exception ex)
			{
				Promise.PropagateUnhandledException(this, ex);
			});
		}

		public void Done(Action<PromisedT> onResolved)
		{
			Then(onResolved).Catch(delegate(Exception ex)
			{
				Promise.PropagateUnhandledException(this, ex);
			});
		}

		public void Done()
		{
			Catch(delegate(Exception ex)
			{
				Promise.PropagateUnhandledException(this, ex);
			});
		}

		public IPromise<PromisedT> WithName(string name)
		{
			Name = name;
			return this;
		}

		public IPromise<PromisedT> Catch(Action<Exception> onRejected)
		{
			Promise<PromisedT> resultPromise = new Promise<PromisedT>();
			resultPromise.WithName(Name);
			Action<PromisedT> resolveHandler = delegate(PromisedT v)
			{
				resultPromise.Resolve(v);
			};
			Action<Exception> rejectHandler = delegate(Exception ex)
			{
				onRejected(ex);
				resultPromise.Reject(ex);
			};
			ActionHandlers(resultPromise, resolveHandler, rejectHandler);
			return resultPromise;
		}

		public IPromise<ConvertedT> Then<ConvertedT>(Func<PromisedT, IPromise<ConvertedT>> onResolved)
		{
			return Then(onResolved, null);
		}

		public IPromise Then(Func<PromisedT, IPromise> onResolved)
		{
			return Then(onResolved, null);
		}

		public IPromise<PromisedT> Then(Action<PromisedT> onResolved)
		{
			return Then(onResolved, null);
		}

		public IPromise<ConvertedT> Then<ConvertedT>(Func<PromisedT, IPromise<ConvertedT>> onResolved, Action<Exception> onRejected)
		{
			Promise<ConvertedT> resultPromise = (Promise<ConvertedT>)new Promise<ConvertedT>();
			((Promise<ConvertedT>)resultPromise).WithName(Name);
			Action<PromisedT> resolveHandler = delegate(PromisedT v)
			{
				onResolved(v).Then(delegate(ConvertedT chainedValue)
				{
					((Promise<ConvertedT>)resultPromise).Resolve(chainedValue);
				}, delegate(Exception ex)
				{
					((Promise<ConvertedT>)resultPromise).Reject(ex);
				});
			};
			Action<Exception> rejectHandler = delegate(Exception ex)
			{
				if (onRejected != null)
				{
					onRejected(ex);
				}
				((Promise<ConvertedT>)resultPromise).Reject(ex);
			};
			ActionHandlers(resultPromise, resolveHandler, rejectHandler);
			return (IPromise<ConvertedT>)resultPromise;
		}

		public IPromise Then(Func<PromisedT, IPromise> onResolved, Action<Exception> onRejected)
		{
			Promise resultPromise = new Promise();
			resultPromise.WithName(Name);
			Action<PromisedT> resolveHandler = delegate(PromisedT v)
			{
				if (onResolved != null)
				{
					onResolved(v).Then(delegate
					{
						resultPromise.Resolve();
					}, delegate(Exception ex)
					{
						resultPromise.Reject(ex);
					});
				}
				else
				{
					resultPromise.Resolve();
				}
			};
			Action<Exception> rejectHandler = delegate(Exception ex)
			{
				if (onRejected != null)
				{
					onRejected(ex);
				}
				resultPromise.Reject(ex);
			};
			ActionHandlers(resultPromise, resolveHandler, rejectHandler);
			return resultPromise;
		}

		public IPromise<PromisedT> Then(Action<PromisedT> onResolved, Action<Exception> onRejected)
		{
			Promise<PromisedT> resultPromise = new Promise<PromisedT>();
			resultPromise.WithName(Name);
			Action<PromisedT> resolveHandler = delegate(PromisedT v)
			{
				if (onResolved != null)
				{
					onResolved(v);
				}
				resultPromise.Resolve(v);
			};
			Action<Exception> rejectHandler = delegate(Exception ex)
			{
				if (onRejected != null)
				{
					onRejected(ex);
				}
				resultPromise.Reject(ex);
			};
			ActionHandlers(resultPromise, resolveHandler, rejectHandler);
			return resultPromise;
		}

		public IPromise<ConvertedT> Then<ConvertedT>(Func<PromisedT, ConvertedT> transform)
		{
			return Then((PromisedT value) => Promise<ConvertedT>.Resolved(transform(value)));
		}

		[Obsolete("Use Then instead")]
		public IPromise<ConvertedT> Transform<ConvertedT>(Func<PromisedT, ConvertedT> transform)
		{
			return Then((PromisedT value) => Promise<ConvertedT>.Resolved(transform(value)));
		}

		private void ActionHandlers(IRejectable resultPromise, Action<PromisedT> resolveHandler, Action<Exception> rejectHandler)
		{
			if (CurState == PromiseState.Resolved)
			{
				InvokeHandler(resolveHandler, resultPromise, resolveValue);
				return;
			}
			if (CurState == PromiseState.Rejected)
			{
				InvokeHandler(rejectHandler, resultPromise, rejectionException);
				return;
			}
			AddResolveHandler(resolveHandler, resultPromise);
			AddRejectHandler(rejectHandler, resultPromise);
		}

		public IPromise<IEnumerable<ConvertedT>> ThenAll<ConvertedT>(Func<PromisedT, IEnumerable<IPromise<ConvertedT>>> chain)
		{
			return Then((PromisedT value) => Promise<ConvertedT>.All(chain(value)));
		}

		public IPromise ThenAll(Func<PromisedT, IEnumerable<IPromise>> chain)
		{
			return Then((PromisedT value) => Promise.All(chain(value)));
		}

		public static IPromise<IEnumerable<PromisedT>> All(params IPromise<PromisedT>[] promises)
		{
			return All((IEnumerable<IPromise<PromisedT>>)promises);
		}

		public static IPromise<IEnumerable<PromisedT>> All(IEnumerable<IPromise<PromisedT>> promises)
		{
			IPromise<PromisedT>[] array = promises.ToArray();
			if (array.Length == 0)
			{
				return Promise<IEnumerable<PromisedT>>.Resolved(EnumerableExt.Empty<PromisedT>());
			}
			int remainingCount = array.Length;
			PromisedT[] results = new PromisedT[remainingCount];
			Promise<IEnumerable<PromisedT>> resultPromise = new Promise<IEnumerable<PromisedT>>();
			resultPromise.WithName("All");
			array.Each(delegate(IPromise<PromisedT> promise, int index)
			{
				promise.Catch(delegate(Exception ex)
				{
					if (resultPromise.CurState == PromiseState.Pending)
					{
						resultPromise.Reject(ex);
					}
				}).Then(delegate(PromisedT result)
				{
					results[index] = result;
					remainingCount--;
					if (remainingCount <= 0)
					{
						resultPromise.Resolve(results);
					}
				}).Done();
			});
			return resultPromise;
		}

		public IPromise<ConvertedT> ThenRace<ConvertedT>(Func<PromisedT, IEnumerable<IPromise<ConvertedT>>> chain)
		{
			return Then((PromisedT value) => Promise<ConvertedT>.Race(chain(value)));
		}

		public IPromise ThenRace(Func<PromisedT, IEnumerable<IPromise>> chain)
		{
			return Then((PromisedT value) => Promise.Race(chain(value)));
		}

		public static IPromise<PromisedT> Race(params IPromise<PromisedT>[] promises)
		{
			return Race((IEnumerable<IPromise<PromisedT>>)promises);
		}

		public static IPromise<PromisedT> Race(IEnumerable<IPromise<PromisedT>> promises)
		{
			IPromise<PromisedT>[] array = promises.ToArray();
			if (array.Length == 0)
			{
				throw new ApplicationException("At least 1 input promise must be provided for Race");
			}
			Promise<PromisedT> resultPromise = new Promise<PromisedT>();
			resultPromise.WithName("Race");
			array.Each(delegate(IPromise<PromisedT> promise, int index)
			{
				promise.Catch(delegate(Exception ex)
				{
					if (resultPromise.CurState == PromiseState.Pending)
					{
						resultPromise.Reject(ex);
					}
				}).Then(delegate(PromisedT result)
				{
					if (resultPromise.CurState == PromiseState.Pending)
					{
						resultPromise.Resolve(result);
					}
				}).Done();
			});
			return resultPromise;
		}

		public static IPromise<PromisedT> Resolved(PromisedT promisedValue)
		{
			Promise<PromisedT> promise = new Promise<PromisedT>();
			promise.Resolve(promisedValue);
			return promise;
		}

		public static IPromise<PromisedT> Rejected(Exception ex)
		{
			Promise<PromisedT> promise = new Promise<PromisedT>();
			promise.Reject(ex);
			return promise;
		}
	}
	public class Promise : IPromise, IPendingPromise, IPromiseInfo, IRejectable
	{
		public struct ResolveHandler
		{
			public Action callback;

			public IRejectable rejectable;
		}

		public static bool EnablePromiseTracking = false;

		private static EventHandler<ExceptionEventArgs> unhandlerException;

		internal static int nextPromiseId = 0;

		internal static HashSet<IPromiseInfo> pendingPromises = new HashSet<IPromiseInfo>();

		private Exception rejectionException;

		private List<RejectHandler> rejectHandlers;

		private List<ResolveHandler> resolveHandlers;

		public int Id
		{
			get;
			private set;
		}

		public string Name
		{
			get;
			private set;
		}

		public PromiseState CurState
		{
			get;
			private set;
		}

		public static event EventHandler<ExceptionEventArgs> UnhandledException;

		public Promise()
		{
			CurState = PromiseState.Pending;
			if (EnablePromiseTracking)
			{
				pendingPromises.Add(this);
			}
		}

		public Promise(Action<Action, Action<Exception>> resolver)
		{
			CurState = PromiseState.Pending;
			if (EnablePromiseTracking)
			{
				pendingPromises.Add(this);
			}
			try
			{
				resolver(delegate
				{
					Resolve();
				}, delegate(Exception ex)
				{
					Reject(ex);
				});
			}
			catch (Exception ex2)
			{
				Reject(ex2);
			}
		}

		public static IEnumerable<IPromiseInfo> GetPendingPromises()
		{
			return pendingPromises;
		}

		private void AddRejectHandler(Action<Exception> onRejected, IRejectable rejectable)
		{
			if (rejectHandlers == null)
			{
				rejectHandlers = new List<RejectHandler>();
			}
			rejectHandlers.Add(new RejectHandler
			{
				callback = onRejected,
				rejectable = rejectable
			});
		}

		private void AddResolveHandler(Action onResolved, IRejectable rejectable)
		{
			if (resolveHandlers == null)
			{
				resolveHandlers = new List<ResolveHandler>();
			}
			resolveHandlers.Add(new ResolveHandler
			{
				callback = onResolved,
				rejectable = rejectable
			});
		}

		private void InvokeRejectHandler(Action<Exception> callback, IRejectable rejectable, Exception value)
		{
			try
			{
				callback(value);
			}
			catch (Exception ex)
			{
				rejectable.Reject(ex);
			}
		}

		private void InvokeResolveHandler(Action callback, IRejectable rejectable)
		{
			try
			{
				callback();
			}
			catch (Exception ex)
			{
				rejectable.Reject(ex);
			}
		}

		private void ClearHandlers()
		{
			rejectHandlers = null;
			resolveHandlers = null;
		}

		private void InvokeRejectHandlers(Exception ex)
		{
			if (rejectHandlers != null)
			{
				rejectHandlers.Each(delegate(RejectHandler handler)
				{
					InvokeRejectHandler(handler.callback, handler.rejectable, ex);
				});
			}
			ClearHandlers();
		}

		private void InvokeResolveHandlers()
		{
			if (resolveHandlers != null)
			{
				resolveHandlers.Each(delegate(ResolveHandler handler)
				{
					InvokeResolveHandler(handler.callback, handler.rejectable);
				});
			}
			ClearHandlers();
		}

		public void Reject(Exception ex)
		{
			if (CurState != 0)
			{
				throw new ApplicationException("Attempt to reject a promise that is already in state: " + CurState + ", a promise can only be rejected when it is still in state: " + PromiseState.Pending);
			}
			rejectionException = ex;
			CurState = PromiseState.Rejected;
			if (EnablePromiseTracking)
			{
				pendingPromises.Remove(this);
			}
			InvokeRejectHandlers(ex);
		}

		public void Resolve()
		{
			if (CurState != 0)
			{
				throw new ApplicationException("Attempt to resolve a promise that is already in state: " + CurState + ", a promise can only be resolved when it is still in state: " + PromiseState.Pending);
			}
			CurState = PromiseState.Resolved;
			if (EnablePromiseTracking)
			{
				pendingPromises.Remove(this);
			}
			InvokeResolveHandlers();
		}

		public void Done(Action onResolved, Action<Exception> onRejected)
		{
			Then(onResolved, onRejected).Catch(delegate(Exception ex)
			{
				PropagateUnhandledException(this, ex);
			});
		}

		public void Done(Action onResolved)
		{
			Then(onResolved).Catch(delegate(Exception ex)
			{
				PropagateUnhandledException(this, ex);
			});
		}

		public void Done()
		{
			Catch(delegate(Exception ex)
			{
				PropagateUnhandledException(this, ex);
			});
		}

		public IPromise WithName(string name)
		{
			Name = name;
			return this;
		}

		public IPromise Catch(Action<Exception> onRejected)
		{
			Promise resultPromise = new Promise();
			resultPromise.WithName(Name);
			Action resolveHandler = delegate
			{
				resultPromise.Resolve();
			};
			Action<Exception> rejectHandler = delegate(Exception ex)
			{
				onRejected(ex);
				resultPromise.Reject(ex);
			};
			ActionHandlers(resultPromise, resolveHandler, rejectHandler);
			return resultPromise;
		}

		public IPromise<ConvertedT> Then<ConvertedT>(Func<IPromise<ConvertedT>> onResolved)
		{
			return Then(onResolved, null);
		}

		public IPromise Then(Func<IPromise> onResolved)
		{
			return Then(onResolved, null);
		}

		public IPromise Then(Action onResolved)
		{
			return Then(onResolved, null);
		}

		public IPromise<ConvertedT> Then<ConvertedT>(Func<IPromise<ConvertedT>> onResolved, Action<Exception> onRejected)
		{
			Promise<ConvertedT> resultPromise = (Promise<ConvertedT>)new Promise<ConvertedT>();
			((Promise<ConvertedT>)resultPromise).WithName(Name);
			Action resolveHandler = delegate
			{
				onResolved().Then(delegate(ConvertedT chainedValue)
				{
					((Promise<ConvertedT>)resultPromise).Resolve(chainedValue);
				}, delegate(Exception ex)
				{
					((Promise<ConvertedT>)resultPromise).Reject(ex);
				});
			};
			Action<Exception> rejectHandler = delegate(Exception ex)
			{
				if (onRejected != null)
				{
					onRejected(ex);
				}
				((Promise<ConvertedT>)resultPromise).Reject(ex);
			};
			ActionHandlers(resultPromise, resolveHandler, rejectHandler);
			return (IPromise<ConvertedT>)resultPromise;
		}

		public IPromise Then(Func<IPromise> onResolved, Action<Exception> onRejected)
		{
			Promise resultPromise = new Promise();
			resultPromise.WithName(Name);
			Action resolveHandler = delegate
			{
				if (onResolved != null)
				{
					onResolved().Then(delegate
					{
						resultPromise.Resolve();
					}, delegate(Exception ex)
					{
						resultPromise.Reject(ex);
					});
				}
				else
				{
					resultPromise.Resolve();
				}
			};
			Action<Exception> rejectHandler = delegate(Exception ex)
			{
				if (onRejected != null)
				{
					onRejected(ex);
				}
				resultPromise.Reject(ex);
			};
			ActionHandlers(resultPromise, resolveHandler, rejectHandler);
			return resultPromise;
		}

		public IPromise Then(Action onResolved, Action<Exception> onRejected)
		{
			Promise resultPromise = new Promise();
			resultPromise.WithName(Name);
			Action resolveHandler = delegate
			{
				if (onResolved != null)
				{
					onResolved();
				}
				resultPromise.Resolve();
			};
			Action<Exception> rejectHandler = delegate(Exception ex)
			{
				if (onRejected != null)
				{
					onRejected(ex);
				}
				resultPromise.Reject(ex);
			};
			ActionHandlers(resultPromise, resolveHandler, rejectHandler);
			return resultPromise;
		}

		private void ActionHandlers(IRejectable resultPromise, Action resolveHandler, Action<Exception> rejectHandler)
		{
			if (CurState == PromiseState.Resolved)
			{
				InvokeResolveHandler(resolveHandler, resultPromise);
				return;
			}
			if (CurState == PromiseState.Rejected)
			{
				InvokeRejectHandler(rejectHandler, resultPromise, rejectionException);
				return;
			}
			AddResolveHandler(resolveHandler, resultPromise);
			AddRejectHandler(rejectHandler, resultPromise);
		}

		public IPromise ThenAll(Func<IEnumerable<IPromise>> chain)
		{
			return Then(() => All(chain()));
		}

		public IPromise<IEnumerable<ConvertedT>> ThenAll<ConvertedT>(Func<IEnumerable<IPromise<ConvertedT>>> chain)
		{
			return Then(() => Promise<ConvertedT>.All(chain()));
		}

		public static IPromise All(params IPromise[] promises)
		{
			return All((IEnumerable<IPromise>)promises);
		}

		public static IPromise All(IEnumerable<IPromise> promises)
		{
			IPromise[] array = promises.ToArray();
			if (array.Length == 0)
			{
				return Resolved();
			}
			int remainingCount = array.Length;
			Promise resultPromise = new Promise();
			resultPromise.WithName("All");
			array.Each(delegate(IPromise promise, int index)
			{
				promise.Catch(delegate(Exception ex)
				{
					if (resultPromise.CurState == PromiseState.Pending)
					{
						resultPromise.Reject(ex);
					}
				}).Then(delegate
				{
					remainingCount--;
					if (remainingCount <= 0)
					{
						resultPromise.Resolve();
					}
				}).Done();
			});
			return resultPromise;
		}

		public IPromise ThenSequence(Func<IEnumerable<Func<IPromise>>> chain)
		{
			return Then(() => Sequence(chain()));
		}

		public static IPromise Sequence(params Func<IPromise>[] fns)
		{
			return Sequence((IEnumerable<Func<IPromise>>)fns);
		}

		public static IPromise Sequence(IEnumerable<Func<IPromise>> fns)
		{
			return fns.Aggregate(Resolved(), (IPromise prevPromise, Func<IPromise> fn) => prevPromise.Then(() => fn()));
		}

		public IPromise ThenRace(Func<IEnumerable<IPromise>> chain)
		{
			return Then(() => Race(chain()));
		}

		public IPromise<ConvertedT> ThenRace<ConvertedT>(Func<IEnumerable<IPromise<ConvertedT>>> chain)
		{
			return Then(() => Promise<ConvertedT>.Race(chain()));
		}

		public static IPromise Race(params IPromise[] promises)
		{
			return Race((IEnumerable<IPromise>)promises);
		}

		public static IPromise Race(IEnumerable<IPromise> promises)
		{
			IPromise[] array = promises.ToArray();
			if (array.Length == 0)
			{
				throw new ApplicationException("At least 1 input promise must be provided for Race");
			}
			Promise resultPromise = new Promise();
			resultPromise.WithName("Race");
			array.Each(delegate(IPromise promise, int index)
			{
				promise.Catch(delegate(Exception ex)
				{
					if (resultPromise.CurState == PromiseState.Pending)
					{
						resultPromise.Reject(ex);
					}
				}).Then(delegate
				{
					if (resultPromise.CurState == PromiseState.Pending)
					{
						resultPromise.Resolve();
					}
				}).Done();
			});
			return resultPromise;
		}

		public static IPromise Resolved()
		{
			Promise promise = new Promise();
			promise.Resolve();
			return promise;
		}

		public static IPromise Rejected(Exception ex)
		{
			Promise promise = new Promise();
			promise.Reject(ex);
			return promise;
		}

		internal static void PropagateUnhandledException(object sender, Exception ex)
		{
			if (unhandlerException != null)
			{
				unhandlerException(sender, new ExceptionEventArgs(ex));
			}
		}
	}
}

namespace RSG
{
	public interface IPendingPromise<PromisedT> : IRejectable
	{
		void Resolve(PromisedT value);
	}
	public interface IPendingPromise : IRejectable
	{
		void Resolve();
	}
}

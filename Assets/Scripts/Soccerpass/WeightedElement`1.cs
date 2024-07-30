namespace Soccerpass
{
	public abstract class WeightedElement<T>
	{
		public T value;

		public int weight;

		public abstract bool IsValid();
	}
}

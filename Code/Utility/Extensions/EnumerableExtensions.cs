namespace Minigolf;

public static class EnumerableExtensions
{
	public static int HashCombine<T>( this IEnumerable<T> e, Func<T, decimal> selector )
	{
		var result = 0;

		foreach ( var el in e )
			result = HashCode.Combine( result, selector.Invoke( el ) );

		return result;
	}

	public static int IndexOf<T>( this IEnumerable<T> self, T elementToFind )
	{
		int i = 0;
		foreach ( T element in self )
		{
			if ( Equals( element, elementToFind ) )
				return i;
			i += 1;
		}
		return -1;
	}
}

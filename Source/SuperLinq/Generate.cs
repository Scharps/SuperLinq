﻿namespace SuperLinq;

public static partial class SuperEnumerable
{
	/// <summary>
	///	    Returns a sequence of values consecutively generated by a generator function.
	/// </summary>
	/// <typeparam name="TResult">
	///	    Type of elements to generate.
	/// </typeparam>
	/// <param name="initial">
	///	    Value of first element in sequence
	/// </param>
	/// <param name="generator">
	///	    Generator function which takes the previous series element and uses it to generate the next element.
	/// </param>
	/// <returns>
	///	    A sequence containing the generated values.
	/// </returns>
	/// <exception cref="ArgumentNullException">
	///	    <paramref name="generator"/> is <see langword="null"/>.
	/// </exception>
	/// <remarks>
	///	    This function defers element generation until needed and streams the results. It is treated as an
	///	    infinite sequence, and will not terminate.
	/// </remarks>
	public static IEnumerable<TResult> Generate<TResult>(TResult initial, Func<TResult, TResult> generator)
	{
		Guard.IsNotNull(generator);

		return Core(initial, generator);

		static IEnumerable<TResult> Core(TResult current, Func<TResult, TResult> generator)
		{
			while (true)
			{
				yield return current;
				current = generator(current);
			}
		}
	}
}

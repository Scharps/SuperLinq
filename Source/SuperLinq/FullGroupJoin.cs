﻿namespace SuperLinq;

// Inspiration & credit: http://stackoverflow.com/a/13503860/6682
public static partial class SuperEnumerable
{
	/// <summary>
	///	    Performs a Full Group Join between the <paramref name="first"/> and <paramref name="second"/> sequences.
	/// </summary>
	/// <typeparam name="TFirst">
	///	    The type of the elements in the first input sequence
	/// </typeparam>
	/// <typeparam name="TSecond">
	///	    The type of the elements in the second input sequence
	/// </typeparam>
	/// <typeparam name="TKey">
	///	    The type of the key to use to join
	/// </typeparam>
	/// <param name="first">
	///	    First sequence
	/// </param>
	/// <param name="second">
	///	    Second sequence
	/// </param>
	/// <param name="firstKeySelector">
	///	    The mapping from first sequence to key
	/// </param>
	/// <param name="secondKeySelector">
	///	    The mapping from second sequence to key
	/// </param>
	/// <returns>
	///	    A sequence of elements joined from <paramref name="first"/> and <paramref name="second"/>.
	/// </returns>
	/// <exception cref="ArgumentNullException">
	///	    <paramref name="first"/>, <paramref name="second"/>, <paramref name="firstKeySelector"/>, or <paramref
	///     name="secondKeySelector"/> is <see langword="null"/>.
	/// </exception>
	/// <remarks>
	///	    This operator uses deferred execution and streams the results. The results are yielded in the order of the
	///     elements found in the first sequence followed by those found only in the second. In addition, the callback
	///     responsible for projecting the results is supplied with sequences which preserve their source order.
	/// </remarks>
	public static IEnumerable<(TKey Key, IEnumerable<TFirst> First, IEnumerable<TSecond> Second)> FullGroupJoin<TFirst, TSecond, TKey>(
		this IEnumerable<TFirst> first,
		IEnumerable<TSecond> second,
		Func<TFirst, TKey> firstKeySelector,
		Func<TSecond, TKey> secondKeySelector)
	{
		return FullGroupJoin(first, second, firstKeySelector, secondKeySelector, ValueTuple.Create, EqualityComparer<TKey>.Default);
	}

	/// <summary>
	///	    Performs a Full Group Join between the <paramref name="first"/> and <paramref name="second"/> sequences.
	/// </summary>
	/// <typeparam name="TFirst">
	///	    The type of the elements in the first input sequence
	/// </typeparam>
	/// <typeparam name="TSecond">
	///	    The type of the elements in the second input sequence
	/// </typeparam>
	/// <typeparam name="TKey">
	///	    The type of the key to use to join
	/// </typeparam>
	/// <param name="first">
	///	    First sequence
	/// </param>
	/// <param name="second">
	///	    Second sequence
	/// </param>
	/// <param name="firstKeySelector">
	///	    The mapping from first sequence to key
	/// </param>
	/// <param name="secondKeySelector">
	///	    The mapping from second sequence to key
	/// </param>
	/// <param name="comparer">
	///	    The equality comparer to use to determine whether or not keys are equal. If <see langword="null"/>, the
	///     default equality comparer for <typeparamref name="TKey"/> is used.
	/// </param>
	/// <returns>
	///	    A sequence of elements joined from <paramref name="first"/> and <paramref name="second"/>.
	/// </returns>
	/// <exception cref="ArgumentNullException">
	///	    <paramref name="first"/>, <paramref name="second"/>, <paramref name="firstKeySelector"/>, or <paramref
	///     name="secondKeySelector"/> is <see langword="null"/>.
	/// </exception>
	/// <remarks>
	///	    This operator uses deferred execution and streams the results. The results are yielded in the order of the
	///     elements found in the first sequence followed by those found only in the second. In addition, the callback
	///     responsible for projecting the results is supplied with sequences which preserve their source order.
	/// </remarks>
	public static IEnumerable<(TKey Key, IEnumerable<TFirst> First, IEnumerable<TSecond> Second)> FullGroupJoin<TFirst, TSecond, TKey>(
		this IEnumerable<TFirst> first,
		IEnumerable<TSecond> second,
		Func<TFirst, TKey> firstKeySelector,
		Func<TSecond, TKey> secondKeySelector,
		IEqualityComparer<TKey>? comparer)
	{
		return FullGroupJoin(first, second, firstKeySelector, secondKeySelector, ValueTuple.Create, comparer);
	}

	/// <summary>
	///	    Performs a full group-join between two sequences.
	/// </summary>
	/// <typeparam name="TFirst">
	///	    The type of the elements in the first input sequence
	/// </typeparam>
	/// <typeparam name="TSecond">
	///	    The type of the elements in the second input sequence
	/// </typeparam>
	/// <typeparam name="TKey">
	///	    The type of the key to use to join
	/// </typeparam>
	/// <typeparam name="TResult">
	///	    The type of the elements of the resulting sequence
	/// </typeparam>
	/// <param name="first">
	///	    First sequence
	/// </param>
	/// <param name="second">
	///	    Second sequence
	/// </param>
	/// <param name="firstKeySelector">
	///	    The mapping from first sequence to key
	/// </param>
	/// <param name="secondKeySelector">
	///	    The mapping from second sequence to key
	/// </param>
	/// <param name="resultSelector">
	///	    Function to apply to each pair of elements plus the key
	/// </param>
	/// <returns>
	///	    A sequence of elements joined from <paramref name="first"/> and <paramref name="second"/>.
	/// </returns>
	/// <exception cref="ArgumentNullException">
	///	    <paramref name="first"/>, <paramref name="second"/>, <paramref name="firstKeySelector"/>, <paramref
	///     name="secondKeySelector"/>, or <paramref name="resultSelector"/> is <see langword="null"/>.
	/// </exception>
	/// <remarks>
	///	    This operator uses deferred execution and streams the results. The results are yielded in the order of the
	///     elements found in the first sequence followed by those found only in the second. In addition, the callback
	///     responsible for projecting the results is supplied with sequences which preserve their source order.
	/// </remarks>
	public static IEnumerable<TResult> FullGroupJoin<TFirst, TSecond, TKey, TResult>(
		this IEnumerable<TFirst> first,
		IEnumerable<TSecond> second,
		Func<TFirst, TKey> firstKeySelector,
		Func<TSecond, TKey> secondKeySelector,
		Func<TKey, IEnumerable<TFirst>, IEnumerable<TSecond>, TResult> resultSelector)
	{
		return FullGroupJoin(first, second, firstKeySelector, secondKeySelector, resultSelector, EqualityComparer<TKey>.Default);
	}

	/// <summary>
	///	    Performs a full group-join between two sequences.
	/// </summary>
	/// <typeparam name="TFirst">
	///	    The type of the elements in the first input sequence
	/// </typeparam>
	/// <typeparam name="TSecond">
	///	    The type of the elements in the second input sequence
	/// </typeparam>
	/// <typeparam name="TKey">
	///	    The type of the key to use to join
	/// </typeparam>
	/// <typeparam name="TResult">
	///	    The type of the elements of the resulting sequence
	/// </typeparam>
	/// <param name="first">
	///	    First sequence
	/// </param>
	/// <param name="second">
	///	    Second sequence
	/// </param>
	/// <param name="firstKeySelector">
	///	    The mapping from first sequence to key
	/// </param>
	/// <param name="secondKeySelector">
	///	    The mapping from second sequence to key
	/// </param>
	/// <param name="resultSelector">
	///	    Function to apply to each pair of elements plus the key
	/// </param>
	/// <param name="comparer">
	///	    The equality comparer to use to determine whether or not keys are equal. If <see langword="null"/>, the
	///     default equality comparer for <typeparamref name="TKey"/> is used.
	/// </param>
	/// <returns>
	///	    A sequence of elements joined from <paramref name="first"/> and <paramref name="second"/>.
	/// </returns>
	/// <exception cref="ArgumentNullException">
	///	    <paramref name="first"/>, <paramref name="second"/>, <paramref name="firstKeySelector"/>, <paramref
	///     name="secondKeySelector"/>, or <paramref name="resultSelector"/> is <see langword="null"/>.
	/// </exception>
	/// <remarks>
	///	    This operator uses deferred execution and streams the results. The results are yielded in the order of the
	///     elements found in the first sequence followed by those found only in the second. In addition, the callback
	///     responsible for projecting the results is supplied with sequences which preserve their source order.
	/// </remarks>
	public static IEnumerable<TResult> FullGroupJoin<TFirst, TSecond, TKey, TResult>(
		this IEnumerable<TFirst> first,
		IEnumerable<TSecond> second,
		Func<TFirst, TKey> firstKeySelector,
		Func<TSecond, TKey> secondKeySelector,
		Func<TKey, IEnumerable<TFirst>, IEnumerable<TSecond>, TResult> resultSelector,
		IEqualityComparer<TKey>? comparer)
	{
		ArgumentNullException.ThrowIfNull(first);
		ArgumentNullException.ThrowIfNull(second);
		ArgumentNullException.ThrowIfNull(firstKeySelector);
		ArgumentNullException.ThrowIfNull(secondKeySelector);
		ArgumentNullException.ThrowIfNull(resultSelector);

		return Core(comparer ?? EqualityComparer<TKey>.Default);

		IEnumerable<TResult> Core(IEqualityComparer<TKey> comparer)
		{
			var alookup = Lookup<TKey, TFirst>.CreateForJoin(first, firstKeySelector, comparer);
			var blookup = Lookup<TKey, TSecond>.CreateForJoin(second, secondKeySelector, comparer);

			foreach (var a in alookup)
				yield return resultSelector(a.Key, a, blookup[a.Key]);

			foreach (var b in blookup)
			{
				if (alookup.Contains(b.Key))
					continue;
				// We can skip the lookup because we are iterating over keys not found in the first sequence
				yield return resultSelector(b.Key, Enumerable.Empty<TFirst>(), b);
			}
		}
	}
}

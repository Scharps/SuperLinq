﻿namespace SuperLinq;

public static partial class SuperEnumerable
{
	/// <summary>
	///	    Returns a sequence with a range of elements in the source sequence moved to a new offset.
	/// </summary>
	/// <typeparam name="T">
	///	    Type of the source sequence.
	/// </typeparam>
	/// <param name="source">
	///	    The source sequence.
	/// </param>
	/// <param name="fromIndex">
	///	    The zero-based index identifying the first element in the range of elements to move.
	/// </param>
	/// <param name="count">
	///	    The count of items to move.
	/// </param>
	/// <param name="toIndex">
	///	    The index where the specified range will be moved.
	/// </param>
	/// <returns>
	///	    A sequence with the specified range moved to the new position.
	/// </returns>
	/// <exception cref="ArgumentNullException">
	///	    <paramref name="source"/> is <see langword="null"/>.
	/// </exception>
	/// <exception cref="ArgumentOutOfRangeException">
	///	    <paramref name="fromIndex"/>, <paramref name="count"/>, or <paramref name="toIndex"/> is less than <c>0</c>.
	/// </exception>
	/// <remarks>
	///	    This operator uses deferred execution and streams its results.
	/// </remarks>
	public static IEnumerable<T> Move<T>(
		this IEnumerable<T> source,
		int fromIndex,
		int count,
		int toIndex
	)
	{
		ArgumentNullException.ThrowIfNull(source);
		ArgumentOutOfRangeException.ThrowIfNegative(fromIndex);
		ArgumentOutOfRangeException.ThrowIfNegative(count);
		ArgumentOutOfRangeException.ThrowIfNegative(toIndex);

		return toIndex == fromIndex || count == 0
			? source
			: toIndex < fromIndex
				? Core(source, toIndex, fromIndex - toIndex, count)
				: Core(source, fromIndex, count, toIndex - fromIndex);

		static IEnumerable<T> Core(
			IEnumerable<T> source,
			int bufferStartIndex,
			int bufferSize,
			int bufferYieldIndex
		)
		{
			var hasMore = true;
			bool MoveNext(IEnumerator<T> e) => hasMore && (hasMore = e.MoveNext());

			using var e = source.GetEnumerator();

			for (var i = 0; i < bufferStartIndex && MoveNext(e); i++)
				yield return e.Current;

			var buffer = new T[bufferSize];
			var length = 0;

			for (; length < bufferSize && MoveNext(e); length++)
				buffer[length] = e.Current;

			for (var i = 0; i < bufferYieldIndex && MoveNext(e); i++)
				yield return e.Current;

			for (var i = 0; i < length; i++)
				yield return buffer[i];

			while (MoveNext(e))
				yield return e.Current;
		}
	}

	/// <summary>
	/// 	Returns a sequence with a range of elements in the source sequence moved to a new offset.
	/// </summary>
	/// <typeparam name="T">
	/// 	Type of the source sequence.
	/// </typeparam>
	/// <param name="source">
	/// 	The source sequence.
	/// </param>
	/// <param name="range">
	/// 	The range of values to move.
	/// </param>
	/// <param name="to">
	/// 	The index where the specified range will be moved.</param>
	/// <returns>
	/// 	A sequence with the specified range moved to the new position.
	/// </returns>
	/// <exception cref="ArgumentNullException">
	///	    <paramref name="source"/> is <see langword="null"/>.
	/// </exception>
	/// <exception cref="ArgumentOutOfRangeException">
	///	    <paramref name="range"/>'s start is less than <c>0</c> or <paramref name="range"/>'s end is before start in the sequence.
	/// </exception>
	/// <remarks>
	/// 	This operator uses deferred executing and streams its results.
	/// </remarks>
	public static IEnumerable<T> Move<T>(this IEnumerable<T> source, Range range, Index to)
	{
		if (!range.Start.IsFromEnd && !range.End.IsFromEnd && !to.IsFromEnd)
		{
			foreach (
				var e in Move(
					source,
					range.Start.Value,
					range.End.Value - range.Start.Value,
					to.Value
				)
			)
			{
				yield return e;
			}
		}
		else
		{
			int startIndex;
			int endIndex;
			int toIndex;

			if (source.TryGetCollectionCount() is int count)
			{
				startIndex = range.Start.GetOffset(count);
				endIndex = range.End.GetOffset(count);
				toIndex = to.GetOffset(count);
				yield return (T)Move(source, startIndex, endIndex - startIndex, toIndex);
			}
			else
			{
				switch ((range.Start.IsFromEnd, range.End.IsFromEnd, to.IsFromEnd))
				{
					case (false, false, true):
						using (var e = source.GetEnumerator())
						{
							if (!e.MoveNext())
							{
								yield break;
							}

							var bufferCap = to.Value;
							var moveCap = range.End.Value - range.Start.Value;
							var buffer = new Queue<T>(bufferCap);
							var move = new Queue<T>(moveCap);

							buffer.Enqueue(e.Current);
							count = 1;

							while (e.MoveNext())
							{
								buffer.Enqueue(e.Current);
								checked
								{
									++count;
								}
								if (count > to.Value)
								{
									var idx = count - bufferCap;
									if (idx > range.Start.Value && idx <= range.End.Value)
									{
										move.Enqueue(buffer.Dequeue());
									}
									else
									{
										yield return buffer.Dequeue();
									}
								}
							}
							while (move.TryDequeue(out var element))
							{
								yield return element;
							}
							while (buffer.TryDequeue(out var element))
							{
								yield return element;
							}
						}
						yield break;
					case (false, true, false):
						// [4, 5, 2, 4, 1, §, 5] Move(1..^4, 2)
						break;
					case (false, true, true):
						// [4, 5, 2, 4, 1, §, 5] Move(1..^4, ^2)
						break;
					case (true, false, false):
						// [4, 5, 2, 4, 1, §, 5] Move(^5..4, 2)
						break;
					case (true, false, true):
						// [4, 5, 2, 4, 1, §, 5] Move(^5..4, ^2)
						break;
					case (true, true, false):
						if (range.End.Value > range.Start.Value)
						{
							yield break;
						}
						// [4, 5, 2, 4, 1, §, 5] Move(^5..^2, 4)
						// Cannot yield any elements until count is known.
						// Once count is known, can proceed to yield elements
						break;
					case (true, true, true):
						// [4, 5, 2, 4, 1, §, 5] Move(^5..^3, ^2)
						break;
				}
			}
		}
	}
}

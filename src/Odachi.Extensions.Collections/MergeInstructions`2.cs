using System;
using System.Collections.Generic;

namespace Odachi.Extensions.Collections;

/// <summary>
/// Instructions for adapting collection of <typeparamref name="TCurrent"/> using reference collection of <typeparamref name="TNew"/>.
/// </summary>
public class MergeInstructions<TCurrent, TNew>
{
	public MergeInstructions(IEnumerable<TCurrent> currentCollection, IEnumerable<TNew> newCollection, IReadOnlyList<TNew> inserts, IReadOnlyList<KeyValuePair<TCurrent, TNew>> updates, IReadOnlyList<TCurrent> deletes)
	{
		CurrentCollection = currentCollection ?? throw new ArgumentNullException(nameof(currentCollection));
		NewCollection = newCollection ?? throw new ArgumentNullException(nameof(newCollection));

		Inserts = inserts ?? throw new ArgumentNullException(nameof(inserts));
		Updates = updates ?? throw new ArgumentNullException(nameof(updates));
		Deletes = deletes ?? throw new ArgumentNullException(nameof(deletes));
	}

	public IEnumerable<TCurrent> CurrentCollection { get; }
	public IEnumerable<TNew> NewCollection { get; }

	public IReadOnlyList<TNew> Inserts { get; }
	public IReadOnlyList<KeyValuePair<TCurrent, TNew>> Updates { get; }
	public IReadOnlyList<TCurrent> Deletes { get; }
}

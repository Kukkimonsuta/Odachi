using System;
using System.Collections.Generic;
using System.Linq;

namespace Odachi.Extensions.Collections;

public static class MergeExtensions
{
	/// <summary>
	/// Prepare instructions for adapting <paramref name="currentCollection"/> using <paramref name="newCollection"/>.
	/// </summary>
	public static MergeInstructions<TCurrent, TNew> PrepareMerge<TCurrent, TNew>(this IEnumerable<TCurrent> currentCollection, IEnumerable<TNew> newCollection, Func<TCurrent, TNew, bool> equalityComparer)
	{
		if (currentCollection == null)
			throw new ArgumentNullException(nameof(currentCollection));
		if (newCollection == null)
			throw new ArgumentNullException(nameof(newCollection));

		var inserts = new List<TNew>();
		var updates = new List<KeyValuePair<TCurrent, TNew>>();
		var deletes = new List<TCurrent>();

		// look for updated/deleted items
		foreach (var currentItem in currentCollection)
		{
			var newItem = newCollection.SingleOrDefault(x => equalityComparer(currentItem, x));
			if (newItem == null)
			{
				deletes.Add(currentItem);
			}
			else
			{
				updates.Add(new KeyValuePair<TCurrent, TNew>(currentItem, newItem));
			}
		}

		// look for inserted items
		foreach (var newItem in newCollection)
		{
			var currentItem = currentCollection.SingleOrDefault(x => equalityComparer(x, newItem));
			if (currentItem == null)
			{
				inserts.Add(newItem);
			}
		}

		return new MergeInstructions<TCurrent, TNew>(currentCollection, newCollection, inserts, updates, deletes);
	}

	/// <summary>
	/// Apply merge instructions.
	/// </summary>
	public static void Apply<TCurrent, TNew>(this MergeInstructions<TCurrent, TNew> instructions, Action<TNew> insert, Action<TCurrent, TNew> update, Action<TCurrent> delete)
	{
		foreach (var insertEntry in instructions.Inserts)
		{
			insert(insertEntry);
		}

		foreach (var updateEntry in instructions.Updates)
		{
			update(updateEntry.Key, updateEntry.Value);
		}

		foreach (var deleteEntry in instructions.Deletes)
		{
			delete(deleteEntry);
		}
	}

	/// <summary>
	/// Apply merge instructions.
	/// </summary>
	public static void Apply<TCurrent, TNew>(
		this MergeInstructions<TCurrent, TNew> instructions,
		Action<MergeInstructions<TCurrent, TNew>, TNew> insert,
		Action<MergeInstructions<TCurrent, TNew>, TCurrent, TNew> update,
		Action<MergeInstructions<TCurrent, TNew>, TCurrent> delete
	)
	{
		foreach (var insertEntry in instructions.Inserts)
		{
			insert(instructions, insertEntry);
		}

		foreach (var updateEntry in instructions.Updates)
		{
			update(instructions, updateEntry.Key, updateEntry.Value);
		}

		foreach (var deleteEntry in instructions.Deletes)
		{
			delete(instructions, deleteEntry);
		}
	}

	/// <summary>
	/// Apply merge instructions to current collection.
	/// </summary>
	public static void ApplyToCurrent<TCurrent, TNew>(this MergeInstructions<TCurrent, TNew> instructions, Func<TNew, TCurrent> insert, Action<TCurrent, TNew> update)
	{
		if (instructions.CurrentCollection is ICollection<TCurrent> currentCollection)
		{
			Apply(
				instructions,
				insertEntry =>
				{
					var record = insert(insertEntry);
					currentCollection.Add(record);
				},
				update,
				deleteEntry =>
				{
					currentCollection.Remove(deleteEntry);
				}
			);
		}
		else
		{
			throw new InvalidOperationException($"Cannot apply to current collection of type '{instructions.CurrentCollection?.GetType().FullName}'");
		}
	}
}

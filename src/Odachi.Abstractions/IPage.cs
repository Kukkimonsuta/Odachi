using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odachi.Abstractions;

/// <summary>
/// Holds information about page of a collection.
/// </summary>
public interface IPage : IEnumerable
{
	/// <summary>
	/// Number of page that is represented by this instance.
	/// </summary>
	int Number { get; }

	/// <summary>
	/// Maximum size of a page.
	/// </summary>
	int Size { get; }

	/// <summary>
	/// Total number of discovered items.
	/// </summary>
	int? Total { get; }

	/// <summary>
	/// Notes whether there are more items after `Total` was reached.
	/// </summary>
	bool Overflow { get; }
}

/// <summary>
/// Holds information about page of a collection.
/// </summary>
public interface IPage<out T> : IPage, IEnumerable<T>
{
}
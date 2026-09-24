using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Odachi.Testing.Scenarios.XunitV3.Generators.Internal;

/// <summary>
/// Array with value equality, so that it can take part in a model cached by the incremental
/// generator pipeline.
/// </summary>
internal readonly struct EquatableArray<T> : IEquatable<EquatableArray<T>>, IEnumerable<T>
    where T : IEquatable<T>
{
    public static readonly EquatableArray<T> Empty = new EquatableArray<T>(ImmutableArray<T>.Empty);

    public EquatableArray(ImmutableArray<T> values)
    {
        _values = values;
    }

    private readonly ImmutableArray<T> _values;

    public int Count => _values.IsDefault ? 0 : _values.Length;

    public T this[int index] => _values[index];

    public bool Equals(EquatableArray<T> other)
    {
        if (_values.IsDefault || other._values.IsDefault)
        {
            return _values.IsDefault == other._values.IsDefault;
        }

        return _values.SequenceEqual(other._values);
    }

    public override bool Equals(object? obj) => obj is EquatableArray<T> other && Equals(other);

    public override int GetHashCode()
    {
        if (_values.IsDefault)
        {
            return 0;
        }

        var hash = 17;

        foreach (var value in _values)
        {
            hash = unchecked(hash * 31 + (value?.GetHashCode() ?? 0));
        }

        return hash;
    }

    public IEnumerator<T> GetEnumerator() => (_values.IsDefault ? Enumerable.Empty<T>() : _values).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

internal static class EquatableArrayExtensions
{
    public static EquatableArray<T> ToEquatableArray<T>(this IEnumerable<T> values)
        where T : IEquatable<T>
    {
        return new EquatableArray<T>(values.ToImmutableArray());
    }
}

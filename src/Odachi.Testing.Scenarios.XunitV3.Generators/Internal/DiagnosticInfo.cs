using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Odachi.Testing.Scenarios.XunitV3.Generators.Internal;

/// <summary>
/// Value equatable stand in for <see cref="Location"/>. Holding onto syntax nodes and locations
/// would keep whole compilations alive between generator runs.
/// </summary>
internal readonly struct LocationInfo : IEquatable<LocationInfo>
{
    public static LocationInfo? From(SyntaxNode? node)
    {
        if (node is null)
        {
            return null;
        }

        return From(node.GetLocation());
    }

    public static LocationInfo? From(Location? location)
    {
        if (location is null || location.SourceTree is null)
        {
            return null;
        }

        return new LocationInfo(location.SourceTree.FilePath, location.SourceSpan, location.GetLineSpan().Span);
    }

    public LocationInfo(string filePath, TextSpan textSpan, LinePositionSpan lineSpan)
    {
        FilePath = filePath;
        TextSpan = textSpan;
        LineSpan = lineSpan;
    }

    public string FilePath { get; }
    public TextSpan TextSpan { get; }
    public LinePositionSpan LineSpan { get; }

    public Location ToLocation() => Location.Create(FilePath, TextSpan, LineSpan);

    public bool Equals(LocationInfo other) =>
        FilePath == other.FilePath && TextSpan.Equals(other.TextSpan) && LineSpan.Equals(other.LineSpan);

    public override bool Equals(object? obj) => obj is LocationInfo other && Equals(other);

    public override int GetHashCode() => unchecked(
        (FilePath?.GetHashCode() ?? 0) * 31 * 31 + TextSpan.GetHashCode() * 31 + LineSpan.GetHashCode()
    );
}

/// <summary>
/// Value equatable stand in for <see cref="Diagnostic"/>.
/// </summary>
internal sealed class DiagnosticInfo : IEquatable<DiagnosticInfo>
{
    public DiagnosticInfo(DiagnosticDescriptor descriptor, LocationInfo? location, EquatableArray<string> messageArgs)
    {
        Descriptor = descriptor;
        Location = location;
        MessageArgs = messageArgs;
    }

    public DiagnosticDescriptor Descriptor { get; }
    public LocationInfo? Location { get; }
    public EquatableArray<string> MessageArgs { get; }

    public Diagnostic ToDiagnostic()
    {
        var messageArgs = new object[MessageArgs.Count];

        for (var i = 0; i < MessageArgs.Count; i++)
        {
            messageArgs[i] = MessageArgs[i];
        }

        return Diagnostic.Create(Descriptor, Location?.ToLocation(), messageArgs);
    }

    public bool Equals(DiagnosticInfo? other)
    {
        if (other is null)
        {
            return false;
        }

        return ReferenceEquals(Descriptor, other.Descriptor)
            && Nullable.Equals(Location, other.Location)
            && MessageArgs.Equals(other.MessageArgs);
    }

    public override bool Equals(object? obj) => Equals(obj as DiagnosticInfo);

    public override int GetHashCode() => unchecked(
        Descriptor.Id.GetHashCode() * 31 * 31 + (Location?.GetHashCode() ?? 0) * 31 + MessageArgs.GetHashCode()
    );
}

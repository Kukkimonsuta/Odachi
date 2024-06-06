using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Odachi.EntityFrameworkCore;

/// <summary>
/// Generic entity representing a enum.
/// </summary>
/// <typeparam name="T">Enum type.</typeparam>
public class EnumEntity<T>(T value)
    where T : Enum
{
    public T Value { get; set; } = value;
    public string Code { get; set; } = value.ToString();
}

public class GenerateEnumTablesConvention(string? prefix = null, string? suffix = null, string? schema = null) :
    IEntityTypeAddedConvention,
    IPropertyAddedConvention,
    IModelFinalizingConvention
{
    public void ProcessEntityTypeAdded(IConventionEntityTypeBuilder entityTypeBuilder, IConventionContext<IConventionEntityTypeBuilder> context)
    {
        var clrType = entityTypeBuilder.Metadata.ClrType;
        if (clrType.IsGenericType && clrType.GetGenericTypeDefinition() == typeof(EnumEntity<>))
        {
            var primaryKeyProperty = entityTypeBuilder.Metadata.GetProperty(nameof(EnumEntity<Enum>.Value));
            entityTypeBuilder.PrimaryKey([primaryKeyProperty]);
        }
    }

    public void ProcessPropertyAdded(IConventionPropertyBuilder propertyBuilder, IConventionContext<IConventionPropertyBuilder> context)
    {
        var propertyType = propertyBuilder.Metadata.ClrType;

        if (propertyType.IsEnum)
        {
            // ensure all enums have their own lookup table
            var enumEntityType = typeof(EnumEntity<>).MakeGenericType(propertyType);

            if (propertyBuilder.ModelBuilder.Metadata.FindEntityType(enumEntityType) == null)
            {
                var entityTypeBuilder = propertyBuilder.ModelBuilder.Entity(enumEntityType, fromDataAnnotation: true /* otherwise it's considered orphaned and removed */);
                entityTypeBuilder?.ToTable($"{prefix}{propertyType.Name}{suffix}", schema);
            }
        }
    }

    public void ProcessModelFinalizing(
        IConventionModelBuilder modelBuilder,
        IConventionContext<IConventionModelBuilder> context
    )
    {
        foreach (var entityType in modelBuilder.Metadata.GetEntityTypes())
        {
            var clrType = entityType.ClrType;
            if (!clrType.IsGenericType || clrType.GetGenericTypeDefinition() != typeof(EnumEntity<>))
            {
                continue;
            }

            var enumType = clrType.GetGenericArguments()[0];

            var data = new Dictionary<object, object>();
            foreach (var value in Enum.GetValues(enumType))
            {
                if (value == null || data.Any(v => Equals(v.Key, value)))
                {
                    // filter out aliases
                    continue;
                }

                if (Activator.CreateInstance(clrType, [value]) is not { } entry)
                {
                    throw new InvalidOperationException($"Failed to instantiate {clrType.FullName} using value '{value}'");
                }

                data.Add(value, entry);
            }

            // this is probably invalid use, but so far it seem to work
            ((IMutableEntityType)entityType).AddData(data.Values);
        }
    }
}

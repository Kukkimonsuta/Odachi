using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Odachi.Build.OptionsFileGenerator.Model;

namespace Odachi.Build.OptionsFileGenerator.IO;

public class JsonOptionsFileWriter : OptionsFileWriter
{
    public string Indent { get; }

    public JsonOptionsFileWriter(TextWriter writer, string? rootKey, string indent = "  ")
        : base(writer, rootKey)
    {
        Indent = indent;
    }

    public JsonOptionsFileWriter(TextWriter writer, string[]? rootKey = null, string indent = "  ")
        : base(writer, rootKey)
    {
        Indent = indent;
    }

    private void WriteIndent(int level)
    {
        for (var i = 0; i < level; i++)
        {
            Writer.Write(Indent);
        }
    }

    private void WriteValue(object? obj)
    {
        switch (obj)
        {
            case null:
                Writer.Write("null");
                break;

            case bool boolValue:
	            Writer.Write(boolValue ? "true" : "false");
	            break;

            case sbyte:
            case byte:
            case short:
            case ushort:
            case int:
            case uint:
            case long:
            case ulong:
                Writer.Write($@"{obj}");
                break;

            case float floatValue:
                Writer.Write($@"{floatValue.ToString(CultureInfo.InvariantCulture)}");
                break;

            case double doubleValue:
                Writer.Write($@"{doubleValue.ToString(CultureInfo.InvariantCulture)}");
                break;

            case decimal decimalValue:
                Writer.Write($@"{decimalValue.ToString(CultureInfo.InvariantCulture)}");
                break;

            case string stringValue:
                Writer.Write($@"""{stringValue}""");
                break;

            case object?[] array:
	            Writer.Write("[");
	            for (var i = 0; i < array.Length; i++)
	            {
					WriteValue(array.GetValue(i));

					if (i < array.Length - 1)
					{
						Writer.Write(", ");
					}
	            }
	            Writer.Write("]");
	            break;

            case TimeSpan timeSpan:
	            var builder = new StringBuilder();
	            if (timeSpan.Days > 0)
	            {
		            builder.Append("d\\.");
	            }

	            builder.Append("hh\\:mm\\:ss");

	            var microseconds = (timeSpan.Ticks % TimeSpan.TicksPerMillisecond) / (TimeSpan.TicksPerMillisecond / 1000);

	            if (timeSpan.Milliseconds > 0 || microseconds > 0)
	            {
		            builder.Append("\\.fff");
	            }

	            if (microseconds > 0)
	            {
		            builder.Append("fff");
	            }

	            Writer.Write($@"""{timeSpan.ToString(builder.ToString(), CultureInfo.InvariantCulture)}""");
	            break;

            case EnumValue enumValue:
                Writer.Write("\"");
                for (var i = 0; i < enumValue.Values.Length; i++)
                {
                    var enumItemName = enumValue.Values[i];

                    Writer.Write(enumItemName);

                    if (i < enumValue.Values.Length - 1)
                    {
                        Writer.Write(", ");
                    }
                }
                Writer.Write("\"");
                break;

            default:
                // write unsupported types as null
                Writer.Write("null");
                break;
        }
    }

    private void Write(IReadOnlyList<OptionsProperty> properties, int indentLevel)
    {
        foreach (var property in properties)
        {
            if (!string.IsNullOrWhiteSpace(property.Description))
            {
                foreach (var line in property.Description!.Replace("\r", "").Split('\n'))
                {
                    WriteIndent(indentLevel);
                    Writer.Write("// ");
                    Writer.WriteLine(line);
                }
            }

            WriteIndent(indentLevel);

            if (property is LeafOptionsProperty leafProperty)
            {
                Writer.Write($@"""{property.Key}"": ");
                WriteValue(leafProperty.DefaultValue);
                Writer.WriteLine(",");
            }
            else if (property is BranchOptionsProperty branchProperty)
            {
                Writer.WriteLine($@"""{property.Key}"": {{");

                Write(branchProperty.Properties, indentLevel + 1);

                WriteIndent(indentLevel);
                Writer.WriteLine("},");
            }
        }
    }

    public override void Write(IReadOnlyList<OptionsProperty> properties)
    {
        Writer.WriteLine("{");

        var indent = 1;

        for (var i = 0; i < RootKey.Length; i++)
        {
            var property = RootKey[i];

            WriteIndent(indent);
            Writer.WriteLine($@"""{property}"": {{");

            indent += 1;
        }

        Write(properties, indent);

        for (var i = RootKey.Length - 1; i >= 0; i--)
        {
            indent -= 1;

            WriteIndent(indent);
            Writer.WriteLine("},");
        }

        Writer.WriteLine("}");
    }
}

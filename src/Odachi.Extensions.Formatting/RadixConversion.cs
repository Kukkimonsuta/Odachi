using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Odachi.Extensions.Formatting
{
	public static class RadixConversion
	{
		public static string ToString(byte[] source, string characterSet)
		{
			if (source == null)
				throw new ArgumentNullException(nameof(source));
			if (characterSet == null)
				throw new ArgumentNullException(nameof(characterSet));
			if (characterSet.Length <= 0 || characterSet.Length > int.MaxValue)
				throw new ArgumentOutOfRangeException(nameof(characterSet));

			var radix = characterSet.Length;

			if (source.Length <= 0)
			{
				return "";
			}

			var actualSource = new byte[source.Length + 1];
			Array.Copy(source, actualSource, source.Length);

			var remainder = new BigInteger(actualSource);
			if (remainder == 0)
			{
				return characterSet[0].ToString();
			}

			var inputLength = remainder.IsZero ? 1 : (int)(BigInteger.Log10(remainder) + 1);
			var maxOutputLength = (int)Math.Ceiling(inputLength * Math.Log(10) / Math.Log(radix));

			var result = new char[maxOutputLength];
			var resultIndex = result.Length - 1;

			while (remainder > 0)
			{
				remainder = BigInteger.DivRem(remainder, radix, out var v);

				result[resultIndex--] = characterSet[(int)v];
			}

			return new string(result, resultIndex + 1, result.Length - resultIndex -1);
		}

		public static byte[] FromString(string source, string characterSet)
		{
			if (source == null)
				throw new ArgumentNullException(nameof(source));
			if (characterSet == null)
				throw new ArgumentNullException(nameof(characterSet));
			if (characterSet.Length <= 0 || characterSet.Length > int.MaxValue)
				throw new ArgumentOutOfRangeException(nameof(characterSet));

			var radix = characterSet.Length;

			if (source.Length <= 0)
			{
				return Array.Empty<byte>();
			}

			var result = new BigInteger();

			for (var i = 0; i < source.Length; i++)
			{
				var ch = source[i];
				var n = characterSet.IndexOf(ch);
				if (n == -1)
					throw new FormatException($"Source has invalid format. Unexpected character '{ch}'");

				result *= radix;
				result += n;
			}

			var resultArray = result.ToByteArray();

			if (resultArray.Length > 1 && resultArray[resultArray.Length - 1] == 0)
			{
				Array.Resize(ref resultArray, resultArray.Length - 1);
			}

			return resultArray;
		}
	}
}

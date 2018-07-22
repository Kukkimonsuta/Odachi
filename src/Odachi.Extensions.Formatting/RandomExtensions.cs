#if !NETSTANDARD10
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Odachi.Extensions.Formatting
{
    public static class RandomExtensions
    {
		public const string NumericCharacters = "0123456789";
		public const string LowerCaseCharacters = "abcdefghijklmnopqrstuvwxyz";
		public const string UpperCaseCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		public const string DefaultCharacters = NumericCharacters + LowerCaseCharacters + UpperCaseCharacters;

		/// <summary>
		/// Returns random string of given length consisting of numeric, lowercase and uppercase english characters.
		/// </summary>
		/// <param name="rng">Random number generator</param>
		/// <param name="length">Length of resulting string</param>
		/// <returns>Random string</returns>
		public static string GetString(this RandomNumberGenerator rng, int length)
		{
			if (rng == null)
				throw new ArgumentNullException(nameof(rng));
			if (length <= 0)
				throw new ArgumentOutOfRangeException(nameof(length));

			return GetString(rng, DefaultCharacters, length);
		}

		/// <summary>
		/// Returns random string of given length consisting of given characters.
		/// </summary>
		/// <param name="rng">Random number generator</param>
		/// <param name="characters">String of 1-255 characters that are allowed to appear in resulting string. Repeated characters have higher chance to appear.</param>
		/// <param name="length">Length of resulting string</param>
		/// <returns>Random string</returns>
		public static string GetString(this RandomNumberGenerator rng, string characters, int length)
		{
			if (rng == null)
				throw new ArgumentNullException(nameof(rng));
			if (characters == null)
				throw new ArgumentNullException(nameof(characters));
			if (characters.Length <= 0 || characters.Length >= 256)
				throw new ArgumentOutOfRangeException(nameof(characters));
			if (length <= 0)
				throw new ArgumentOutOfRangeException(nameof(length));

			var buffer = new byte[length];
			rng.GetBytes(buffer);

			var result = new string('\0', length);

			unsafe
			{
				fixed (char* pResult = result)
				{
					for (var i = 0; i < buffer.Length; i++)
					{
						var ci = (int)(((1.0 / (byte.MaxValue + 1)) * buffer[i]) * (characters.Length));

						pResult[i] = characters[ci];
					}
				}
			}

			return result;
		}
	}
}
#endif

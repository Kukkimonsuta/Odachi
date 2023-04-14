using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Odachi.Extensions.Primitives
{
	public class OneOf
	{
		public const int MinTypes = 2;
		public const int MaxTypes = 9;
	}

	[DataContract]
	public struct OneOf<T1, T2> : IEquatable<OneOf<T1, T2>>
	{
		private static readonly EqualityComparer<T1> s_t1Comparer = EqualityComparer<T1>.Default;
		private static readonly EqualityComparer<T2> s_t2Comparer = EqualityComparer<T2>.Default;

		public OneOf(T1 value)
		{
			Index = 1;
			Option1 = value;
			Option2 = default(T2);
		}
		public OneOf(T2 value)
		{
			Index = 2;
			Option1 = default(T1);
			Option2 = value;
		}

		[DataMember(Order = 0)]
		public readonly int Index;
		[DataMember(EmitDefaultValue = true, Order = 1)]
		public readonly T1? Option1;
		[DataMember(EmitDefaultValue = true, Order = 2)]
		public readonly T2? Option2;

		public bool IsEmpty => Index == 0;

		public bool Is1 => Index == 1;
		public T1 As1 => Index == 1 ? Option1! : throw new InvalidOperationException("OneOf<T1, T2> doesn't contain T1");

		public bool Is2 => Index == 2;
		public T2 As2 => Index == 2 ? Option2! : throw new InvalidOperationException("OneOf<T1, T2> doesn't contain T2");

		public object Value
		{
			get
			{
				switch (Index)
				{
					case 0:
						throw new InvalidOperationException("OneOf<T1, T2> is empty");
					case 1:
						return Option1!;
					case 2:
						return Option2!;
					default:
						throw new InvalidOperationException($"Undefined behavior for OneOf<T1, T2> with Index {Index}'");
				}
			}
		}

		public void Match(Action<T1> when1, Action<T2> when2, Action? whenEmpty = null)
		{
			switch (Index)
			{
				case 0:
					if (whenEmpty == null)
						throw new InvalidOperationException("OneOf<T1, T2> is empty");
					whenEmpty();
					return;
				case 1:
					when1(Option1!);
					return;
				case 2:
					when2(Option2!);
					return;
				default:
					throw new InvalidOperationException($"Undefined behavior for OneOf<T1, T2> with Index {Index}'");
			}
		}
		public TResult Match<TResult>(Func<T1, TResult> when1, Func<T2, TResult> when2, Func<TResult>? whenEmpty = null)
		{
			switch (Index)
			{
				case 0:
					return whenEmpty != null ? whenEmpty() : throw new InvalidOperationException("OneOf<T1, T2> is empty");
				case 1:
					return when1(Option1!);
				case 2:
					return when2(Option2!);
				default:
					throw new InvalidOperationException($"Undefined behavior for OneOf<T1, T2> with Index {Index}'");
			}
		}

		public override int GetHashCode()
		{
			var hash = Index.GetHashCode();

			switch (Index)
			{
				case 1:
					hash ^= s_t1Comparer.GetHashCode(As1);
					break;
				case 2:
					hash ^= s_t2Comparer.GetHashCode(As2);
					break;
			}

			return hash;
		}

		public override bool Equals(object obj)
		{
			if (obj is OneOf<T1, T2> oneOfObj)
			{
				return Equals(oneOfObj);
			}

			return false;
		}

		public bool Equals(OneOf<T1, T2> other)
		{
			return Equals(this, other);
		}

		public static bool operator ==(OneOf<T1, T2> a, OneOf<T1, T2> b)
		{
			return Equals(a, b);
		}
		public static bool operator !=(OneOf<T1, T2> a, OneOf<T1, T2> b)
		{
			return !Equals(a, b);
		}

		public static implicit operator OneOf<T1, T2>(T1 value)
		{
			return new OneOf<T1, T2>(value);
		}
		public static implicit operator OneOf<T1, T2>(T2 value)
		{
			return new OneOf<T1, T2>(value);
		}

		public static explicit operator T1(OneOf<T1, T2> value)
		{
			return value.As1;
		}
		public static explicit operator T2(OneOf<T1, T2> value)
		{
			return value.As2;
		}

		public static bool Equals(OneOf<T1, T2> a, OneOf<T1, T2> b)
		{
			if (a.Index != b.Index)
			{
				return false;
			}

			switch (a.Index)
			{
				case 0:
					return true;
				case 1:
					return s_t1Comparer.Equals(a.As1, b.As1);
				case 2:
					return s_t2Comparer.Equals(a.As2, b.As2);
				default:
					return false;
			}
		}
	}

	[DataContract]
	public struct OneOf<T1, T2, T3> : IEquatable<OneOf<T1, T2, T3>>
	{
		private static readonly EqualityComparer<T1> s_t1Comparer = EqualityComparer<T1>.Default;
		private static readonly EqualityComparer<T2> s_t2Comparer = EqualityComparer<T2>.Default;
		private static readonly EqualityComparer<T3> s_t3Comparer = EqualityComparer<T3>.Default;

		public OneOf(T1 value)
		{
			Index = 1;
			Option1 = value;
			Option2 = default(T2);
			Option3 = default(T3);
		}
		public OneOf(T2 value)
		{
			Index = 2;
			Option1 = default(T1);
			Option2 = value;
			Option3 = default(T3);
		}
		public OneOf(T3 value)
		{
			Index = 3;
			Option1 = default(T1);
			Option2 = default(T2);
			Option3 = value;
		}

		[DataMember(Order = 0)]
		public readonly int Index;
		[DataMember(EmitDefaultValue = true, Order = 1)]
		public readonly T1? Option1;
		[DataMember(EmitDefaultValue = true, Order = 2)]
		public readonly T2? Option2;
		[DataMember(EmitDefaultValue = true, Order = 3)]
		public readonly T3? Option3;

		public bool IsEmpty => Index == 0;

		public bool Is1 => Index == 1;
		public T1 As1 => Index == 1 ? Option1! : throw new InvalidOperationException("OneOf<T1, T2, T3> doesn't contain T1");

		public bool Is2 => Index == 2;
		public T2 As2 => Index == 2 ? Option2! : throw new InvalidOperationException("OneOf<T1, T2, T3> doesn't contain T2");

		public bool Is3 => Index == 3;
		public T3 As3 => Index == 3 ? Option3! : throw new InvalidOperationException("OneOf<T1, T2, T3> doesn't contain T3");

		public object Value
		{
			get
			{
				switch (Index)
				{
					case 0:
						throw new InvalidOperationException("OneOf<T1, T2, T3> is empty");
					case 1:
						return Option1!;
					case 2:
						return Option2!;
					case 3:
						return Option3!;
					default:
						throw new InvalidOperationException($"Undefined behavior for OneOf<T1, T2, T3> with Index {Index}'");
				}
			}
		}

		public void Match(Action<T1> when1, Action<T2> when2, Action<T3> when3, Action? whenEmpty = null)
		{
			switch (Index)
			{
				case 0:
					if (whenEmpty == null)
						throw new InvalidOperationException("OneOf<T1, T2, T3> is empty");
					whenEmpty();
					return;
				case 1:
					when1(Option1!);
					return;
				case 2:
					when2(Option2!);
					return;
				case 3:
					when3(Option3!);
					return;
				default:
					throw new InvalidOperationException($"Undefined behavior for OneOf<T1, T2, T3> with Index {Index}'");
			}
		}
		public TResult Match<TResult>(Func<T1, TResult> when1, Func<T2, TResult> when2, Func<T3, TResult> when3, Func<TResult>? whenEmpty = null)
		{
			switch (Index)
			{
				case 0:
					return whenEmpty != null ? whenEmpty() : throw new InvalidOperationException("OneOf<T1, T2, T3> is empty");
				case 1:
					return when1(Option1!);
				case 2:
					return when2(Option2!);
				case 3:
					return when3(Option3!);
				default:
					throw new InvalidOperationException($"Undefined behavior for OneOf<T1, T2, T3> with Index {Index}'");
			}
		}

		public override int GetHashCode()
		{
			var hash = Index.GetHashCode();

			switch (Index)
			{
				case 1:
					hash ^= s_t1Comparer.GetHashCode(As1);
					break;
				case 2:
					hash ^= s_t2Comparer.GetHashCode(As2);
					break;
				case 3:
					hash ^= s_t3Comparer.GetHashCode(As3);
					break;
			}

			return hash;
		}

		public override bool Equals(object obj)
		{
			if (obj is OneOf<T1, T2, T3> oneOfObj)
			{
				return Equals(oneOfObj);
			}

			return false;
		}

		public bool Equals(OneOf<T1, T2, T3> other)
		{
			return Equals(this, other);
		}

		public static bool operator ==(OneOf<T1, T2, T3> a, OneOf<T1, T2, T3> b)
		{
			return Equals(a, b);
		}
		public static bool operator !=(OneOf<T1, T2, T3> a, OneOf<T1, T2, T3> b)
		{
			return !Equals(a, b);
		}

		public static implicit operator OneOf<T1, T2, T3>(T1 value)
		{
			return new OneOf<T1, T2, T3>(value);
		}
		public static implicit operator OneOf<T1, T2, T3>(T2 value)
		{
			return new OneOf<T1, T2, T3>(value);
		}
		public static implicit operator OneOf<T1, T2, T3>(T3 value)
		{
			return new OneOf<T1, T2, T3>(value);
		}

		public static explicit operator T1(OneOf<T1, T2, T3> value)
		{
			return value.As1;
		}
		public static explicit operator T2(OneOf<T1, T2, T3> value)
		{
			return value.As2;
		}
		public static explicit operator T3(OneOf<T1, T2, T3> value)
		{
			return value.As3;
		}

		public static bool Equals(OneOf<T1, T2, T3> a, OneOf<T1, T2, T3> b)
		{
			if (a.Index != b.Index)
			{
				return false;
			}

			switch (a.Index)
			{
				case 0:
					return true;
				case 1:
					return s_t1Comparer.Equals(a.As1, b.As1);
				case 2:
					return s_t2Comparer.Equals(a.As2, b.As2);
				case 3:
					return s_t3Comparer.Equals(a.As3, b.As3);
				default:
					return false;
			}
		}
	}

	[DataContract]
	public struct OneOf<T1, T2, T3, T4> : IEquatable<OneOf<T1, T2, T3, T4>>
	{
		private static readonly EqualityComparer<T1> s_t1Comparer = EqualityComparer<T1>.Default;
		private static readonly EqualityComparer<T2> s_t2Comparer = EqualityComparer<T2>.Default;
		private static readonly EqualityComparer<T3> s_t3Comparer = EqualityComparer<T3>.Default;
		private static readonly EqualityComparer<T4> s_t4Comparer = EqualityComparer<T4>.Default;

		public OneOf(T1 value)
		{
			Index = 1;
			Option1 = value;
			Option2 = default(T2);
			Option3 = default(T3);
			Option4 = default(T4);
		}
		public OneOf(T2 value)
		{
			Index = 2;
			Option1 = default(T1);
			Option2 = value;
			Option3 = default(T3);
			Option4 = default(T4);
		}
		public OneOf(T3 value)
		{
			Index = 3;
			Option1 = default(T1);
			Option2 = default(T2);
			Option3 = value;
			Option4 = default(T4);
		}
		public OneOf(T4 value)
		{
			Index = 4;
			Option1 = default(T1);
			Option2 = default(T2);
			Option3 = default(T3);
			Option4 = value;
		}

		[DataMember(Order = 0)]
		public readonly int Index;
		[DataMember(EmitDefaultValue = true, Order = 1)]
		public readonly T1? Option1;
		[DataMember(EmitDefaultValue = true, Order = 2)]
		public readonly T2? Option2;
		[DataMember(EmitDefaultValue = true, Order = 3)]
		public readonly T3? Option3;
		[DataMember(EmitDefaultValue = true, Order = 4)]
		public readonly T4? Option4;

		public bool IsEmpty => Index == 0;

		public bool Is1 => Index == 1;
		public T1 As1 => Index == 1 ? Option1! : throw new InvalidOperationException("OneOf<T1, T2, T3, T4> doesn't contain T1");

		public bool Is2 => Index == 2;
		public T2 As2 => Index == 2 ? Option2! : throw new InvalidOperationException("OneOf<T1, T2, T3, T4> doesn't contain T2");

		public bool Is3 => Index == 3;
		public T3 As3 => Index == 3 ? Option3! : throw new InvalidOperationException("OneOf<T1, T2, T3, T4> doesn't contain T3");

		public bool Is4 => Index == 4;
		public T4 As4 => Index == 4 ? Option4! : throw new InvalidOperationException("OneOf<T1, T2, T3, T4> doesn't contain T4");

		public object Value
		{
			get
			{
				switch (Index)
				{
					case 0:
						throw new InvalidOperationException("OneOf<T1, T2, T3, T4> is empty");
					case 1:
						return Option1!;
					case 2:
						return Option2!;
					case 3:
						return Option3!;
					case 4:
						return Option4!;
					default:
						throw new InvalidOperationException($"Undefined behavior for OneOf<T1, T2, T3, T4> with Index {Index}'");
				}
			}
		}

		public void Match(Action<T1> when1, Action<T2> when2, Action<T3> when3, Action<T4> when4, Action? whenEmpty = null)
		{
			switch (Index)
			{
				case 0:
					if (whenEmpty == null)
						throw new InvalidOperationException("OneOf<T1, T2, T3, T4> is empty");
					whenEmpty();
					return;
				case 1:
					when1(Option1!);
					return;
				case 2:
					when2(Option2!);
					return;
				case 3:
					when3(Option3!);
					return;
				case 4:
					when4(Option4!);
					return;
				default:
					throw new InvalidOperationException($"Undefined behavior for OneOf<T1, T2, T3, T4> with Index {Index}'");
			}
		}
		public TResult Match<TResult>(Func<T1, TResult> when1, Func<T2, TResult> when2, Func<T3, TResult> when3, Func<T4, TResult> when4, Func<TResult>? whenEmpty = null)
		{
			switch (Index)
			{
				case 0:
					return whenEmpty != null ? whenEmpty() : throw new InvalidOperationException("OneOf<T1, T2, T3, T4> is empty");
				case 1:
					return when1(Option1!);
				case 2:
					return when2(Option2!);
				case 3:
					return when3(Option3!);
				case 4:
					return when4(Option4!);
				default:
					throw new InvalidOperationException($"Undefined behavior for OneOf<T1, T2, T3, T4> with Index {Index}'");
			}
		}

		public override int GetHashCode()
		{
			var hash = Index.GetHashCode();

			switch (Index)
			{
				case 1:
					hash ^= s_t1Comparer.GetHashCode(As1);
					break;
				case 2:
					hash ^= s_t2Comparer.GetHashCode(As2);
					break;
				case 3:
					hash ^= s_t3Comparer.GetHashCode(As3);
					break;
				case 4:
					hash ^= s_t4Comparer.GetHashCode(As4);
					break;
			}

			return hash;
		}

		public override bool Equals(object obj)
		{
			if (obj is OneOf<T1, T2, T3, T4> oneOfObj)
			{
				return Equals(oneOfObj);
			}

			return false;
		}

		public bool Equals(OneOf<T1, T2, T3, T4> other)
		{
			return Equals(this, other);
		}

		public static bool operator ==(OneOf<T1, T2, T3, T4> a, OneOf<T1, T2, T3, T4> b)
		{
			return Equals(a, b);
		}
		public static bool operator !=(OneOf<T1, T2, T3, T4> a, OneOf<T1, T2, T3, T4> b)
		{
			return !Equals(a, b);
		}

		public static implicit operator OneOf<T1, T2, T3, T4>(T1 value)
		{
			return new OneOf<T1, T2, T3, T4>(value);
		}
		public static implicit operator OneOf<T1, T2, T3, T4>(T2 value)
		{
			return new OneOf<T1, T2, T3, T4>(value);
		}
		public static implicit operator OneOf<T1, T2, T3, T4>(T3 value)
		{
			return new OneOf<T1, T2, T3, T4>(value);
		}
		public static implicit operator OneOf<T1, T2, T3, T4>(T4 value)
		{
			return new OneOf<T1, T2, T3, T4>(value);
		}

		public static explicit operator T1(OneOf<T1, T2, T3, T4> value)
		{
			return value.As1;
		}
		public static explicit operator T2(OneOf<T1, T2, T3, T4> value)
		{
			return value.As2;
		}
		public static explicit operator T3(OneOf<T1, T2, T3, T4> value)
		{
			return value.As3;
		}
		public static explicit operator T4(OneOf<T1, T2, T3, T4> value)
		{
			return value.As4;
		}

		public static bool Equals(OneOf<T1, T2, T3, T4> a, OneOf<T1, T2, T3, T4> b)
		{
			if (a.Index != b.Index)
			{
				return false;
			}

			switch (a.Index)
			{
				case 0:
					return true;
				case 1:
					return s_t1Comparer.Equals(a.As1, b.As1);
				case 2:
					return s_t2Comparer.Equals(a.As2, b.As2);
				case 3:
					return s_t3Comparer.Equals(a.As3, b.As3);
				case 4:
					return s_t4Comparer.Equals(a.As4, b.As4);
				default:
					return false;
			}
		}
	}

	[DataContract]
	public struct OneOf<T1, T2, T3, T4, T5> : IEquatable<OneOf<T1, T2, T3, T4, T5>>
	{
		private static readonly EqualityComparer<T1> s_t1Comparer = EqualityComparer<T1>.Default;
		private static readonly EqualityComparer<T2> s_t2Comparer = EqualityComparer<T2>.Default;
		private static readonly EqualityComparer<T3> s_t3Comparer = EqualityComparer<T3>.Default;
		private static readonly EqualityComparer<T4> s_t4Comparer = EqualityComparer<T4>.Default;
		private static readonly EqualityComparer<T5> s_t5Comparer = EqualityComparer<T5>.Default;

		public OneOf(T1 value)
		{
			Index = 1;
			Option1 = value;
			Option2 = default(T2);
			Option3 = default(T3);
			Option4 = default(T4);
			Option5 = default(T5);
		}
		public OneOf(T2 value)
		{
			Index = 2;
			Option1 = default(T1);
			Option2 = value;
			Option3 = default(T3);
			Option4 = default(T4);
			Option5 = default(T5);
		}
		public OneOf(T3 value)
		{
			Index = 3;
			Option1 = default(T1);
			Option2 = default(T2);
			Option3 = value;
			Option4 = default(T4);
			Option5 = default(T5);
		}
		public OneOf(T4 value)
		{
			Index = 4;
			Option1 = default(T1);
			Option2 = default(T2);
			Option3 = default(T3);
			Option4 = value;
			Option5 = default(T5);
		}
		public OneOf(T5 value)
		{
			Index = 5;
			Option1 = default(T1);
			Option2 = default(T2);
			Option3 = default(T3);
			Option4 = default(T4);
			Option5 = value;
		}

		[DataMember(Order = 0)]
		public readonly int Index;
		[DataMember(EmitDefaultValue = true, Order = 1)]
		public readonly T1? Option1;
		[DataMember(EmitDefaultValue = true, Order = 2)]
		public readonly T2? Option2;
		[DataMember(EmitDefaultValue = true, Order = 3)]
		public readonly T3? Option3;
		[DataMember(EmitDefaultValue = true, Order = 4)]
		public readonly T4? Option4;
		[DataMember(EmitDefaultValue = true, Order = 5)]
		public readonly T5? Option5;

		public bool IsEmpty => Index == 0;

		public bool Is1 => Index == 1;
		public T1 As1 => Index == 1 ? Option1! : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5> doesn't contain T1");

		public bool Is2 => Index == 2;
		public T2 As2 => Index == 2 ? Option2! : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5> doesn't contain T2");

		public bool Is3 => Index == 3;
		public T3 As3 => Index == 3 ? Option3! : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5> doesn't contain T3");

		public bool Is4 => Index == 4;
		public T4 As4 => Index == 4 ? Option4! : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5> doesn't contain T4");

		public bool Is5 => Index == 5;
		public T5 As5 => Index == 5 ? Option5! : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5> doesn't contain T5");

		public object Value
		{
			get
			{
				switch (Index)
				{
					case 0:
						throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5> is empty");
					case 1:
						return Option1!;
					case 2:
						return Option2!;
					case 3:
						return Option3!;
					case 4:
						return Option4!;
					case 5:
						return Option5!;
					default:
						throw new InvalidOperationException($"Undefined behavior for OneOf<T1, T2, T3, T4, T5> with Index {Index}'");
				}
			}
		}

		public void Match(Action<T1> when1, Action<T2> when2, Action<T3> when3, Action<T4> when4, Action<T5> when5, Action? whenEmpty = null)
		{
			switch (Index)
			{
				case 0:
					if (whenEmpty == null)
						throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5> is empty");
					whenEmpty();
					return;
				case 1:
					when1(Option1!);
					return;
				case 2:
					when2(Option2!);
					return;
				case 3:
					when3(Option3!);
					return;
				case 4:
					when4(Option4!);
					return;
				case 5:
					when5(Option5!);
					return;
				default:
					throw new InvalidOperationException($"Undefined behavior for OneOf<T1, T2, T3, T4, T5> with Index {Index}'");
			}
		}
		public TResult Match<TResult>(Func<T1, TResult> when1, Func<T2, TResult> when2, Func<T3, TResult> when3, Func<T4, TResult> when4, Func<T5, TResult> when5, Func<TResult>? whenEmpty = null)
		{
			switch (Index)
			{
				case 0:
					return whenEmpty != null ? whenEmpty() : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5> is empty");
				case 1:
					return when1(Option1!);
				case 2:
					return when2(Option2!);
				case 3:
					return when3(Option3!);
				case 4:
					return when4(Option4!);
				case 5:
					return when5(Option5!);
				default:
					throw new InvalidOperationException($"Undefined behavior for OneOf<T1, T2, T3, T4, T5> with Index {Index}'");
			}
		}

		public override int GetHashCode()
		{
			var hash = Index.GetHashCode();

			switch (Index)
			{
				case 1:
					hash ^= s_t1Comparer.GetHashCode(As1);
					break;
				case 2:
					hash ^= s_t2Comparer.GetHashCode(As2);
					break;
				case 3:
					hash ^= s_t3Comparer.GetHashCode(As3);
					break;
				case 4:
					hash ^= s_t4Comparer.GetHashCode(As4);
					break;
				case 5:
					hash ^= s_t5Comparer.GetHashCode(As5);
					break;
			}

			return hash;
		}

		public override bool Equals(object obj)
		{
			if (obj is OneOf<T1, T2, T3, T4, T5> oneOfObj)
			{
				return Equals(oneOfObj);
			}

			return false;
		}

		public bool Equals(OneOf<T1, T2, T3, T4, T5> other)
		{
			return Equals(this, other);
		}

		public static bool operator ==(OneOf<T1, T2, T3, T4, T5> a, OneOf<T1, T2, T3, T4, T5> b)
		{
			return Equals(a, b);
		}
		public static bool operator !=(OneOf<T1, T2, T3, T4, T5> a, OneOf<T1, T2, T3, T4, T5> b)
		{
			return !Equals(a, b);
		}

		public static implicit operator OneOf<T1, T2, T3, T4, T5>(T1 value)
		{
			return new OneOf<T1, T2, T3, T4, T5>(value);
		}
		public static implicit operator OneOf<T1, T2, T3, T4, T5>(T2 value)
		{
			return new OneOf<T1, T2, T3, T4, T5>(value);
		}
		public static implicit operator OneOf<T1, T2, T3, T4, T5>(T3 value)
		{
			return new OneOf<T1, T2, T3, T4, T5>(value);
		}
		public static implicit operator OneOf<T1, T2, T3, T4, T5>(T4 value)
		{
			return new OneOf<T1, T2, T3, T4, T5>(value);
		}
		public static implicit operator OneOf<T1, T2, T3, T4, T5>(T5 value)
		{
			return new OneOf<T1, T2, T3, T4, T5>(value);
		}

		public static explicit operator T1(OneOf<T1, T2, T3, T4, T5> value)
		{
			return value.As1;
		}
		public static explicit operator T2(OneOf<T1, T2, T3, T4, T5> value)
		{
			return value.As2;
		}
		public static explicit operator T3(OneOf<T1, T2, T3, T4, T5> value)
		{
			return value.As3;
		}
		public static explicit operator T4(OneOf<T1, T2, T3, T4, T5> value)
		{
			return value.As4;
		}
		public static explicit operator T5(OneOf<T1, T2, T3, T4, T5> value)
		{
			return value.As5;
		}

		public static bool Equals(OneOf<T1, T2, T3, T4, T5> a, OneOf<T1, T2, T3, T4, T5> b)
		{
			if (a.Index != b.Index)
			{
				return false;
			}

			switch (a.Index)
			{
				case 0:
					return true;
				case 1:
					return s_t1Comparer.Equals(a.As1, b.As1);
				case 2:
					return s_t2Comparer.Equals(a.As2, b.As2);
				case 3:
					return s_t3Comparer.Equals(a.As3, b.As3);
				case 4:
					return s_t4Comparer.Equals(a.As4, b.As4);
				case 5:
					return s_t5Comparer.Equals(a.As5, b.As5);
				default:
					return false;
			}
		}
	}

	[DataContract]
	public struct OneOf<T1, T2, T3, T4, T5, T6> : IEquatable<OneOf<T1, T2, T3, T4, T5, T6>>
	{
		private static readonly EqualityComparer<T1> s_t1Comparer = EqualityComparer<T1>.Default;
		private static readonly EqualityComparer<T2> s_t2Comparer = EqualityComparer<T2>.Default;
		private static readonly EqualityComparer<T3> s_t3Comparer = EqualityComparer<T3>.Default;
		private static readonly EqualityComparer<T4> s_t4Comparer = EqualityComparer<T4>.Default;
		private static readonly EqualityComparer<T5> s_t5Comparer = EqualityComparer<T5>.Default;
		private static readonly EqualityComparer<T6> s_t6Comparer = EqualityComparer<T6>.Default;

		public OneOf(T1 value)
		{
			Index = 1;
			Option1 = value;
			Option2 = default(T2);
			Option3 = default(T3);
			Option4 = default(T4);
			Option5 = default(T5);
			Option6 = default(T6);
		}
		public OneOf(T2 value)
		{
			Index = 2;
			Option1 = default(T1);
			Option2 = value;
			Option3 = default(T3);
			Option4 = default(T4);
			Option5 = default(T5);
			Option6 = default(T6);
		}
		public OneOf(T3 value)
		{
			Index = 3;
			Option1 = default(T1);
			Option2 = default(T2);
			Option3 = value;
			Option4 = default(T4);
			Option5 = default(T5);
			Option6 = default(T6);
		}
		public OneOf(T4 value)
		{
			Index = 4;
			Option1 = default(T1);
			Option2 = default(T2);
			Option3 = default(T3);
			Option4 = value;
			Option5 = default(T5);
			Option6 = default(T6);
		}
		public OneOf(T5 value)
		{
			Index = 5;
			Option1 = default(T1);
			Option2 = default(T2);
			Option3 = default(T3);
			Option4 = default(T4);
			Option5 = value;
			Option6 = default(T6);
		}
		public OneOf(T6 value)
		{
			Index = 6;
			Option1 = default(T1);
			Option2 = default(T2);
			Option3 = default(T3);
			Option4 = default(T4);
			Option5 = default(T5);
			Option6 = value;
		}

		[DataMember(Order = 0)]
		public readonly int Index;
		[DataMember(EmitDefaultValue = true, Order = 1)]
		public readonly T1? Option1;
		[DataMember(EmitDefaultValue = true, Order = 2)]
		public readonly T2? Option2;
		[DataMember(EmitDefaultValue = true, Order = 3)]
		public readonly T3? Option3;
		[DataMember(EmitDefaultValue = true, Order = 4)]
		public readonly T4? Option4;
		[DataMember(EmitDefaultValue = true, Order = 5)]
		public readonly T5? Option5;
		[DataMember(EmitDefaultValue = true, Order = 6)]
		public readonly T6? Option6;

		public bool IsEmpty => Index == 0;

		public bool Is1 => Index == 1;
		public T1 As1 => Index == 1 ? Option1! : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5, T6> doesn't contain T1");

		public bool Is2 => Index == 2;
		public T2 As2 => Index == 2 ? Option2! : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5, T6> doesn't contain T2");

		public bool Is3 => Index == 3;
		public T3 As3 => Index == 3 ? Option3! : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5, T6> doesn't contain T3");

		public bool Is4 => Index == 4;
		public T4 As4 => Index == 4 ? Option4! : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5, T6> doesn't contain T4");

		public bool Is5 => Index == 5;
		public T5 As5 => Index == 5 ? Option5! : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5, T6> doesn't contain T5");

		public bool Is6 => Index == 6;
		public T6 As6 => Index == 6 ? Option6! : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5, T6> doesn't contain T6");

		public object Value
		{
			get
			{
				switch (Index)
				{
					case 0:
						throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5, T6> is empty");
					case 1:
						return Option1!;
					case 2:
						return Option2!;
					case 3:
						return Option3!;
					case 4:
						return Option4!;
					case 5:
						return Option5!;
					case 6:
						return Option6!;
					default:
						throw new InvalidOperationException($"Undefined behavior for OneOf<T1, T2, T3, T4, T5, T6> with Index {Index}'");
				}
			}
		}

		public void Match(Action<T1> when1, Action<T2> when2, Action<T3> when3, Action<T4> when4, Action<T5> when5, Action<T6> when6, Action? whenEmpty = null)
		{
			switch (Index)
			{
				case 0:
					if (whenEmpty == null)
						throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5, T6> is empty");
					whenEmpty();
					return;
				case 1:
					when1(Option1!);
					return;
				case 2:
					when2(Option2!);
					return;
				case 3:
					when3(Option3!);
					return;
				case 4:
					when4(Option4!);
					return;
				case 5:
					when5(Option5!);
					return;
				case 6:
					when6(Option6!);
					return;
				default:
					throw new InvalidOperationException($"Undefined behavior for OneOf<T1, T2, T3, T4, T5, T6> with Index {Index}'");
			}
		}
		public TResult Match<TResult>(Func<T1, TResult> when1, Func<T2, TResult> when2, Func<T3, TResult> when3, Func<T4, TResult> when4, Func<T5, TResult> when5, Func<T6, TResult> when6, Func<TResult>? whenEmpty = null)
		{
			switch (Index)
			{
				case 0:
					return whenEmpty != null ? whenEmpty() : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5, T6> is empty");
				case 1:
					return when1(Option1!);
				case 2:
					return when2(Option2!);
				case 3:
					return when3(Option3!);
				case 4:
					return when4(Option4!);
				case 5:
					return when5(Option5!);
				case 6:
					return when6(Option6!);
				default:
					throw new InvalidOperationException($"Undefined behavior for OneOf<T1, T2, T3, T4, T5, T6> with Index {Index}'");
			}
		}

		public override int GetHashCode()
		{
			var hash = Index.GetHashCode();

			switch (Index)
			{
				case 1:
					hash ^= s_t1Comparer.GetHashCode(As1);
					break;
				case 2:
					hash ^= s_t2Comparer.GetHashCode(As2);
					break;
				case 3:
					hash ^= s_t3Comparer.GetHashCode(As3);
					break;
				case 4:
					hash ^= s_t4Comparer.GetHashCode(As4);
					break;
				case 5:
					hash ^= s_t5Comparer.GetHashCode(As5);
					break;
				case 6:
					hash ^= s_t6Comparer.GetHashCode(As6);
					break;
			}

			return hash;
		}

		public override bool Equals(object obj)
		{
			if (obj is OneOf<T1, T2, T3, T4, T5, T6> oneOfObj)
			{
				return Equals(oneOfObj);
			}

			return false;
		}

		public bool Equals(OneOf<T1, T2, T3, T4, T5, T6> other)
		{
			return Equals(this, other);
		}

		public static bool operator ==(OneOf<T1, T2, T3, T4, T5, T6> a, OneOf<T1, T2, T3, T4, T5, T6> b)
		{
			return Equals(a, b);
		}
		public static bool operator !=(OneOf<T1, T2, T3, T4, T5, T6> a, OneOf<T1, T2, T3, T4, T5, T6> b)
		{
			return !Equals(a, b);
		}

		public static implicit operator OneOf<T1, T2, T3, T4, T5, T6>(T1 value)
		{
			return new OneOf<T1, T2, T3, T4, T5, T6>(value);
		}
		public static implicit operator OneOf<T1, T2, T3, T4, T5, T6>(T2 value)
		{
			return new OneOf<T1, T2, T3, T4, T5, T6>(value);
		}
		public static implicit operator OneOf<T1, T2, T3, T4, T5, T6>(T3 value)
		{
			return new OneOf<T1, T2, T3, T4, T5, T6>(value);
		}
		public static implicit operator OneOf<T1, T2, T3, T4, T5, T6>(T4 value)
		{
			return new OneOf<T1, T2, T3, T4, T5, T6>(value);
		}
		public static implicit operator OneOf<T1, T2, T3, T4, T5, T6>(T5 value)
		{
			return new OneOf<T1, T2, T3, T4, T5, T6>(value);
		}
		public static implicit operator OneOf<T1, T2, T3, T4, T5, T6>(T6 value)
		{
			return new OneOf<T1, T2, T3, T4, T5, T6>(value);
		}

		public static explicit operator T1(OneOf<T1, T2, T3, T4, T5, T6> value)
		{
			return value.As1;
		}
		public static explicit operator T2(OneOf<T1, T2, T3, T4, T5, T6> value)
		{
			return value.As2;
		}
		public static explicit operator T3(OneOf<T1, T2, T3, T4, T5, T6> value)
		{
			return value.As3;
		}
		public static explicit operator T4(OneOf<T1, T2, T3, T4, T5, T6> value)
		{
			return value.As4;
		}
		public static explicit operator T5(OneOf<T1, T2, T3, T4, T5, T6> value)
		{
			return value.As5;
		}
		public static explicit operator T6(OneOf<T1, T2, T3, T4, T5, T6> value)
		{
			return value.As6;
		}

		public static bool Equals(OneOf<T1, T2, T3, T4, T5, T6> a, OneOf<T1, T2, T3, T4, T5, T6> b)
		{
			if (a.Index != b.Index)
			{
				return false;
			}

			switch (a.Index)
			{
				case 0:
					return true;
				case 1:
					return s_t1Comparer.Equals(a.As1, b.As1);
				case 2:
					return s_t2Comparer.Equals(a.As2, b.As2);
				case 3:
					return s_t3Comparer.Equals(a.As3, b.As3);
				case 4:
					return s_t4Comparer.Equals(a.As4, b.As4);
				case 5:
					return s_t5Comparer.Equals(a.As5, b.As5);
				case 6:
					return s_t6Comparer.Equals(a.As6, b.As6);
				default:
					return false;
			}
		}
	}

	[DataContract]
	public struct OneOf<T1, T2, T3, T4, T5, T6, T7> : IEquatable<OneOf<T1, T2, T3, T4, T5, T6, T7>>
	{
		private static readonly EqualityComparer<T1> s_t1Comparer = EqualityComparer<T1>.Default;
		private static readonly EqualityComparer<T2> s_t2Comparer = EqualityComparer<T2>.Default;
		private static readonly EqualityComparer<T3> s_t3Comparer = EqualityComparer<T3>.Default;
		private static readonly EqualityComparer<T4> s_t4Comparer = EqualityComparer<T4>.Default;
		private static readonly EqualityComparer<T5> s_t5Comparer = EqualityComparer<T5>.Default;
		private static readonly EqualityComparer<T6> s_t6Comparer = EqualityComparer<T6>.Default;
		private static readonly EqualityComparer<T7> s_t7Comparer = EqualityComparer<T7>.Default;

		public OneOf(T1 value)
		{
			Index = 1;
			Option1 = value;
			Option2 = default(T2);
			Option3 = default(T3);
			Option4 = default(T4);
			Option5 = default(T5);
			Option6 = default(T6);
			Option7 = default(T7);
		}
		public OneOf(T2 value)
		{
			Index = 2;
			Option1 = default(T1);
			Option2 = value;
			Option3 = default(T3);
			Option4 = default(T4);
			Option5 = default(T5);
			Option6 = default(T6);
			Option7 = default(T7);
		}
		public OneOf(T3 value)
		{
			Index = 3;
			Option1 = default(T1);
			Option2 = default(T2);
			Option3 = value;
			Option4 = default(T4);
			Option5 = default(T5);
			Option6 = default(T6);
			Option7 = default(T7);
		}
		public OneOf(T4 value)
		{
			Index = 4;
			Option1 = default(T1);
			Option2 = default(T2);
			Option3 = default(T3);
			Option4 = value;
			Option5 = default(T5);
			Option6 = default(T6);
			Option7 = default(T7);
		}
		public OneOf(T5 value)
		{
			Index = 5;
			Option1 = default(T1);
			Option2 = default(T2);
			Option3 = default(T3);
			Option4 = default(T4);
			Option5 = value;
			Option6 = default(T6);
			Option7 = default(T7);
		}
		public OneOf(T6 value)
		{
			Index = 6;
			Option1 = default(T1);
			Option2 = default(T2);
			Option3 = default(T3);
			Option4 = default(T4);
			Option5 = default(T5);
			Option6 = value;
			Option7 = default(T7);
		}
		public OneOf(T7 value)
		{
			Index = 7;
			Option1 = default(T1);
			Option2 = default(T2);
			Option3 = default(T3);
			Option4 = default(T4);
			Option5 = default(T5);
			Option6 = default(T6);
			Option7 = value;
		}

		[DataMember(Order = 0)]
		public readonly int Index;
		[DataMember(EmitDefaultValue = true, Order = 1)]
		public readonly T1? Option1;
		[DataMember(EmitDefaultValue = true, Order = 2)]
		public readonly T2? Option2;
		[DataMember(EmitDefaultValue = true, Order = 3)]
		public readonly T3? Option3;
		[DataMember(EmitDefaultValue = true, Order = 4)]
		public readonly T4? Option4;
		[DataMember(EmitDefaultValue = true, Order = 5)]
		public readonly T5? Option5;
		[DataMember(EmitDefaultValue = true, Order = 6)]
		public readonly T6? Option6;
		[DataMember(EmitDefaultValue = true, Order = 7)]
		public readonly T7? Option7;

		public bool IsEmpty => Index == 0;

		public bool Is1 => Index == 1;
		public T1 As1 => Index == 1 ? Option1! : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5, T6, T7> doesn't contain T1");

		public bool Is2 => Index == 2;
		public T2 As2 => Index == 2 ? Option2! : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5, T6, T7> doesn't contain T2");

		public bool Is3 => Index == 3;
		public T3 As3 => Index == 3 ? Option3! : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5, T6, T7> doesn't contain T3");

		public bool Is4 => Index == 4;
		public T4 As4 => Index == 4 ? Option4! : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5, T6, T7> doesn't contain T4");

		public bool Is5 => Index == 5;
		public T5 As5 => Index == 5 ? Option5! : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5, T6, T7> doesn't contain T5");

		public bool Is6 => Index == 6;
		public T6 As6 => Index == 6 ? Option6! : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5, T6, T7> doesn't contain T6");

		public bool Is7 => Index == 7;
		public T7 As7 => Index == 7 ? Option7! : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5, T6, T7> doesn't contain T7");

		public object Value
		{
			get
			{
				switch (Index)
				{
					case 0:
						throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5, T6, T7> is empty");
					case 1:
						return Option1!;
					case 2:
						return Option2!;
					case 3:
						return Option3!;
					case 4:
						return Option4!;
					case 5:
						return Option5!;
					case 6:
						return Option6!;
					case 7:
						return Option7!;
					default:
						throw new InvalidOperationException($"Undefined behavior for OneOf<T1, T2, T3, T4, T5, T6, T7> with Index {Index}'");
				}
			}
		}

		public void Match(Action<T1> when1, Action<T2> when2, Action<T3> when3, Action<T4> when4, Action<T5> when5, Action<T6> when6, Action<T7> when7, Action? whenEmpty = null)
		{
			switch (Index)
			{
				case 0:
					if (whenEmpty == null)
						throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5, T6, T7> is empty");
					whenEmpty();
					return;
				case 1:
					when1(Option1!);
					return;
				case 2:
					when2(Option2!);
					return;
				case 3:
					when3(Option3!);
					return;
				case 4:
					when4(Option4!);
					return;
				case 5:
					when5(Option5!);
					return;
				case 6:
					when6(Option6!);
					return;
				case 7:
					when7(Option7!);
					return;
				default:
					throw new InvalidOperationException($"Undefined behavior for OneOf<T1, T2, T3, T4, T5, T6, T7> with Index {Index}'");
			}
		}
		public TResult Match<TResult>(Func<T1, TResult> when1, Func<T2, TResult> when2, Func<T3, TResult> when3, Func<T4, TResult> when4, Func<T5, TResult> when5, Func<T6, TResult> when6, Func<T7, TResult> when7, Func<TResult>? whenEmpty = null)
		{
			switch (Index)
			{
				case 0:
					return whenEmpty != null ? whenEmpty() : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5, T6, T7> is empty");
				case 1:
					return when1(Option1!);
				case 2:
					return when2(Option2!);
				case 3:
					return when3(Option3!);
				case 4:
					return when4(Option4!);
				case 5:
					return when5(Option5!);
				case 6:
					return when6(Option6!);
				case 7:
					return when7(Option7!);
				default:
					throw new InvalidOperationException($"Undefined behavior for OneOf<T1, T2, T3, T4, T5, T6, T7> with Index {Index}'");
			}
		}

		public override int GetHashCode()
		{
			var hash = Index.GetHashCode();

			switch (Index)
			{
				case 1:
					hash ^= s_t1Comparer.GetHashCode(As1);
					break;
				case 2:
					hash ^= s_t2Comparer.GetHashCode(As2);
					break;
				case 3:
					hash ^= s_t3Comparer.GetHashCode(As3);
					break;
				case 4:
					hash ^= s_t4Comparer.GetHashCode(As4);
					break;
				case 5:
					hash ^= s_t5Comparer.GetHashCode(As5);
					break;
				case 6:
					hash ^= s_t6Comparer.GetHashCode(As6);
					break;
				case 7:
					hash ^= s_t7Comparer.GetHashCode(As7);
					break;
			}

			return hash;
		}

		public override bool Equals(object obj)
		{
			if (obj is OneOf<T1, T2, T3, T4, T5, T6, T7> oneOfObj)
			{
				return Equals(oneOfObj);
			}

			return false;
		}

		public bool Equals(OneOf<T1, T2, T3, T4, T5, T6, T7> other)
		{
			return Equals(this, other);
		}

		public static bool operator ==(OneOf<T1, T2, T3, T4, T5, T6, T7> a, OneOf<T1, T2, T3, T4, T5, T6, T7> b)
		{
			return Equals(a, b);
		}
		public static bool operator !=(OneOf<T1, T2, T3, T4, T5, T6, T7> a, OneOf<T1, T2, T3, T4, T5, T6, T7> b)
		{
			return !Equals(a, b);
		}

		public static implicit operator OneOf<T1, T2, T3, T4, T5, T6, T7>(T1 value)
		{
			return new OneOf<T1, T2, T3, T4, T5, T6, T7>(value);
		}
		public static implicit operator OneOf<T1, T2, T3, T4, T5, T6, T7>(T2 value)
		{
			return new OneOf<T1, T2, T3, T4, T5, T6, T7>(value);
		}
		public static implicit operator OneOf<T1, T2, T3, T4, T5, T6, T7>(T3 value)
		{
			return new OneOf<T1, T2, T3, T4, T5, T6, T7>(value);
		}
		public static implicit operator OneOf<T1, T2, T3, T4, T5, T6, T7>(T4 value)
		{
			return new OneOf<T1, T2, T3, T4, T5, T6, T7>(value);
		}
		public static implicit operator OneOf<T1, T2, T3, T4, T5, T6, T7>(T5 value)
		{
			return new OneOf<T1, T2, T3, T4, T5, T6, T7>(value);
		}
		public static implicit operator OneOf<T1, T2, T3, T4, T5, T6, T7>(T6 value)
		{
			return new OneOf<T1, T2, T3, T4, T5, T6, T7>(value);
		}
		public static implicit operator OneOf<T1, T2, T3, T4, T5, T6, T7>(T7 value)
		{
			return new OneOf<T1, T2, T3, T4, T5, T6, T7>(value);
		}

		public static explicit operator T1(OneOf<T1, T2, T3, T4, T5, T6, T7> value)
		{
			return value.As1;
		}
		public static explicit operator T2(OneOf<T1, T2, T3, T4, T5, T6, T7> value)
		{
			return value.As2;
		}
		public static explicit operator T3(OneOf<T1, T2, T3, T4, T5, T6, T7> value)
		{
			return value.As3;
		}
		public static explicit operator T4(OneOf<T1, T2, T3, T4, T5, T6, T7> value)
		{
			return value.As4;
		}
		public static explicit operator T5(OneOf<T1, T2, T3, T4, T5, T6, T7> value)
		{
			return value.As5;
		}
		public static explicit operator T6(OneOf<T1, T2, T3, T4, T5, T6, T7> value)
		{
			return value.As6;
		}
		public static explicit operator T7(OneOf<T1, T2, T3, T4, T5, T6, T7> value)
		{
			return value.As7;
		}

		public static bool Equals(OneOf<T1, T2, T3, T4, T5, T6, T7> a, OneOf<T1, T2, T3, T4, T5, T6, T7> b)
		{
			if (a.Index != b.Index)
			{
				return false;
			}

			switch (a.Index)
			{
				case 0:
					return true;
				case 1:
					return s_t1Comparer.Equals(a.As1, b.As1);
				case 2:
					return s_t2Comparer.Equals(a.As2, b.As2);
				case 3:
					return s_t3Comparer.Equals(a.As3, b.As3);
				case 4:
					return s_t4Comparer.Equals(a.As4, b.As4);
				case 5:
					return s_t5Comparer.Equals(a.As5, b.As5);
				case 6:
					return s_t6Comparer.Equals(a.As6, b.As6);
				case 7:
					return s_t7Comparer.Equals(a.As7, b.As7);
				default:
					return false;
			}
		}
	}

	[DataContract]
	public struct OneOf<T1, T2, T3, T4, T5, T6, T7, T8> : IEquatable<OneOf<T1, T2, T3, T4, T5, T6, T7, T8>>
	{
		private static readonly EqualityComparer<T1> s_t1Comparer = EqualityComparer<T1>.Default;
		private static readonly EqualityComparer<T2> s_t2Comparer = EqualityComparer<T2>.Default;
		private static readonly EqualityComparer<T3> s_t3Comparer = EqualityComparer<T3>.Default;
		private static readonly EqualityComparer<T4> s_t4Comparer = EqualityComparer<T4>.Default;
		private static readonly EqualityComparer<T5> s_t5Comparer = EqualityComparer<T5>.Default;
		private static readonly EqualityComparer<T6> s_t6Comparer = EqualityComparer<T6>.Default;
		private static readonly EqualityComparer<T7> s_t7Comparer = EqualityComparer<T7>.Default;
		private static readonly EqualityComparer<T8> s_t8Comparer = EqualityComparer<T8>.Default;

		public OneOf(T1 value)
		{
			Index = 1;
			Option1 = value;
			Option2 = default(T2);
			Option3 = default(T3);
			Option4 = default(T4);
			Option5 = default(T5);
			Option6 = default(T6);
			Option7 = default(T7);
			Option8 = default(T8);
		}
		public OneOf(T2 value)
		{
			Index = 2;
			Option1 = default(T1);
			Option2 = value;
			Option3 = default(T3);
			Option4 = default(T4);
			Option5 = default(T5);
			Option6 = default(T6);
			Option7 = default(T7);
			Option8 = default(T8);
		}
		public OneOf(T3 value)
		{
			Index = 3;
			Option1 = default(T1);
			Option2 = default(T2);
			Option3 = value;
			Option4 = default(T4);
			Option5 = default(T5);
			Option6 = default(T6);
			Option7 = default(T7);
			Option8 = default(T8);
		}
		public OneOf(T4 value)
		{
			Index = 4;
			Option1 = default(T1);
			Option2 = default(T2);
			Option3 = default(T3);
			Option4 = value;
			Option5 = default(T5);
			Option6 = default(T6);
			Option7 = default(T7);
			Option8 = default(T8);
		}
		public OneOf(T5 value)
		{
			Index = 5;
			Option1 = default(T1);
			Option2 = default(T2);
			Option3 = default(T3);
			Option4 = default(T4);
			Option5 = value;
			Option6 = default(T6);
			Option7 = default(T7);
			Option8 = default(T8);
		}
		public OneOf(T6 value)
		{
			Index = 6;
			Option1 = default(T1);
			Option2 = default(T2);
			Option3 = default(T3);
			Option4 = default(T4);
			Option5 = default(T5);
			Option6 = value;
			Option7 = default(T7);
			Option8 = default(T8);
		}
		public OneOf(T7 value)
		{
			Index = 7;
			Option1 = default(T1);
			Option2 = default(T2);
			Option3 = default(T3);
			Option4 = default(T4);
			Option5 = default(T5);
			Option6 = default(T6);
			Option7 = value;
			Option8 = default(T8);
		}
		public OneOf(T8 value)
		{
			Index = 8;
			Option1 = default(T1);
			Option2 = default(T2);
			Option3 = default(T3);
			Option4 = default(T4);
			Option5 = default(T5);
			Option6 = default(T6);
			Option7 = default(T7);
			Option8 = value;
		}

		[DataMember(Order = 0)]
		public readonly int Index;
		[DataMember(EmitDefaultValue = true, Order = 1)]
		public readonly T1? Option1;
		[DataMember(EmitDefaultValue = true, Order = 2)]
		public readonly T2? Option2;
		[DataMember(EmitDefaultValue = true, Order = 3)]
		public readonly T3? Option3;
		[DataMember(EmitDefaultValue = true, Order = 4)]
		public readonly T4? Option4;
		[DataMember(EmitDefaultValue = true, Order = 5)]
		public readonly T5? Option5;
		[DataMember(EmitDefaultValue = true, Order = 6)]
		public readonly T6? Option6;
		[DataMember(EmitDefaultValue = true, Order = 7)]
		public readonly T7? Option7;
		[DataMember(EmitDefaultValue = true, Order = 8)]
		public readonly T8? Option8;

		public bool IsEmpty => Index == 0;

		public bool Is1 => Index == 1;
		public T1 As1 => Index == 1 ? Option1! : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5, T6, T7, T8> doesn't contain T1");

		public bool Is2 => Index == 2;
		public T2 As2 => Index == 2 ? Option2! : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5, T6, T7, T8> doesn't contain T2");

		public bool Is3 => Index == 3;
		public T3 As3 => Index == 3 ? Option3! : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5, T6, T7, T8> doesn't contain T3");

		public bool Is4 => Index == 4;
		public T4 As4 => Index == 4 ? Option4! : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5, T6, T7, T8> doesn't contain T4");

		public bool Is5 => Index == 5;
		public T5 As5 => Index == 5 ? Option5! : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5, T6, T7, T8> doesn't contain T5");

		public bool Is6 => Index == 6;
		public T6 As6 => Index == 6 ? Option6! : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5, T6, T7, T8> doesn't contain T6");

		public bool Is7 => Index == 7;
		public T7 As7 => Index == 7 ? Option7! : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5, T6, T7, T8> doesn't contain T7");

		public bool Is8 => Index == 8;
		public T8 As8 => Index == 8 ? Option8! : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5, T6, T7, T8> doesn't contain T8");

		public object Value
		{
			get
			{
				switch (Index)
				{
					case 0:
						throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5, T6, T7, T8> is empty");
					case 1:
						return Option1!;
					case 2:
						return Option2!;
					case 3:
						return Option3!;
					case 4:
						return Option4!;
					case 5:
						return Option5!;
					case 6:
						return Option6!;
					case 7:
						return Option7!;
					case 8:
						return Option8!;
					default:
						throw new InvalidOperationException($"Undefined behavior for OneOf<T1, T2, T3, T4, T5, T6, T7, T8> with Index {Index}'");
				}
			}
		}

		public void Match(Action<T1> when1, Action<T2> when2, Action<T3> when3, Action<T4> when4, Action<T5> when5, Action<T6> when6, Action<T7> when7, Action<T8> when8, Action? whenEmpty = null)
		{
			switch (Index)
			{
				case 0:
					if (whenEmpty == null)
						throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5, T6, T7, T8> is empty");
					whenEmpty();
					return;
				case 1:
					when1(Option1!);
					return;
				case 2:
					when2(Option2!);
					return;
				case 3:
					when3(Option3!);
					return;
				case 4:
					when4(Option4!);
					return;
				case 5:
					when5(Option5!);
					return;
				case 6:
					when6(Option6!);
					return;
				case 7:
					when7(Option7!);
					return;
				case 8:
					when8(Option8!);
					return;
				default:
					throw new InvalidOperationException($"Undefined behavior for OneOf<T1, T2, T3, T4, T5, T6, T7, T8> with Index {Index}'");
			}
		}
		public TResult Match<TResult>(Func<T1, TResult> when1, Func<T2, TResult> when2, Func<T3, TResult> when3, Func<T4, TResult> when4, Func<T5, TResult> when5, Func<T6, TResult> when6, Func<T7, TResult> when7, Func<T8, TResult> when8, Func<TResult>? whenEmpty = null)
		{
			switch (Index)
			{
				case 0:
					return whenEmpty != null ? whenEmpty() : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5, T6, T7, T8> is empty");
				case 1:
					return when1(Option1!);
				case 2:
					return when2(Option2!);
				case 3:
					return when3(Option3!);
				case 4:
					return when4(Option4!);
				case 5:
					return when5(Option5!);
				case 6:
					return when6(Option6!);
				case 7:
					return when7(Option7!);
				case 8:
					return when8(Option8!);
				default:
					throw new InvalidOperationException($"Undefined behavior for OneOf<T1, T2, T3, T4, T5, T6, T7, T8> with Index {Index}'");
			}
		}

		public override int GetHashCode()
		{
			var hash = Index.GetHashCode();

			switch (Index)
			{
				case 1:
					hash ^= s_t1Comparer.GetHashCode(As1);
					break;
				case 2:
					hash ^= s_t2Comparer.GetHashCode(As2);
					break;
				case 3:
					hash ^= s_t3Comparer.GetHashCode(As3);
					break;
				case 4:
					hash ^= s_t4Comparer.GetHashCode(As4);
					break;
				case 5:
					hash ^= s_t5Comparer.GetHashCode(As5);
					break;
				case 6:
					hash ^= s_t6Comparer.GetHashCode(As6);
					break;
				case 7:
					hash ^= s_t7Comparer.GetHashCode(As7);
					break;
				case 8:
					hash ^= s_t8Comparer.GetHashCode(As8);
					break;
			}

			return hash;
		}

		public override bool Equals(object obj)
		{
			if (obj is OneOf<T1, T2, T3, T4, T5, T6, T7, T8> oneOfObj)
			{
				return Equals(oneOfObj);
			}

			return false;
		}

		public bool Equals(OneOf<T1, T2, T3, T4, T5, T6, T7, T8> other)
		{
			return Equals(this, other);
		}

		public static bool operator ==(OneOf<T1, T2, T3, T4, T5, T6, T7, T8> a, OneOf<T1, T2, T3, T4, T5, T6, T7, T8> b)
		{
			return Equals(a, b);
		}
		public static bool operator !=(OneOf<T1, T2, T3, T4, T5, T6, T7, T8> a, OneOf<T1, T2, T3, T4, T5, T6, T7, T8> b)
		{
			return !Equals(a, b);
		}

		public static implicit operator OneOf<T1, T2, T3, T4, T5, T6, T7, T8>(T1 value)
		{
			return new OneOf<T1, T2, T3, T4, T5, T6, T7, T8>(value);
		}
		public static implicit operator OneOf<T1, T2, T3, T4, T5, T6, T7, T8>(T2 value)
		{
			return new OneOf<T1, T2, T3, T4, T5, T6, T7, T8>(value);
		}
		public static implicit operator OneOf<T1, T2, T3, T4, T5, T6, T7, T8>(T3 value)
		{
			return new OneOf<T1, T2, T3, T4, T5, T6, T7, T8>(value);
		}
		public static implicit operator OneOf<T1, T2, T3, T4, T5, T6, T7, T8>(T4 value)
		{
			return new OneOf<T1, T2, T3, T4, T5, T6, T7, T8>(value);
		}
		public static implicit operator OneOf<T1, T2, T3, T4, T5, T6, T7, T8>(T5 value)
		{
			return new OneOf<T1, T2, T3, T4, T5, T6, T7, T8>(value);
		}
		public static implicit operator OneOf<T1, T2, T3, T4, T5, T6, T7, T8>(T6 value)
		{
			return new OneOf<T1, T2, T3, T4, T5, T6, T7, T8>(value);
		}
		public static implicit operator OneOf<T1, T2, T3, T4, T5, T6, T7, T8>(T7 value)
		{
			return new OneOf<T1, T2, T3, T4, T5, T6, T7, T8>(value);
		}
		public static implicit operator OneOf<T1, T2, T3, T4, T5, T6, T7, T8>(T8 value)
		{
			return new OneOf<T1, T2, T3, T4, T5, T6, T7, T8>(value);
		}

		public static explicit operator T1(OneOf<T1, T2, T3, T4, T5, T6, T7, T8> value)
		{
			return value.As1;
		}
		public static explicit operator T2(OneOf<T1, T2, T3, T4, T5, T6, T7, T8> value)
		{
			return value.As2;
		}
		public static explicit operator T3(OneOf<T1, T2, T3, T4, T5, T6, T7, T8> value)
		{
			return value.As3;
		}
		public static explicit operator T4(OneOf<T1, T2, T3, T4, T5, T6, T7, T8> value)
		{
			return value.As4;
		}
		public static explicit operator T5(OneOf<T1, T2, T3, T4, T5, T6, T7, T8> value)
		{
			return value.As5;
		}
		public static explicit operator T6(OneOf<T1, T2, T3, T4, T5, T6, T7, T8> value)
		{
			return value.As6;
		}
		public static explicit operator T7(OneOf<T1, T2, T3, T4, T5, T6, T7, T8> value)
		{
			return value.As7;
		}
		public static explicit operator T8(OneOf<T1, T2, T3, T4, T5, T6, T7, T8> value)
		{
			return value.As8;
		}

		public static bool Equals(OneOf<T1, T2, T3, T4, T5, T6, T7, T8> a, OneOf<T1, T2, T3, T4, T5, T6, T7, T8> b)
		{
			if (a.Index != b.Index)
			{
				return false;
			}

			switch (a.Index)
			{
				case 0:
					return true;
				case 1:
					return s_t1Comparer.Equals(a.As1, b.As1);
				case 2:
					return s_t2Comparer.Equals(a.As2, b.As2);
				case 3:
					return s_t3Comparer.Equals(a.As3, b.As3);
				case 4:
					return s_t4Comparer.Equals(a.As4, b.As4);
				case 5:
					return s_t5Comparer.Equals(a.As5, b.As5);
				case 6:
					return s_t6Comparer.Equals(a.As6, b.As6);
				case 7:
					return s_t7Comparer.Equals(a.As7, b.As7);
				case 8:
					return s_t8Comparer.Equals(a.As8, b.As8);
				default:
					return false;
			}
		}
	}

	[DataContract]
	public struct OneOf<T1, T2, T3, T4, T5, T6, T7, T8, T9> : IEquatable<OneOf<T1, T2, T3, T4, T5, T6, T7, T8, T9>>
	{
		private static readonly EqualityComparer<T1> s_t1Comparer = EqualityComparer<T1>.Default;
		private static readonly EqualityComparer<T2> s_t2Comparer = EqualityComparer<T2>.Default;
		private static readonly EqualityComparer<T3> s_t3Comparer = EqualityComparer<T3>.Default;
		private static readonly EqualityComparer<T4> s_t4Comparer = EqualityComparer<T4>.Default;
		private static readonly EqualityComparer<T5> s_t5Comparer = EqualityComparer<T5>.Default;
		private static readonly EqualityComparer<T6> s_t6Comparer = EqualityComparer<T6>.Default;
		private static readonly EqualityComparer<T7> s_t7Comparer = EqualityComparer<T7>.Default;
		private static readonly EqualityComparer<T8> s_t8Comparer = EqualityComparer<T8>.Default;
		private static readonly EqualityComparer<T9> s_t9Comparer = EqualityComparer<T9>.Default;

		public OneOf(T1 value)
		{
			Index = 1;
			Option1 = value;
			Option2 = default(T2);
			Option3 = default(T3);
			Option4 = default(T4);
			Option5 = default(T5);
			Option6 = default(T6);
			Option7 = default(T7);
			Option8 = default(T8);
			Option9 = default(T9);
		}
		public OneOf(T2 value)
		{
			Index = 2;
			Option1 = default(T1);
			Option2 = value;
			Option3 = default(T3);
			Option4 = default(T4);
			Option5 = default(T5);
			Option6 = default(T6);
			Option7 = default(T7);
			Option8 = default(T8);
			Option9 = default(T9);
		}
		public OneOf(T3 value)
		{
			Index = 3;
			Option1 = default(T1);
			Option2 = default(T2);
			Option3 = value;
			Option4 = default(T4);
			Option5 = default(T5);
			Option6 = default(T6);
			Option7 = default(T7);
			Option8 = default(T8);
			Option9 = default(T9);
		}
		public OneOf(T4 value)
		{
			Index = 4;
			Option1 = default(T1);
			Option2 = default(T2);
			Option3 = default(T3);
			Option4 = value;
			Option5 = default(T5);
			Option6 = default(T6);
			Option7 = default(T7);
			Option8 = default(T8);
			Option9 = default(T9);
		}
		public OneOf(T5 value)
		{
			Index = 5;
			Option1 = default(T1);
			Option2 = default(T2);
			Option3 = default(T3);
			Option4 = default(T4);
			Option5 = value;
			Option6 = default(T6);
			Option7 = default(T7);
			Option8 = default(T8);
			Option9 = default(T9);
		}
		public OneOf(T6 value)
		{
			Index = 6;
			Option1 = default(T1);
			Option2 = default(T2);
			Option3 = default(T3);
			Option4 = default(T4);
			Option5 = default(T5);
			Option6 = value;
			Option7 = default(T7);
			Option8 = default(T8);
			Option9 = default(T9);
		}
		public OneOf(T7 value)
		{
			Index = 7;
			Option1 = default(T1);
			Option2 = default(T2);
			Option3 = default(T3);
			Option4 = default(T4);
			Option5 = default(T5);
			Option6 = default(T6);
			Option7 = value;
			Option8 = default(T8);
			Option9 = default(T9);
		}
		public OneOf(T8 value)
		{
			Index = 8;
			Option1 = default(T1);
			Option2 = default(T2);
			Option3 = default(T3);
			Option4 = default(T4);
			Option5 = default(T5);
			Option6 = default(T6);
			Option7 = default(T7);
			Option8 = value;
			Option9 = default(T9);
		}
		public OneOf(T9 value)
		{
			Index = 9;
			Option1 = default(T1);
			Option2 = default(T2);
			Option3 = default(T3);
			Option4 = default(T4);
			Option5 = default(T5);
			Option6 = default(T6);
			Option7 = default(T7);
			Option8 = default(T8);
			Option9 = value;
		}

		[DataMember(Order = 0)]
		public readonly int Index;
		[DataMember(EmitDefaultValue = true, Order = 1)]
		public readonly T1? Option1;
		[DataMember(EmitDefaultValue = true, Order = 2)]
		public readonly T2? Option2;
		[DataMember(EmitDefaultValue = true, Order = 3)]
		public readonly T3? Option3;
		[DataMember(EmitDefaultValue = true, Order = 4)]
		public readonly T4? Option4;
		[DataMember(EmitDefaultValue = true, Order = 5)]
		public readonly T5? Option5;
		[DataMember(EmitDefaultValue = true, Order = 6)]
		public readonly T6? Option6;
		[DataMember(EmitDefaultValue = true, Order = 7)]
		public readonly T7? Option7;
		[DataMember(EmitDefaultValue = true, Order = 8)]
		public readonly T8? Option8;
		[DataMember(EmitDefaultValue = true, Order = 9)]
		public readonly T9? Option9;

		public bool IsEmpty => Index == 0;

		public bool Is1 => Index == 1;
		public T1 As1 => Index == 1 ? Option1! : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5, T6, T7, T8, T9> doesn't contain T1");

		public bool Is2 => Index == 2;
		public T2 As2 => Index == 2 ? Option2! : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5, T6, T7, T8, T9> doesn't contain T2");

		public bool Is3 => Index == 3;
		public T3 As3 => Index == 3 ? Option3! : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5, T6, T7, T8, T9> doesn't contain T3");

		public bool Is4 => Index == 4;
		public T4 As4 => Index == 4 ? Option4! : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5, T6, T7, T8, T9> doesn't contain T4");

		public bool Is5 => Index == 5;
		public T5 As5 => Index == 5 ? Option5! : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5, T6, T7, T8, T9> doesn't contain T5");

		public bool Is6 => Index == 6;
		public T6 As6 => Index == 6 ? Option6! : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5, T6, T7, T8, T9> doesn't contain T6");

		public bool Is7 => Index == 7;
		public T7 As7 => Index == 7 ? Option7! : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5, T6, T7, T8, T9> doesn't contain T7");

		public bool Is8 => Index == 8;
		public T8 As8 => Index == 8 ? Option8! : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5, T6, T7, T8, T9> doesn't contain T8");

		public bool Is9 => Index == 9;
		public T9 As9 => Index == 9 ? Option9! : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5, T6, T7, T8, T9> doesn't contain T9");

		public object Value
		{
			get
			{
				switch (Index)
				{
					case 0:
						throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5, T6, T7, T8, T9> is empty");
					case 1:
						return Option1!;
					case 2:
						return Option2!;
					case 3:
						return Option3!;
					case 4:
						return Option4!;
					case 5:
						return Option5!;
					case 6:
						return Option6!;
					case 7:
						return Option7!;
					case 8:
						return Option8!;
					case 9:
						return Option9!;
					default:
						throw new InvalidOperationException($"Undefined behavior for OneOf<T1, T2, T3, T4, T5, T6, T7, T8, T9> with Index {Index}'");
				}
			}
		}

		public void Match(Action<T1> when1, Action<T2> when2, Action<T3> when3, Action<T4> when4, Action<T5> when5, Action<T6> when6, Action<T7> when7, Action<T8> when8, Action<T9> when9, Action? whenEmpty = null)
		{
			switch (Index)
			{
				case 0:
					if (whenEmpty == null)
						throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5, T6, T7, T8, T9> is empty");
					whenEmpty();
					return;
				case 1:
					when1(Option1!);
					return;
				case 2:
					when2(Option2!);
					return;
				case 3:
					when3(Option3!);
					return;
				case 4:
					when4(Option4!);
					return;
				case 5:
					when5(Option5!);
					return;
				case 6:
					when6(Option6!);
					return;
				case 7:
					when7(Option7!);
					return;
				case 8:
					when8(Option8!);
					return;
				case 9:
					when9(Option9!);
					return;
				default:
					throw new InvalidOperationException($"Undefined behavior for OneOf<T1, T2, T3, T4, T5, T6, T7, T8, T9> with Index {Index}'");
			}
		}
		public TResult Match<TResult>(Func<T1, TResult> when1, Func<T2, TResult> when2, Func<T3, TResult> when3, Func<T4, TResult> when4, Func<T5, TResult> when5, Func<T6, TResult> when6, Func<T7, TResult> when7, Func<T8, TResult> when8, Func<T9, TResult> when9, Func<TResult>? whenEmpty = null)
		{
			switch (Index)
			{
				case 0:
					return whenEmpty != null ? whenEmpty() : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5, T6, T7, T8, T9> is empty");
				case 1:
					return when1(Option1!);
				case 2:
					return when2(Option2!);
				case 3:
					return when3(Option3!);
				case 4:
					return when4(Option4!);
				case 5:
					return when5(Option5!);
				case 6:
					return when6(Option6!);
				case 7:
					return when7(Option7!);
				case 8:
					return when8(Option8!);
				case 9:
					return when9(Option9!);
				default:
					throw new InvalidOperationException($"Undefined behavior for OneOf<T1, T2, T3, T4, T5, T6, T7, T8, T9> with Index {Index}'");
			}
		}

		public override int GetHashCode()
		{
			var hash = Index.GetHashCode();

			switch (Index)
			{
				case 1:
					hash ^= s_t1Comparer.GetHashCode(As1);
					break;
				case 2:
					hash ^= s_t2Comparer.GetHashCode(As2);
					break;
				case 3:
					hash ^= s_t3Comparer.GetHashCode(As3);
					break;
				case 4:
					hash ^= s_t4Comparer.GetHashCode(As4);
					break;
				case 5:
					hash ^= s_t5Comparer.GetHashCode(As5);
					break;
				case 6:
					hash ^= s_t6Comparer.GetHashCode(As6);
					break;
				case 7:
					hash ^= s_t7Comparer.GetHashCode(As7);
					break;
				case 8:
					hash ^= s_t8Comparer.GetHashCode(As8);
					break;
				case 9:
					hash ^= s_t9Comparer.GetHashCode(As9);
					break;
			}

			return hash;
		}

		public override bool Equals(object obj)
		{
			if (obj is OneOf<T1, T2, T3, T4, T5, T6, T7, T8, T9> oneOfObj)
			{
				return Equals(oneOfObj);
			}

			return false;
		}

		public bool Equals(OneOf<T1, T2, T3, T4, T5, T6, T7, T8, T9> other)
		{
			return Equals(this, other);
		}

		public static bool operator ==(OneOf<T1, T2, T3, T4, T5, T6, T7, T8, T9> a, OneOf<T1, T2, T3, T4, T5, T6, T7, T8, T9> b)
		{
			return Equals(a, b);
		}
		public static bool operator !=(OneOf<T1, T2, T3, T4, T5, T6, T7, T8, T9> a, OneOf<T1, T2, T3, T4, T5, T6, T7, T8, T9> b)
		{
			return !Equals(a, b);
		}

		public static implicit operator OneOf<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T1 value)
		{
			return new OneOf<T1, T2, T3, T4, T5, T6, T7, T8, T9>(value);
		}
		public static implicit operator OneOf<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T2 value)
		{
			return new OneOf<T1, T2, T3, T4, T5, T6, T7, T8, T9>(value);
		}
		public static implicit operator OneOf<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T3 value)
		{
			return new OneOf<T1, T2, T3, T4, T5, T6, T7, T8, T9>(value);
		}
		public static implicit operator OneOf<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T4 value)
		{
			return new OneOf<T1, T2, T3, T4, T5, T6, T7, T8, T9>(value);
		}
		public static implicit operator OneOf<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T5 value)
		{
			return new OneOf<T1, T2, T3, T4, T5, T6, T7, T8, T9>(value);
		}
		public static implicit operator OneOf<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T6 value)
		{
			return new OneOf<T1, T2, T3, T4, T5, T6, T7, T8, T9>(value);
		}
		public static implicit operator OneOf<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T7 value)
		{
			return new OneOf<T1, T2, T3, T4, T5, T6, T7, T8, T9>(value);
		}
		public static implicit operator OneOf<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T8 value)
		{
			return new OneOf<T1, T2, T3, T4, T5, T6, T7, T8, T9>(value);
		}
		public static implicit operator OneOf<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T9 value)
		{
			return new OneOf<T1, T2, T3, T4, T5, T6, T7, T8, T9>(value);
		}

		public static explicit operator T1(OneOf<T1, T2, T3, T4, T5, T6, T7, T8, T9> value)
		{
			return value.As1;
		}
		public static explicit operator T2(OneOf<T1, T2, T3, T4, T5, T6, T7, T8, T9> value)
		{
			return value.As2;
		}
		public static explicit operator T3(OneOf<T1, T2, T3, T4, T5, T6, T7, T8, T9> value)
		{
			return value.As3;
		}
		public static explicit operator T4(OneOf<T1, T2, T3, T4, T5, T6, T7, T8, T9> value)
		{
			return value.As4;
		}
		public static explicit operator T5(OneOf<T1, T2, T3, T4, T5, T6, T7, T8, T9> value)
		{
			return value.As5;
		}
		public static explicit operator T6(OneOf<T1, T2, T3, T4, T5, T6, T7, T8, T9> value)
		{
			return value.As6;
		}
		public static explicit operator T7(OneOf<T1, T2, T3, T4, T5, T6, T7, T8, T9> value)
		{
			return value.As7;
		}
		public static explicit operator T8(OneOf<T1, T2, T3, T4, T5, T6, T7, T8, T9> value)
		{
			return value.As8;
		}
		public static explicit operator T9(OneOf<T1, T2, T3, T4, T5, T6, T7, T8, T9> value)
		{
			return value.As9;
		}

		public static bool Equals(OneOf<T1, T2, T3, T4, T5, T6, T7, T8, T9> a, OneOf<T1, T2, T3, T4, T5, T6, T7, T8, T9> b)
		{
			if (a.Index != b.Index)
			{
				return false;
			}

			switch (a.Index)
			{
				case 0:
					return true;
				case 1:
					return s_t1Comparer.Equals(a.As1, b.As1);
				case 2:
					return s_t2Comparer.Equals(a.As2, b.As2);
				case 3:
					return s_t3Comparer.Equals(a.As3, b.As3);
				case 4:
					return s_t4Comparer.Equals(a.As4, b.As4);
				case 5:
					return s_t5Comparer.Equals(a.As5, b.As5);
				case 6:
					return s_t6Comparer.Equals(a.As6, b.As6);
				case 7:
					return s_t7Comparer.Equals(a.As7, b.As7);
				case 8:
					return s_t8Comparer.Equals(a.As8, b.As8);
				case 9:
					return s_t9Comparer.Equals(a.As9, b.As9);
				default:
					return false;
			}
		}
	}
}

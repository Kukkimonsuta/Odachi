using System;
using System.Runtime.Serialization;

namespace Odachi.Extensions.Primitives
{
	[DataContract]
	public struct OneOf<T1, T2>
	{
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

		[DataMember]
		public readonly int Index;
		[DataMember(EmitDefaultValue = false)]
		public readonly T1 Option1;
		[DataMember(EmitDefaultValue = false)]
		public readonly T2 Option2;

		public bool IsEmpty => Index == 0;

		public bool Is1 => Index == 1;
		public T1 As1 => Index == 1 ? Option1 : throw new InvalidOperationException("OneOf<T1, T2> doesn't contain T1");

		public bool Is2 => Index == 2;
		public T2 As2 => Index == 2 ? Option2 : throw new InvalidOperationException("OneOf<T1, T2> doesn't contain T2");

		public TResult Match<TResult>(Func<T1, TResult> when1, Func<T2, TResult> when2, Func<TResult> whenEmpty = null)
		{
			switch (Index)
			{
				case 0:
					return whenEmpty != null ? whenEmpty() : throw new InvalidOperationException($"OneOf<T1, T2> is empty");
				case 1:
					return when1(Option1);
				case 2:
					return when2(Option2);
				default:
					throw new InvalidOperationException($"Undefined behavior for OneOf<T1, T2> with Index {Index}'");
			}
		}

		public static implicit operator OneOf<T1, T2>(T1 value)
		{
			return new OneOf<T1, T2>(value);
		}
		public static implicit operator OneOf<T1, T2>(T2 value)
		{
			return new OneOf<T1, T2>(value);
		}
	}

	[DataContract]
	public struct OneOf<T1, T2, T3>
	{
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

		[DataMember]
		public readonly int Index;
		[DataMember(EmitDefaultValue = false)]
		public readonly T1 Option1;
		[DataMember(EmitDefaultValue = false)]
		public readonly T2 Option2;
		[DataMember(EmitDefaultValue = false)]
		public readonly T3 Option3;

		public bool IsEmpty => Index == 0;

		public bool Is1 => Index == 1;
		public T1 As1 => Index == 1 ? Option1 : throw new InvalidOperationException("OneOf<T1, T2, T3> doesn't contain T1");

		public bool Is2 => Index == 2;
		public T2 As2 => Index == 2 ? Option2 : throw new InvalidOperationException("OneOf<T1, T2, T3> doesn't contain T2");

		public bool Is3 => Index == 3;
		public T3 As3 => Index == 3 ? Option3 : throw new InvalidOperationException("OneOf<T1, T2, T3> doesn't contain T3");

		public TResult Match<TResult>(Func<T1, TResult> when1, Func<T2, TResult> when2, Func<T3, TResult> when3, Func<TResult> whenEmpty = null)
		{
			switch (Index)
			{
				case 0:
					return whenEmpty != null ? whenEmpty() : throw new InvalidOperationException($"OneOf<T1, T2, T3> is empty");
				case 1:
					return when1(Option1);
				case 2:
					return when2(Option2);
				case 3:
					return when3(Option3);
				default:
					throw new InvalidOperationException($"Undefined behavior for OneOf<T1, T2, T3> with Index {Index}'");
			}
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
	}

	[DataContract]
	public struct OneOf<T1, T2, T3, T4>
	{
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

		[DataMember]
		public readonly int Index;
		[DataMember(EmitDefaultValue = false)]
		public readonly T1 Option1;
		[DataMember(EmitDefaultValue = false)]
		public readonly T2 Option2;
		[DataMember(EmitDefaultValue = false)]
		public readonly T3 Option3;
		[DataMember(EmitDefaultValue = false)]
		public readonly T4 Option4;

		public bool IsEmpty => Index == 0;

		public bool Is1 => Index == 1;
		public T1 As1 => Index == 1 ? Option1 : throw new InvalidOperationException("OneOf<T1, T2, T3, T4> doesn't contain T1");

		public bool Is2 => Index == 2;
		public T2 As2 => Index == 2 ? Option2 : throw new InvalidOperationException("OneOf<T1, T2, T3, T4> doesn't contain T2");

		public bool Is3 => Index == 3;
		public T3 As3 => Index == 3 ? Option3 : throw new InvalidOperationException("OneOf<T1, T2, T3, T4> doesn't contain T3");

		public bool Is4 => Index == 4;
		public T4 As4 => Index == 4 ? Option4 : throw new InvalidOperationException("OneOf<T1, T2, T3, T4> doesn't contain T4");

		public TResult Match<TResult>(Func<T1, TResult> when1, Func<T2, TResult> when2, Func<T3, TResult> when3, Func<T4, TResult> when4, Func<TResult> whenEmpty = null)
		{
			switch (Index)
			{
				case 0:
					return whenEmpty != null ? whenEmpty() : throw new InvalidOperationException($"OneOf<T1, T2, T3, T4> is empty");
				case 1:
					return when1(Option1);
				case 2:
					return when2(Option2);
				case 3:
					return when3(Option3);
				case 4:
					return when4(Option4);
				default:
					throw new InvalidOperationException($"Undefined behavior for OneOf<T1, T2, T3, T4> with Index {Index}'");
			}
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
	}

	[DataContract]
	public struct OneOf<T1, T2, T3, T4, T5>
	{
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

		[DataMember]
		public readonly int Index;
		[DataMember(EmitDefaultValue = false)]
		public readonly T1 Option1;
		[DataMember(EmitDefaultValue = false)]
		public readonly T2 Option2;
		[DataMember(EmitDefaultValue = false)]
		public readonly T3 Option3;
		[DataMember(EmitDefaultValue = false)]
		public readonly T4 Option4;
		[DataMember(EmitDefaultValue = false)]
		public readonly T5 Option5;

		public bool IsEmpty => Index == 0;

		public bool Is1 => Index == 1;
		public T1 As1 => Index == 1 ? Option1 : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5> doesn't contain T1");

		public bool Is2 => Index == 2;
		public T2 As2 => Index == 2 ? Option2 : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5> doesn't contain T2");

		public bool Is3 => Index == 3;
		public T3 As3 => Index == 3 ? Option3 : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5> doesn't contain T3");

		public bool Is4 => Index == 4;
		public T4 As4 => Index == 4 ? Option4 : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5> doesn't contain T4");

		public bool Is5 => Index == 5;
		public T5 As5 => Index == 5 ? Option5 : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5> doesn't contain T5");

		public TResult Match<TResult>(Func<T1, TResult> when1, Func<T2, TResult> when2, Func<T3, TResult> when3, Func<T4, TResult> when4, Func<T5, TResult> when5, Func<TResult> whenEmpty = null)
		{
			switch (Index)
			{
				case 0:
					return whenEmpty != null ? whenEmpty() : throw new InvalidOperationException($"OneOf<T1, T2, T3, T4, T5> is empty");
				case 1:
					return when1(Option1);
				case 2:
					return when2(Option2);
				case 3:
					return when3(Option3);
				case 4:
					return when4(Option4);
				case 5:
					return when5(Option5);
				default:
					throw new InvalidOperationException($"Undefined behavior for OneOf<T1, T2, T3, T4, T5> with Index {Index}'");
			}
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
	}

	[DataContract]
	public struct OneOf<T1, T2, T3, T4, T5, T6>
	{
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

		[DataMember]
		public readonly int Index;
		[DataMember(EmitDefaultValue = false)]
		public readonly T1 Option1;
		[DataMember(EmitDefaultValue = false)]
		public readonly T2 Option2;
		[DataMember(EmitDefaultValue = false)]
		public readonly T3 Option3;
		[DataMember(EmitDefaultValue = false)]
		public readonly T4 Option4;
		[DataMember(EmitDefaultValue = false)]
		public readonly T5 Option5;
		[DataMember(EmitDefaultValue = false)]
		public readonly T6 Option6;

		public bool IsEmpty => Index == 0;

		public bool Is1 => Index == 1;
		public T1 As1 => Index == 1 ? Option1 : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5, T6> doesn't contain T1");

		public bool Is2 => Index == 2;
		public T2 As2 => Index == 2 ? Option2 : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5, T6> doesn't contain T2");

		public bool Is3 => Index == 3;
		public T3 As3 => Index == 3 ? Option3 : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5, T6> doesn't contain T3");

		public bool Is4 => Index == 4;
		public T4 As4 => Index == 4 ? Option4 : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5, T6> doesn't contain T4");

		public bool Is5 => Index == 5;
		public T5 As5 => Index == 5 ? Option5 : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5, T6> doesn't contain T5");

		public bool Is6 => Index == 6;
		public T6 As6 => Index == 6 ? Option6 : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5, T6> doesn't contain T6");

		public TResult Match<TResult>(Func<T1, TResult> when1, Func<T2, TResult> when2, Func<T3, TResult> when3, Func<T4, TResult> when4, Func<T5, TResult> when5, Func<T6, TResult> when6, Func<TResult> whenEmpty = null)
		{
			switch (Index)
			{
				case 0:
					return whenEmpty != null ? whenEmpty() : throw new InvalidOperationException($"OneOf<T1, T2, T3, T4, T5, T6> is empty");
				case 1:
					return when1(Option1);
				case 2:
					return when2(Option2);
				case 3:
					return when3(Option3);
				case 4:
					return when4(Option4);
				case 5:
					return when5(Option5);
				case 6:
					return when6(Option6);
				default:
					throw new InvalidOperationException($"Undefined behavior for OneOf<T1, T2, T3, T4, T5, T6> with Index {Index}'");
			}
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
	}

	[DataContract]
	public struct OneOf<T1, T2, T3, T4, T5, T6, T7>
	{
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

		[DataMember]
		public readonly int Index;
		[DataMember(EmitDefaultValue = false)]
		public readonly T1 Option1;
		[DataMember(EmitDefaultValue = false)]
		public readonly T2 Option2;
		[DataMember(EmitDefaultValue = false)]
		public readonly T3 Option3;
		[DataMember(EmitDefaultValue = false)]
		public readonly T4 Option4;
		[DataMember(EmitDefaultValue = false)]
		public readonly T5 Option5;
		[DataMember(EmitDefaultValue = false)]
		public readonly T6 Option6;
		[DataMember(EmitDefaultValue = false)]
		public readonly T7 Option7;

		public bool IsEmpty => Index == 0;

		public bool Is1 => Index == 1;
		public T1 As1 => Index == 1 ? Option1 : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5, T6, T7> doesn't contain T1");

		public bool Is2 => Index == 2;
		public T2 As2 => Index == 2 ? Option2 : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5, T6, T7> doesn't contain T2");

		public bool Is3 => Index == 3;
		public T3 As3 => Index == 3 ? Option3 : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5, T6, T7> doesn't contain T3");

		public bool Is4 => Index == 4;
		public T4 As4 => Index == 4 ? Option4 : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5, T6, T7> doesn't contain T4");

		public bool Is5 => Index == 5;
		public T5 As5 => Index == 5 ? Option5 : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5, T6, T7> doesn't contain T5");

		public bool Is6 => Index == 6;
		public T6 As6 => Index == 6 ? Option6 : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5, T6, T7> doesn't contain T6");

		public bool Is7 => Index == 7;
		public T7 As7 => Index == 7 ? Option7 : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5, T6, T7> doesn't contain T7");

		public TResult Match<TResult>(Func<T1, TResult> when1, Func<T2, TResult> when2, Func<T3, TResult> when3, Func<T4, TResult> when4, Func<T5, TResult> when5, Func<T6, TResult> when6, Func<T7, TResult> when7, Func<TResult> whenEmpty = null)
		{
			switch (Index)
			{
				case 0:
					return whenEmpty != null ? whenEmpty() : throw new InvalidOperationException($"OneOf<T1, T2, T3, T4, T5, T6, T7> is empty");
				case 1:
					return when1(Option1);
				case 2:
					return when2(Option2);
				case 3:
					return when3(Option3);
				case 4:
					return when4(Option4);
				case 5:
					return when5(Option5);
				case 6:
					return when6(Option6);
				case 7:
					return when7(Option7);
				default:
					throw new InvalidOperationException($"Undefined behavior for OneOf<T1, T2, T3, T4, T5, T6, T7> with Index {Index}'");
			}
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
	}

	[DataContract]
	public struct OneOf<T1, T2, T3, T4, T5, T6, T7, T8>
	{
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

		[DataMember]
		public readonly int Index;
		[DataMember(EmitDefaultValue = false)]
		public readonly T1 Option1;
		[DataMember(EmitDefaultValue = false)]
		public readonly T2 Option2;
		[DataMember(EmitDefaultValue = false)]
		public readonly T3 Option3;
		[DataMember(EmitDefaultValue = false)]
		public readonly T4 Option4;
		[DataMember(EmitDefaultValue = false)]
		public readonly T5 Option5;
		[DataMember(EmitDefaultValue = false)]
		public readonly T6 Option6;
		[DataMember(EmitDefaultValue = false)]
		public readonly T7 Option7;
		[DataMember(EmitDefaultValue = false)]
		public readonly T8 Option8;

		public bool IsEmpty => Index == 0;

		public bool Is1 => Index == 1;
		public T1 As1 => Index == 1 ? Option1 : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5, T6, T7, T8> doesn't contain T1");

		public bool Is2 => Index == 2;
		public T2 As2 => Index == 2 ? Option2 : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5, T6, T7, T8> doesn't contain T2");

		public bool Is3 => Index == 3;
		public T3 As3 => Index == 3 ? Option3 : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5, T6, T7, T8> doesn't contain T3");

		public bool Is4 => Index == 4;
		public T4 As4 => Index == 4 ? Option4 : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5, T6, T7, T8> doesn't contain T4");

		public bool Is5 => Index == 5;
		public T5 As5 => Index == 5 ? Option5 : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5, T6, T7, T8> doesn't contain T5");

		public bool Is6 => Index == 6;
		public T6 As6 => Index == 6 ? Option6 : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5, T6, T7, T8> doesn't contain T6");

		public bool Is7 => Index == 7;
		public T7 As7 => Index == 7 ? Option7 : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5, T6, T7, T8> doesn't contain T7");

		public bool Is8 => Index == 8;
		public T8 As8 => Index == 8 ? Option8 : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5, T6, T7, T8> doesn't contain T8");

		public TResult Match<TResult>(Func<T1, TResult> when1, Func<T2, TResult> when2, Func<T3, TResult> when3, Func<T4, TResult> when4, Func<T5, TResult> when5, Func<T6, TResult> when6, Func<T7, TResult> when7, Func<T8, TResult> when8, Func<TResult> whenEmpty = null)
		{
			switch (Index)
			{
				case 0:
					return whenEmpty != null ? whenEmpty() : throw new InvalidOperationException($"OneOf<T1, T2, T3, T4, T5, T6, T7, T8> is empty");
				case 1:
					return when1(Option1);
				case 2:
					return when2(Option2);
				case 3:
					return when3(Option3);
				case 4:
					return when4(Option4);
				case 5:
					return when5(Option5);
				case 6:
					return when6(Option6);
				case 7:
					return when7(Option7);
				case 8:
					return when8(Option8);
				default:
					throw new InvalidOperationException($"Undefined behavior for OneOf<T1, T2, T3, T4, T5, T6, T7, T8> with Index {Index}'");
			}
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
	}

	[DataContract]
	public struct OneOf<T1, T2, T3, T4, T5, T6, T7, T8, T9>
	{
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

		[DataMember]
		public readonly int Index;
		[DataMember(EmitDefaultValue = false)]
		public readonly T1 Option1;
		[DataMember(EmitDefaultValue = false)]
		public readonly T2 Option2;
		[DataMember(EmitDefaultValue = false)]
		public readonly T3 Option3;
		[DataMember(EmitDefaultValue = false)]
		public readonly T4 Option4;
		[DataMember(EmitDefaultValue = false)]
		public readonly T5 Option5;
		[DataMember(EmitDefaultValue = false)]
		public readonly T6 Option6;
		[DataMember(EmitDefaultValue = false)]
		public readonly T7 Option7;
		[DataMember(EmitDefaultValue = false)]
		public readonly T8 Option8;
		[DataMember(EmitDefaultValue = false)]
		public readonly T9 Option9;

		public bool IsEmpty => Index == 0;

		public bool Is1 => Index == 1;
		public T1 As1 => Index == 1 ? Option1 : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5, T6, T7, T8, T9> doesn't contain T1");

		public bool Is2 => Index == 2;
		public T2 As2 => Index == 2 ? Option2 : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5, T6, T7, T8, T9> doesn't contain T2");

		public bool Is3 => Index == 3;
		public T3 As3 => Index == 3 ? Option3 : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5, T6, T7, T8, T9> doesn't contain T3");

		public bool Is4 => Index == 4;
		public T4 As4 => Index == 4 ? Option4 : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5, T6, T7, T8, T9> doesn't contain T4");

		public bool Is5 => Index == 5;
		public T5 As5 => Index == 5 ? Option5 : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5, T6, T7, T8, T9> doesn't contain T5");

		public bool Is6 => Index == 6;
		public T6 As6 => Index == 6 ? Option6 : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5, T6, T7, T8, T9> doesn't contain T6");

		public bool Is7 => Index == 7;
		public T7 As7 => Index == 7 ? Option7 : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5, T6, T7, T8, T9> doesn't contain T7");

		public bool Is8 => Index == 8;
		public T8 As8 => Index == 8 ? Option8 : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5, T6, T7, T8, T9> doesn't contain T8");

		public bool Is9 => Index == 9;
		public T9 As9 => Index == 9 ? Option9 : throw new InvalidOperationException("OneOf<T1, T2, T3, T4, T5, T6, T7, T8, T9> doesn't contain T9");

		public TResult Match<TResult>(Func<T1, TResult> when1, Func<T2, TResult> when2, Func<T3, TResult> when3, Func<T4, TResult> when4, Func<T5, TResult> when5, Func<T6, TResult> when6, Func<T7, TResult> when7, Func<T8, TResult> when8, Func<T9, TResult> when9, Func<TResult> whenEmpty = null)
		{
			switch (Index)
			{
				case 0:
					return whenEmpty != null ? whenEmpty() : throw new InvalidOperationException($"OneOf<T1, T2, T3, T4, T5, T6, T7, T8, T9> is empty");
				case 1:
					return when1(Option1);
				case 2:
					return when2(Option2);
				case 3:
					return when3(Option3);
				case 4:
					return when4(Option4);
				case 5:
					return when5(Option5);
				case 6:
					return when6(Option6);
				case 7:
					return when7(Option7);
				case 8:
					return when8(Option8);
				case 9:
					return when9(Option9);
				default:
					throw new InvalidOperationException($"Undefined behavior for OneOf<T1, T2, T3, T4, T5, T6, T7, T8, T9> with Index {Index}'");
			}
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
	}
}

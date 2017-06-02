namespace Odachi.Validation
{
	/// <summary>
	/// Either value or validation state.
	/// </summary>
	public struct ValueOrState<TValue>
	{
		public TValue Value { get; private set; }

		public ValidationState State { get; private set; }

		#region Static members

		public static implicit operator ValueOrState<TValue>(Validator validator)
		{
			return new ValueOrState<TValue>()
			{
				State = validator.State,
			};
		}

		public static implicit operator ValueOrState<TValue>(TValue result)
		{
			return new ValueOrState<TValue>()
			{
				Value = result,
			};
		}

		#endregion
	}
}

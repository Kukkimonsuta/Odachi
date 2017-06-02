namespace Odachi.Validation
{
	/// <summary>
	/// Either value or validation state.
	/// </summary>
	public struct ValueOrState<TValue>
	{
		public ValueOrState(TValue value)
		{
			Value = value;
			State = null;
		}
		public ValueOrState(ValidationState state)
		{
			Value = default(TValue);
			State = state;
		}

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

		public static implicit operator ValueOrState<TValue>(ValidationState state)
		{
			return new ValueOrState<TValue>()
			{
				State = state,
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

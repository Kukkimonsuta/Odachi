namespace Odachi.Validation
{
	/// <summary>
	/// Either value or validation state.
	/// </summary>
	public struct ValidationOr<TValue>
	{
		public ValidationOr(TValue value)
		{
			Validation = null;
			Value = value;
		}
		public ValidationOr(ValidationState validation)
		{
			Validation = validation;
			Value = default(TValue);
		}

		public ValidationState Validation { get; private set; }

		public TValue Value { get; private set; }

		#region Static members

		public static implicit operator ValidationOr<TValue>(Validator validator)
		{
			return new ValidationOr<TValue>()
			{
				Validation = validator.State,
			};
		}

		public static implicit operator ValidationOr<TValue>(ValidationState state)
		{
			return new ValidationOr<TValue>()
			{
				Validation = state,
			};
		}

		public static implicit operator ValidationOr<TValue>(TValue result)
		{
			return new ValidationOr<TValue>()
			{
				Value = result,
			};
		}

		#endregion
	}
}

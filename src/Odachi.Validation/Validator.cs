using System;

namespace Odachi.Validation
{
	/// <summary>
	/// Validation state builder.
	/// </summary>
	public class Validator
	{
		public Validator()
		{
			State = new ValidationState();
		}
		public Validator(ValidationState state)
		{
			if (state == null)
				throw new ArgumentNullException(nameof(state));

			State = state;
		}

		public ValidationState State { get; }

		public bool HasWarnings => State.HasWarnings;

		public bool HasErrors => State.HasErrors;

		public Validator Message(string name, ValidationMessage message)
		{
			State.Add(name, message);

			return this;
		}

		public Validator Warning(string name, string message)
		{
			Message(name, new ValidationMessage()
			{
				Level = ValidationLevel.Warning,
				Message = message,
			});

			return this;
		}

		public Validator Error(string name, string message)
		{
			Message(name, new ValidationMessage()
			{
				Level = ValidationLevel.Error,
				Message = message,
			});

			return this;
		}
	}
}

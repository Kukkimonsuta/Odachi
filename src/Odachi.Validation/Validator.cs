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
			State = state ?? throw new ArgumentNullException(nameof(state));
		}

		public ValidationState State { get; }

		public bool HasWarnings => State.HasWarnings;

		public bool HasErrors => State.HasErrors;

		public Validator Message(ValidationMessage message)
		{
			State.Add(message);

			return this;
		}

		public Validator Warning(string key, string message)
		{
			State.Add(new ValidationMessage()
			{
				Key = key,
				Severity = ValidationSeverity.Warning,
				Text = message,
			});

			return this;
		}

		public Validator Error(string key, string message)
		{
			State.Add(new ValidationMessage()
			{
				Key = key,
				Severity = ValidationSeverity.Error,
				Text = message,
			});

			return this;
		}
	}
}

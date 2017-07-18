using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace Odachi.Validation
{
	/// <summary>
	/// Collection of validation messages.
	/// </summary>
	[DataContract]
	public class ValidationState
	{
		public ValidationState()
		{
			State = new Dictionary<string, IList<ValidationMessage>>();
		}
		public ValidationState(IDictionary<string, IList<ValidationMessage>> state)
		{
			if (state == null)
				throw new ArgumentNullException(nameof(state));

			State = state;
		}

		[DataMember]
		public IDictionary<string, IList<ValidationMessage>> State { get; private set; }

		/// <summary>
		/// Returns validation messages for given field name. Returns zero results if there are no results.
		/// </summary>
		public IEnumerable<ValidationMessage> this[string name]
		{
			get
			{
				if (name == null)
					throw new ArgumentNullException(nameof(name));

				return State.TryGetValue(name, out var result) ? result : Enumerable.Empty<ValidationMessage>();
			}
		}

		/// <summary>
		/// Adds message into the dictionary.
		/// </summary>
		public void Add(string name, ValidationMessage message)
		{
			if (!State.TryGetValue(name, out var propertyState))
				State.Add(name, propertyState = new List<ValidationMessage>());

			propertyState.Add(message);
		}

		#region Warnings

		/// <summary>
		/// Adds warning into the dictionary.
		/// </summary>
		public void AddWarning(string name, string message)
		{
			Add(name, new ValidationMessage()
			{
				Level = ValidationLevel.Warning,
				Message = message,
			});
		}

		/// <summary>
		/// Returns whether there are any warnings.
		/// </summary>
		public bool HasWarnings => State.Any(s => s.Value.Any(m => m.Level == ValidationLevel.Error));

		/// <summary>
		/// Returns an warning or null for given field name.
		/// </summary>
		public string GetWarning(string name)
		{
			if (name == null)
				throw new ArgumentNullException(nameof(name));

			if (!State.TryGetValue(name, out var state))
				return null;

			return state.FirstOrDefault().Message;
		}

		/// <summary>
		/// Returns all warnings for given field name.
		/// </summary>
		public IEnumerable<string> GetWarnings(string name)
		{
			if (name == null)
				throw new ArgumentNullException(nameof(name));

			if (!State.TryGetValue(name, out var state))
				return null;

			return state.Select(s => s.Message);
		}

		#endregion

		#region Errors

		/// <summary>
		/// Adds error into the dictionary.
		/// </summary>
		public void AddError(string name, string message)
		{
			Add(name, new ValidationMessage()
			{
				Level = ValidationLevel.Error,
				Message = message,
			});
		}

		/// <summary>
		/// Returns whether there are any errors.
		/// </summary>
		public bool HasErrors => State.Any(s => s.Value.Any(m => m.Level == ValidationLevel.Error));

		/// <summary>
		/// Returns an error or null for given field name.
		/// </summary>
		public string GetError(string name)
		{
			if (name == null)
				throw new ArgumentNullException(nameof(name));

			if (!State.TryGetValue(name, out var state))
				return null;

			return state.FirstOrDefault().Message;
		}

		/// <summary>
		/// Returns all errors for given field name.
		/// </summary>
		public IEnumerable<string> GetErrors(string name)
		{
			if (name == null)
				throw new ArgumentNullException(nameof(name));

			if (!State.TryGetValue(name, out var state))
				return null;

			return state.Select(s => s.Message);
		}

		#endregion
	}
}

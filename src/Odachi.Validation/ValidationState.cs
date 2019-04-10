using System;
using System.Collections;
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
	public class ValidationState : ICollection<ValidationMessage>
	{
		public ValidationState()
		{
			_messages = new Dictionary<string, IList<ValidationMessage>>();
		}

		private IDictionary<string, IList<ValidationMessage>> _messages;

		public int Count => _messages.Sum(p => p.Value.Count);

		/// <summary>
		/// Returns validation messages for given field name. Returns zero results if there are no results.
		/// </summary>
		public IEnumerable<ValidationMessage> this[string key]
		{
			get
			{
				if (key == null)
					throw new ArgumentNullException(nameof(key));

				return _messages.TryGetValue(key, out var result) ? result : Enumerable.Empty<ValidationMessage>();
			}
		}

		/// <summary>
		/// Adds a message for given key.
		/// </summary>
		public void Add(ValidationSeverity severity, string key, string text)
		{
			if (key == null)
				throw new ArgumentNullException(nameof(key));
			if (text == null)
				throw new ArgumentNullException(nameof(text));

			Add(new ValidationMessage()
			{
				Severity = severity,
				Key = key,
				Text = text,
			});
		}
		/// <summary>
		/// Adds a message for given key.
		/// </summary>
		public void Add(ValidationMessage message)
		{
			if (!_messages.TryGetValue(message.Key, out var propertyState))
				_messages.Add(message.Key, propertyState = new List<ValidationMessage>());

			propertyState.Add(message);
		}

		/// <summary>
		/// Determines whether collection contains given message.
		/// </summary>
		public bool Contains(ValidationMessage message)
		{
			return _messages.Any(p => p.Value.Contains(message));
		}

		/// <summary>
		/// Remove all messages for given key.
		/// </summary>
		public void Remove(string key)
		{
			_messages.Remove(key);
		}

		/// <summary>
		/// Remove given message.
		/// </summary>
		public bool Remove(ValidationMessage message)
		{
			if (!_messages.TryGetValue(message.Key, out var propertyState))
				return false;

			return propertyState.Remove(message);
		}

		/// <summary>
		/// Remove all messages.
		/// </summary>
		public void Clear()
		{
			_messages.Clear();
		}

		#region Warnings

		/// <summary>
		/// Returns whether there are any warnings.
		/// </summary>
		public bool HasWarnings => _messages.Any(s => s.Value.Any(m => m.Severity == ValidationSeverity.Warning));

		/// <summary>
		/// Adds warning into the dictionary.
		/// </summary>
		public void AddWarning(string key, string message)
		{
			if (key == null)
				throw new ArgumentNullException(nameof(key));
			if (message == null)
				throw new ArgumentNullException(nameof(message));

			Add(ValidationSeverity.Warning, key, message);
		}

		/// <summary>
		/// Returns all warning messages for given key.
		/// </summary>
		public IEnumerable<ValidationMessage> GetWarningMessages(string key)
		{
			if (key == null)
				throw new ArgumentNullException(nameof(key));

			if (!_messages.TryGetValue(key, out var propertyState))
			{
				yield break;
			}

			foreach (var message in propertyState)
			{
				if (message.Severity != ValidationSeverity.Warning)
				{
					continue;
				}

				yield return message;
			}
		}

		/// <summary>
		/// Returns first warning message or null for given key.
		/// </summary>
		public ValidationMessage GetWarningMessage(string key)
		{
			if (key == null)
				throw new ArgumentNullException(nameof(key));

			return GetWarningMessages(key).FirstOrDefault();
		}

		/// <summary>
		/// Returns first warning message text or null for given key.
		/// </summary>
		public string GetWarningText(string key)
		{
			if (key == null)
				throw new ArgumentNullException(nameof(key));

			return GetWarningMessages(key).FirstOrDefault()?.Text;
		}

		#endregion

		#region Errors

		/// <summary>
		/// Returns whether there are any errors.
		/// </summary>
		public bool HasErrors => _messages.Any(s => s.Value.Any(m => m.Severity == ValidationSeverity.Error));

		/// <summary>
		/// Adds error into the dictionary.
		/// </summary>
		public void AddError(string key, string message)
		{
			if (key == null)
				throw new ArgumentNullException(nameof(key));
			if (message == null)
				throw new ArgumentNullException(nameof(message));

			Add(ValidationSeverity.Error, key, message);
		}

		/// <summary>
		/// Returns all error messages for given key.
		/// </summary>
		public IEnumerable<ValidationMessage> GetErrorMessages(string key)
		{
			if (key == null)
				throw new ArgumentNullException(nameof(key));

			if (!_messages.TryGetValue(key, out var propertyState))
			{
				yield break;
			}

			foreach (var message in propertyState)
			{
				if (message.Severity != ValidationSeverity.Error)
				{
					continue;
				}

				yield return message;
			}
		}

		/// <summary>
		/// Returns first error message or null for given key.
		/// </summary>
		public ValidationMessage GetErrorMessage(string key)
		{
			if (key == null)
				throw new ArgumentNullException(nameof(key));

			return GetErrorMessages(key).FirstOrDefault();
		}

		/// <summary>
		/// Returns first error message text or null for given key.
		/// </summary>
		public string GetErrorText(string key)
		{
			if (key == null)
				throw new ArgumentNullException(nameof(key));

			return GetErrorMessages(key).FirstOrDefault()?.Text;
		}

		#endregion

		#region ICollection

		public void CopyTo(ValidationMessage[] array, int arrayIndex)
		{
			foreach (var pair in _messages)
			{
				foreach (var message in pair.Value)
				{
					array[arrayIndex++] = message;
				}
			}
		}

		bool ICollection<ValidationMessage>.IsReadOnly => false;

		#endregion

		#region IEnumerable

		public IEnumerator<ValidationMessage> GetEnumerator()
		{
			return _messages.SelectMany(p => p.Value).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
	}
}

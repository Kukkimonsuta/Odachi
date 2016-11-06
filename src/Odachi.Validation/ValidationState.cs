using System;
using System.Collections.Generic;

namespace Odachi.Validation
{
	public class ValidationState
	{
		public ValidationState()
		{
			_prefixes = new Stack<string>();
			_errors = new Dictionary<string, ValidationMessage>();
		}

		private Stack<string> _prefixes;
		private IDictionary<string, ValidationMessage> _errors;

		public bool HasErrors => _errors.Count > 0;
		public IEnumerable<ValidationMessage> Errors { get { return _errors.Values; } }

		public IDisposable PushPrefix(string prefix, int index) => PushPrefix(prefix, index.ToString());
		public IDisposable PushPrefix(string prefix, string index) => PushPrefix($"{prefix}[{index}]");
		public IDisposable PushPrefix(string prefix)
		{
			var current = _prefixes.Count > 0 ? $"{_prefixes.Peek()}." : null;

			_prefixes.Push($"{current}{prefix}");

			return new StackPopper<string>(_prefixes);
		}

		public void AddError(string key, string text)
		{
			AddError(new ValidationMessage()
			{
				Key = key,
				Text = text
			});
		}
		public void AddError(ValidationMessage message)
		{
			if (_prefixes.Count > 0)
			{
				if (!string.IsNullOrEmpty(message.Key))
					message.Key = _prefixes.Peek() + "." + message.Key;
				else
					message.Key = _prefixes.Peek();
			}

			if (_errors.ContainsKey(message.Key))
				return;

			_errors.Add(message.Key, message);
		}

		#region Nested type: StackPopper

		private class StackPopper<T> : IDisposable
		{
			public StackPopper(Stack<T> stack)
			{
				_stack = stack;
			}

			private Stack<T> _stack;

			public void Dispose()
			{
				_stack.Pop();
			}
		}

		#endregion
	}
}

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Odachi.Extensions.Reflection.Tests
{
	public class AwaitableTests
	{
		public class CustomAwaiter : INotifyCompletion
		{
			public CustomAwaiter(int dueTime)
			{
				_continuations = new List<Action>();

				_event = new ManualResetEvent(false);
				_timer = new Timer(OnTimer, null, dueTime, Timeout.Infinite);
			}

			private Timer _timer;
			private ManualResetEvent _event;
			private List<Action> _continuations;

			private void OnTimer(object state)
			{
				_event.Set();
				foreach (var continuation in _continuations)
				{
					continuation();
				}
				_continuations.Clear();
			}

			public void OnCompleted(Action continuation)
			{
				if (IsCompleted)
				{
					continuation();
					return;
				}

				_continuations.Add(continuation);
			}

			public bool IsCompleted => _event.WaitOne(0);

			public string GetResult()
			{
				_event.WaitOne(Timeout.Infinite);

				return "result of CustomAwaitable";
			}
		}

		public class CustomAwaitable
		{
			public CustomAwaiter GetAwaiter()
			{
				return new CustomAwaiter(2000);
			}
		}

		public class TestTarget
		{
			public async Task FooTask()
			{
				await Task.Delay(2000);
			}

			public async Task<string> FooTaskOfString()
			{
				await Task.Delay(2000);

				return "result of FooTaskOfString";
			}

			public ValueTask<string> ValueTaskOfString()
			{
				return new ValueTask<string>("result of ValueTask<string>");
			}

			public CustomAwaitable CustomAwaitable()
			{
				return new CustomAwaitable();
			}

			public string NonAwaitable()
			{
				return "result of NonAwaitable";
			}
		}

		[Fact]
		public void Recognizes_awaitable()
		{
			Assert.True(typeof(Task).IsAwaitable());
			Assert.True(typeof(Task<string>).IsAwaitable());
			Assert.True(typeof(ValueTask<string>).IsAwaitable());
			Assert.True(typeof(CustomAwaitable).IsAwaitable());
		}

		[Fact]
		public void Executes_task()
		{
			var target = new TestTarget();
			var method = target.GetType().GetRuntimeMethod("FooTask", Array.Empty<Type>());

			var awaitable = method.InvokeAsync(target, null);

			var result = awaitable.GetAwaiter().GetResult();

			Assert.Equal("System.Threading.Tasks.VoidTaskResult", result.GetType().FullName);
		}

		[Fact]
		public void Executes_task_of_string()
		{
			var target = new TestTarget();
			var method = target.GetType().GetRuntimeMethod("FooTaskOfString", Array.Empty<Type>());

			var awaitable = method.InvokeAsync(target, null);

			var result = awaitable.GetAwaiter().GetResult();

			Assert.Equal("result of FooTaskOfString", result);
		}

		[Fact]
		public void Executes_valuetask_of_string()
		{
			var target = new TestTarget();
			var method = target.GetType().GetRuntimeMethod("ValueTaskOfString", Array.Empty<Type>());

			var awaitable = method.InvokeAsync(target, null);

			var result = awaitable.GetAwaiter().GetResult();

			Assert.Equal("result of ValueTask<string>", result);
		}

		[Fact]
		public void Executes_custom_awaitable()
		{
			var target = new TestTarget();
			var method = target.GetType().GetRuntimeMethod("CustomAwaitable", Array.Empty<Type>());

			var awaitable = method.InvokeAsync(target, null);

			var result = awaitable.GetAwaiter().GetResult();

			Assert.Equal("result of CustomAwaitable", result);
		}

		[Fact]
		public void Executes_non_awaitable()
		{
			var target = new TestTarget();
			var method = target.GetType().GetRuntimeMethod("NonAwaitable", Array.Empty<Type>());

			var awaitable = method.InvokeAsync(target, null);

			var result = awaitable.GetAwaiter().GetResult();

			Assert.Equal("result of NonAwaitable", result);
		}
	}
}

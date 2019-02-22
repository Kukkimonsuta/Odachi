using Odachi.CodeModel.Mapping;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Odachi.CodeModel
{
	public class HintCollection : IDictionary<string, string>
	{
		public HintCollection()
		{
			_store = new Dictionary<string, string>();
		}

		private IDictionary<string, string> _store;

		public string this[string key]
		{
			get => _store.TryGetValue(key, out var v) ? v : null;
			set => _store[key] = value;
		}

		public ICollection<string> Keys => _store.Keys;

		public ICollection<string> Values => _store.Values;

		public int Count => _store.Count;

		public bool IsReadOnly => _store.IsReadOnly;

		public void Add(string key, string value) => _store.Add(key, value);

		public void Add(KeyValuePair<string, string> item) => _store.Add(item);

		public void Clear() => _store.Clear();

		public bool Contains(KeyValuePair<string, string> item) => _store.Contains(item);

		public bool ContainsKey(string key) => _store.ContainsKey(key);

		public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex) => _store.CopyTo(array, arrayIndex);

		public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => _store.GetEnumerator();

		public bool Remove(string key) => _store.Remove(key);

		public bool Remove(KeyValuePair<string, string> item) => _store.Remove(item);

		public bool TryGetValue(string key, out string value) => _store.TryGetValue(key, out value);

		IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_store).GetEnumerator();
	}

	/// <summary>
	/// Base class for all schema building blocks.
	/// </summary>
	public abstract class Fragment
	{
		public string Name { get; set; }

		public IDictionary<string, string> Hints { get; } = new HintCollection();
	}

	/// <summary>
	/// Base class for all types (enums, objects, services).
	/// </summary>
	public abstract class TypeFragment : Fragment
	{
		public string ModuleName { get; set; }
	}
}

using Daylily.CoolQ.Message;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daylily.CoolQ
{
    public struct ValuePair<T>
    {
        public ValuePair(CoolQIdentity identity, T value) : this()
        {
            Identity = identity;
            this.Value = value;
        }

        [JsonProperty("id")]
        public CoolQIdentity Identity { get; set; }
        [JsonProperty("value")]
        public T Value { get; set; }
    }

    public class ForeachClass<T> : IEnumerable<ValuePair<T>>
    {
        public ForeachClass(List<ValuePair<T>> list)
        {
            List = list;
        }
        private List<ValuePair<T>> List { get; }

        public IEnumerator<ValuePair<T>> GetEnumerator()
        {
            return new ListTypeEnumerator(List);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private class ListTypeEnumerator : IEnumerator<ValuePair<T>>
        {
            private int _index;
            private ValuePair<T> _current;
            private readonly List<ValuePair<T>> _list;

            public ListTypeEnumerator(List<ValuePair<T>> list)
            {
                _list = list;
                _index = -1;
            }

            public object Current => _current;

            public bool MoveNext()
            {
                _index++;
                if (_index >= _list.Count) return false;

                _current = new ValuePair<T>(_list[_index].Identity, _list[_index].Value);
                return true;
            }

            public void Reset()
            {
                _index = -1;
                _current = default;
            }

            ValuePair<T> IEnumerator<ValuePair<T>>.Current => _current;

            public void Dispose()
            {
            }
        }
    }

    /// <summary>
    /// This class is thread safe.
    /// </summary>
    public class CoolQIdentityDictionary<T>
    {
        [JsonProperty("collection")]
        private List<ValuePair<T>> List { get; set; } =
            new List<ValuePair<T>>();
        [JsonIgnore]
        private ForeachClass<T> ForEachObject { get; }

        public CoolQIdentityDictionary()
        {
            ForEachObject = new ForeachClass<T>(List);
        }

        public void Foreach(Action<ValuePair<T>> action)
        {
            foreach (var type in ForEachObject)
            {
                action.Invoke(type);
            }
        }

        public void Add(ValuePair<T> item)
        {
            if (!ContainsKey(item.Identity))
            {
                List.Add(new ValuePair<T>(item.Identity, item.Value));
            }
            else
            {
                if (typeof(T).IsSubclassOf(typeof(IList)))
                {
                    foreach (var g in (IList)item.Value)
                    {
                        ((IList)this[item.Identity]).Add(g);
                    }
                }
                else
                    throw new ArgumentOutOfRangeException($"Already contains identity: \"{item.Identity}\"");
            }
        }

        public void Clear()
        {
            List.Clear();
        }

        public bool Contains(ValuePair<T> item)
        {
            if (!ContainsKey(item.Identity)) return false;
            if (!this[item.Identity].Equals(item.Value)) return false;
            return true;
        }

        public void CopyTo(ValuePair<T>[] array, int arrayIndex)
        {
            var o = List.Skip(arrayIndex).ToArray();
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = new ValuePair<T>(o[i].Identity, o[i].Value);
            }
        }

        public bool Remove(ValuePair<T> item)
        {
            if (!Contains(item)) return false;
            return Remove(item.Identity);
        }

        [JsonIgnore]
        public int Count => List.Count;

        [JsonIgnore]
        public bool IsReadOnly => false;

        public void Add(CoolQIdentity key, T value)
        {
            if (ContainsKey(key))
                throw new ArgumentOutOfRangeException($"Already contains identity: \"{key}\"");
            List.Add(new ValuePair<T>(key, value));
        }

        public bool ContainsKey(CoolQIdentity key)
        {
            return List.Any(k => k.Identity == key);
        }

        public bool Remove(CoolQIdentity key)
        {
            if (!ContainsKey(key)) return false;
            try
            {
                List.Remove(List.Single(k => k.Identity == key));
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public bool TryGetValue(CoolQIdentity key, out T value)
        {
            if (ContainsKey(key))
            {
                value = this[key];
                return true;
            }

            value = default;
            return false;
        }

        public T this[CoolQIdentity key]
        {
            get => List.Single(k => k.Identity == key).Value;
            set => throw new NotSupportedException();
        }

        [JsonIgnore]
        public ICollection<CoolQIdentity> Keys => List.Select(k => k.Identity).ToList();
        [JsonIgnore]
        public ICollection<T> Values => List.Select(k => k.Value).ToList();

    }
}

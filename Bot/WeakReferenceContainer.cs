using System;
using System.Collections.Generic;

namespace TemporaryWorkaround.Common
{
    public class WeakReferenceContainer<Tkey, Tval> where Tval : class where Tkey : notnull
    {
        private readonly Dictionary<Tkey, WeakReference<Tval>> _indexes;
        private int _size;

        public WeakReferenceContainer(int size = int.MaxValue)
        {
            _size = size;
            _indexes = new Dictionary<Tkey, WeakReference<Tval>>();
        }
        public void Add(Tkey key, Tval value)
        {
            if (_indexes.ContainsKey(key)) return;

            if (_indexes.Count < _size)
            {
                _indexes.Add(key, new WeakReference<Tval>(value));

                return;
            }

            foreach (KeyValuePair<Tkey, WeakReference<Tval>> v in _indexes)
            {
                if (!v.Value.TryGetTarget(out _))
                {
                    _indexes.Remove(v.Key);
                }
            }

            if (_indexes.Count >= _size) return;
            _indexes.Add(key, new WeakReference<Tval>(value));
        }
        public Tval? Get(Tkey key)
        {
            bool found = _indexes.TryGetValue(key, out WeakReference<Tval>? refValue);
            if (!found) return null;

            refValue!.TryGetTarget(out Tval? value);
            if (!found)
            {
                _indexes.Remove(key);

                return null;
            }

            return value;
        }
        public void Clear()
        {
            foreach (KeyValuePair<Tkey, WeakReference<Tval>> v in _indexes)
            {
                _indexes.Remove(v.Key);
            }
        }
    }
}

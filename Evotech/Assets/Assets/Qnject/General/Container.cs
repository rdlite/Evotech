using System;
using System.Collections.Generic;

namespace Qnject 
{
    public class Container
    {
        private Dictionary<Type, IList<object>> _dependenciesMap;

        public Container()
        {
            _dependenciesMap = new Dictionary<Type, IList<object>>();
        }

        public object Get(Type type)
        {
            IList<object> values = GetDependency(type);

            for (int i = values.Count - 1; 0 <= i; i--)
            {
                object value = values[i];

                if (value != null && !value.Equals(null))
                {
                    return value;
                }
            }

            return default;
        }

        public T Get<T>()
        {
            IList<object> values = GetDependency<T>();

            for (int i = values.Count - 1; 0 <= i; i--)
            {
                object value = values[i];

                if (value != null && !value.Equals(null))
                {
                    return (T)value;
                }
            }

            return default(T);
        }

        public void BindAsSingle<T>(T value)
        {
            GetDependency<T>().Clear();
            GetDependency<T>().Add(value);
        }

        public void BindNonSingle<T>(T value)
        {
            GetDependency<T>().Add(value);
        }

        public void BindRange<T>(IEnumerable<T> collection)
        {
            if (collection == null) throw new ArgumentNullException();

            foreach (T value in collection)
            {
                GetDependency(value.GetType()).Add(value);
            }
        }

        public IList<T> Collect<T>()
        {
            IList<T> returnList = new List<T>();

            if (GetDependency<T>() != null && GetDependency<T>().Count > 0)
            {
                foreach (var value in GetDependency<T>())
                {
                    returnList.Add((T)value);
                }
            }

            return returnList;
        }

        public void Clear()
        {
            foreach (KeyValuePair<Type, IList<object>> item in _dependenciesMap)
            {
                if (item.Value != null)
                {
                    item.Value.Clear();
                }
            }

            _dependenciesMap.Clear();
        }

        private IList<object> GetDependency<T>()
        {
            if (!_dependenciesMap.ContainsKey(typeof(T)))
            {
                _dependenciesMap.Add(typeof(T), new List<object>());
            }

            return _dependenciesMap[typeof(T)];
        }

        private IList<object> GetDependency(Type type)
        {
            if (!_dependenciesMap.ContainsKey(type))
            {
                _dependenciesMap.Add(type, new List<object>());
            }

            return _dependenciesMap[type];
        }
    }
}
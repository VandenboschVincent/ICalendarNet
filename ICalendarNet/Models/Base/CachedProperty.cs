using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICalendarNet.Models.Base
{
    internal struct CachedProperty<T>
    {
        private T _value;
        private bool _initialized;

        // factory takes state explicitly -> can be a static lambda, no capture
        public T Get<TState>(TState state, Func<TState, T> factory)
        {
            if (!_initialized)
            {
                _value = factory(state);
                _initialized = true;
            }
            return _value;
        }

        public void Set(T value, int currentVersion)
        {
            _value = value;
            _initialized = true;
        }
    }
}

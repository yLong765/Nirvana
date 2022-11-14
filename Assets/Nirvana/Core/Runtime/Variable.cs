using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nirvana
{
    [Serializable]
    public abstract class Variable
    {
        [SerializeField] private string _name;

        public string name
        {
            get => _name;
            set => _name = value;
        }

        public object value
        {
            get => GetValue();
            set => SetValue(value);
        }

        public abstract Type type { get; }
        public abstract object GetValue();
        public abstract void SetValue(object value);
    }

    public class Variable<T> : Variable
    {
        [SerializeField] private T _value;

        public override Type type => typeof(T);

        public override object GetValue()
        {
            return _value;
        }

        public override void SetValue(object value)
        {
            _value = (T) value;
        }
    }
}
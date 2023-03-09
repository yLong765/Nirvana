using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nirvana
{
    public abstract class BBVaribale
    {
        public abstract object GetObjValue();
        public abstract void SetObjValue(object value);
        public abstract Type type { get; }
    }

    public class BBVaribale<T> : BBVaribale
    {
        private T _value;

        public T value
        {
            get => _value;
            set => _value = value;
        }
        
        public override object GetObjValue()
        {
            return _value;
        }

        public override void SetObjValue(object newValue)
        {
            _value = (T) newValue;
        }

        public override Type type => typeof(T);
    }
}
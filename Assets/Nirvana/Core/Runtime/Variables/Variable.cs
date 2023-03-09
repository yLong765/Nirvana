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

        /// <summary>
        /// 变量名
        /// </summary>
        public string name
        {
            get => _name;
            set => _name = value;
        }

        /// <summary>
        /// 变量值
        /// </summary>
        public object value
        {
            get => GetValue();
            set => SetValue(value);
        }
        
        public abstract Type type { get; }
        public abstract object GetValue();
        public abstract void SetValue(object value);
    }

    [Serializable]
    public class Variable<T> : Variable
    {
        [SerializeField] private T _value;

        /// <summary>
        /// 变量值
        /// </summary>
        public new T value
        {
            get => _value;
            set => _value = value;
        }

        /// <summary>
        /// 变量类型
        /// </summary>
        public override Type type => typeof(T);

        /// <summary>
        /// 获取变量值（object）
        /// </summary>
        public override object GetValue()
        {
            return _value;
        }

        /// <summary>
        /// 设置变量值（object）
        /// </summary>
        public override void SetValue(object value)
        {
            _value = (T) value;
        }
    }
}
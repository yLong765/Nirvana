using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Nirvana
{
    public abstract class BBVariable
    {
        protected Variable _linkVariable;
        private bool _linkBlackboard;
        private string _linkID;
        private BlackboardSource bbSource;

        public bool linkBlackboard
        {
            get => _linkBlackboard;
            protected set => _linkBlackboard = value;
        }

        public string linkID
        {
            get => _linkID;
            protected set => _linkID = value;
        }

        [JsonIgnore]
        public object value
        {
            get => GetObjValue();
            set => SetObjValue(value);
        }

        public virtual void LinkToBlackboard(Variable variable = null)
        {
            _linkID = variable?.ID;
            _linkVariable = variable;
            linkBlackboard = variable != null;
        }

        public void LinkToBlackboard(bool isLink)
        {
            linkBlackboard = isLink;
        }
        
        [JsonIgnore] public bool isNone => string.IsNullOrEmpty(name);
        public abstract object GetObjValue();
        public abstract void SetObjValue(object value);
        public abstract void Bind(Variable variable);
        [JsonIgnore] public abstract string name { get; }
        [JsonIgnore] public abstract Type type { get; }
    }

    public class BBVariable<T> : BBVariable
    {
        private T _value;
        private Func<T> getter;
        private Action<T> setter;

        [JsonIgnore]
        public new T value
        {
            get
            {
                if (linkBlackboard)
                {
                    if (getter != null) return getter();
                }

                return _value;
            }
            set
            {
                setter?.Invoke(value);
                if (isNone) return;
                _value = value;
            }
        }

        public override void LinkToBlackboard(Variable variable = null)
        {
            base.LinkToBlackboard(variable);
            Bind(variable);
        }

        public override void Bind(Variable variable)
        {
            _value = default;
            if (variable == null)
            {
                getter = null;
                setter = null;
                return;
            }
            
            BindGetter(variable);
            BindSetter(variable);
        }

        private void BindGetter(Variable variable)
        {
            if (variable is Variable<T> varT)
            {
                getter = () => varT.value;
            }
        }
        
        private void BindSetter(Variable variable)
        {
            if (variable is Variable<T> varT)
            {
                setter = newValue => varT.value = newValue;
            }
        }
        
        public override object GetObjValue()
        {
            return _value;
        }

        public override void SetObjValue(object value)
        {
            _value = (T) value;
        }

        public override string name => _linkVariable?.name;
        public override Type type => typeof(T);
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Nirvana
{
    public abstract class BBVar
    {
        private bool _linkBlackboard;
        private string _linkID;
        private BlackboardSource _linkBBSource;
        protected Variable _linkVariable;

        public bool linkBlackboard
        {
            get => _linkBlackboard;
            set => _linkBlackboard = value;
        }

        public string linkID
        {
            get => _linkID;
            set => _linkID = value;
        }

        [JsonIgnore]
        public BlackboardSource linkBBSource
        {
            get => _linkBBSource;
            set
            { 
                if (value != null && !string.IsNullOrEmpty(linkID))
                {
                    var variable = value.GetVariableByID(linkID);
                    _linkVariable = variable;
                    Bind(variable);
                }

                _linkBBSource = value;
            }
        }

        public object value
        {
            get => GetObjValue();
            set => SetObjValue(value);
        }

        public virtual void LinkToBlackboard(BlackboardSource bbSource = null, Variable variable = null)
        {
            _linkBBSource = bbSource;
            _linkID = variable?.ID;
            _linkVariable = variable;
            linkBlackboard = bbSource != null;
        }

        public void LinkToBlackboard(bool isLink)
        {
            linkBlackboard = isLink;
        }
        
        [JsonIgnore] public bool isNone => string.IsNullOrEmpty(name);
        [JsonIgnore] public abstract string name { get; }
        [JsonIgnore] public abstract Type type { get; }
        public abstract object GetObjValue();
        public abstract void SetObjValue(object value);
        public abstract void Bind(Variable variable);
    }

    public class BBVar<T> : BBVar
    {
        private T _value;
        private Func<T> getter;
        private Action<T> setter;
        
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
                if (!linkBlackboard)
                {
                    _value = value;
                }
            }
        }
        

        public override void LinkToBlackboard(BlackboardSource bbSource = null, Variable variable = null)
        {
            base.LinkToBlackboard(bbSource, variable);
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
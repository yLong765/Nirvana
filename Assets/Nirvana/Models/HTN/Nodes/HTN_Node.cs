using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nirvana.HTN
{
    public abstract class HTN_Node : Node
    {
        protected delegate bool PreConditionDelegate();
        private PreConditionDelegate _preConditions;

        protected delegate void EffectDelegate();
        private EffectDelegate _effects;
        
        protected void AddPreCondition(PreConditionDelegate preCondition)
        {
            _preConditions += preCondition;
        }

        protected void AddEffect(EffectDelegate effect)
        {
            _effects += effect;
        }

        #region Abstract

        public abstract bool Action();

        #endregion
    }
}

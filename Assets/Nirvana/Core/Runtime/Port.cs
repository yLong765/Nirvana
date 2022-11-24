using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nirvana
{
    public class Port
    {
        private string _name;
        private Type _type;
        private int _order;

        public string name
        {
            get => _name;
            set => _name = value;
        }

        public Type type
        {
            get => _type;
            set => _type = value;
        }

        public int order
        {
            get => _order;
            set => _order = value;
        }
    }
}
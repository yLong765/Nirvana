using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nirvana
{
    public interface ISerialize
    {
        public string Serialize();
        public void Deserialize(string json);
    }
}
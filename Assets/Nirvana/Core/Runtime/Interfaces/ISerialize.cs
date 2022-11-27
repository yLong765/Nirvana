using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Nirvana
{
    public interface ISerialize
    {
        public string Serialize(Formatting formatting = Formatting.None);
        public void Deserialize(string json);
    }
}
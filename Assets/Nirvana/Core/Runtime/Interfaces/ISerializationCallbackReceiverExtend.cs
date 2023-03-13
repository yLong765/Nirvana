using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Nirvana
{
    public interface ISerializationCallbackReceiverExtend
    {
        public void OnBeforeSerialize();
        public void OnAfterDeserialize();
    }
}
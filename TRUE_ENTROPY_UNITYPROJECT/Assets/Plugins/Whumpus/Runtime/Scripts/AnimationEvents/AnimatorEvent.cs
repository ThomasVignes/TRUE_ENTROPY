using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Whumpus
{
    [System.Serializable]
    public class AnimatorEvent 
    {
        public string Name;
        public UnityEvent Delegate;
    }
}

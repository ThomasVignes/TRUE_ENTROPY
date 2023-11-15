using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Whumpus
{
    public class AnimationEvents : MonoBehaviour
    {
        public List<AnimatorEvent> AnimatorEvents = new List<AnimatorEvent>();
        private Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public void PlayEvent(string name)
        {
            foreach (var e in AnimatorEvents)
            {
                if (e.Name == name)
                {
                    e.Delegate.Invoke();
                    return;
                }
            }
        }

        public void ResetAllTriggers()
        {
            WhumpusUtilities.ResetAllAnimatorTriggers(animator);
        }
    }
}

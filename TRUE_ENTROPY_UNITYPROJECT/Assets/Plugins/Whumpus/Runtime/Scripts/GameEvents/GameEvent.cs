using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Whumpus
{
    [CreateAssetMenu(menuName = "GameEvent")]
    [System.Serializable]
    public class GameEvent : ScriptableObject
    {
        public string Name;
        public GameObject GameObject;
        public Animator Animator;
        public AudioSource AudioSource;
        public UnityEvent Delegate;
        public Transform Target;

        [SerializeReference]
        public List<GameFeedback> Feedbacks = new List<GameFeedback>();


        public IEnumerator Execute()
        {
            foreach (var f in Feedbacks)
            {
                yield return f.Execute(this);
            }
        }
    }
}

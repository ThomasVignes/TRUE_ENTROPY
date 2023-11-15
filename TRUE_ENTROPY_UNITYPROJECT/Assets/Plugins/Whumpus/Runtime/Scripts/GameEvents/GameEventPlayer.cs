using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Whumpus
{
    public class GameEventPlayer : MonoBehaviour
    {
        [Header("Conditions")]
        public bool PlayOnce;
        [SerializeField] private float playDelay;
        public bool PlayOnStart, PlayOnTriggerEnter, PlayOnCollision;
        [SerializeField] private LayerMask layersToHit;
        [SerializeField] private GameObject player;


        [Header("Event Settings")]
        public string EventName;
        [SerializeField] private GameObject gameobject;
        [SerializeField] private GameObject target;
        [SerializeField] private Animator animator;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private UnityEvent delegates;

        private bool played;
        private Coroutine playedEvent;

        private void Start()
        {
            if (PlayOnStart)
            {
                PlayEvent();
            }
        }

        public void PlayEvent()
        {
            if (playedEvent != null)
                StopCoroutine(playedEvent);

            StartCoroutine(PlayerEventCoroutine());
        }

        private IEnumerator PlayerEventCoroutine()
        {
            yield return new WaitForSecondsRealtime(playDelay);

            if (!played)
                GameEventsManager.PlayEvent(EventName, gameobject, target.transform, animator, audioSource, delegates);

            if (PlayOnce)
                played = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (PlayOnTriggerEnter && other.gameObject.layer == WhumpusUtilities.ToLayer(layersToHit))
            {
                PlayEvent();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (PlayOnTriggerEnter && collision.gameObject.layer == WhumpusUtilities.ToLayer(layersToHit))
            {
                PlayEvent();
            }
        }
    }
}

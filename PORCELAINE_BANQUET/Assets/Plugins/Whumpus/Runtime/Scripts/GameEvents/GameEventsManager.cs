using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Whumpus
{
    public class GameEventsManager : MonoBehaviour
    {
        static GameEventsManager singleton;

        public List<GameEvent> GameEvents;

        static Dictionary<string, GameEvent> _events;

        private void Awake()
        {
            singleton = this;

            _events = new Dictionary<string, GameEvent>(GameEvents.Count);

            foreach (var gameEvent in GameEvents)
            {
                _events.Add(gameEvent.Name, gameEvent);
            }
        }

        public static void PlayEvent(string eventName, GameObject gameObject)
        {
            _events[eventName].GameObject = gameObject;
            singleton.StartCoroutine(_events[eventName].Execute());
        }

        #region Overloads
        public static void PlayEvent(string eventName, Animator animator)
        {
            _events[eventName].Animator = animator;
            singleton.StartCoroutine(_events[eventName].Execute());
        }

        public static void PlayEvent(string eventName, AudioSource audioSource)
        {
            _events[eventName].AudioSource = audioSource;
            singleton.StartCoroutine(_events[eventName].Execute());
        }

        public static void PlayEvent(string eventName, GameObject gameObject, Animator animator)
        {
            _events[eventName].GameObject = gameObject;
            _events[eventName].Animator = animator;
            singleton.StartCoroutine(_events[eventName].Execute());
        }

        public static void PlayEvent(string eventName, Animator animator, AudioSource audioSource)
        {
            _events[eventName].Animator = animator;
            _events[eventName].AudioSource = audioSource;
            singleton.StartCoroutine(_events[eventName].Execute());
        }

        public static void PlayEvent(string eventName, GameObject gameObject, Animator animator, AudioSource audioSource, UnityEvent delegates)
        {
            _events[eventName].GameObject = gameObject;
            _events[eventName].AudioSource = audioSource;
            _events[eventName].Animator = animator;
            _events[eventName].Delegate = delegates;
            singleton.StartCoroutine(_events[eventName].Execute());
        }

        public static void PlayEvent(string eventName, GameObject gameObject, Transform target, Animator animator, AudioSource audioSource, UnityEvent delegates)
        {
            if (_events[eventName] != null)
            {
                GameEvent Gevent = CreateInstance(eventName, gameObject, target, animator, audioSource, delegates, _events[eventName].Feedbacks);

                singleton.StartCoroutine(Gevent.Execute());
            }
        }
        #endregion

        public static GameEvent CreateInstance(string eventName, GameObject gameObject, Transform target, Animator animator, AudioSource audioSource, UnityEvent delegates, List<GameFeedback> feedbacks)
        {
            GameEvent o = ScriptableObject.CreateInstance<GameEvent>();
            o.Name = eventName;
            o.GameObject = gameObject;
            o.Target = target;
            o.Animator = animator;
            o.AudioSource = audioSource;
            o.Delegate = delegates;
            o.Feedbacks = feedbacks;

            return o;
        }

    }



    [System.Serializable]
    public class GameFeedback
    {
        public virtual Color ReturnColor()
        {
            return Color.white;
        }
        public virtual IEnumerator Execute(GameEvent gameEvent)
        {
            yield break;
        }
    }

    public class WaitFeedback : GameFeedback
    {
        public bool Realtime;
        public float Timer;

        public override IEnumerator Execute(GameEvent gameEvent)
        {
            if (Realtime)
                yield return new WaitForSecondsRealtime(Timer);
            else
                yield return new WaitForSeconds(Timer);
        }
    }

    public class InstantiateFeedback : GameFeedback
    {
        public GameObject Prefab;

        public override IEnumerator Execute(GameEvent gameEvent)
        {
            GameObject.Instantiate(Prefab, gameEvent.GameObject.transform.position, Quaternion.identity);
            yield break;
        }

        public override Color ReturnColor()
        {
            return Color.blue;
        }
    }

    public class DestroyFeedback : GameFeedback
    {
        public override IEnumerator Execute(GameEvent gameEvent)
        {
            UnityEngine.GameObject.Destroy(gameEvent.GameObject);
            yield break;
        }

        public override Color ReturnColor()
        {
            return Color.red;
        }
    }

    public class SetActiveFeedback : GameFeedback
    {
        public bool Active;

        public override IEnumerator Execute(GameEvent gameEvent)
        {
            gameEvent.GameObject.SetActive(Active);
            yield break;
        }

        public override Color ReturnColor()
        {
            return Color.magenta;
        }
    }

    public class MoveInDirFeedback : GameFeedback
    {
        public Vector3 Dir;
        public float Distance;

        public override IEnumerator Execute(GameEvent gameEvent)
        {
            gameEvent.GameObject.transform.position += Dir.normalized * Distance;
            yield break;
        }

        public override Color ReturnColor()
        {
            return Color.green;
        }
    }

    public class ChangeSceneFeedback : GameFeedback
    {
        public enum ChangeMode
        {
            GoToIndex,
            GoToScene,
            GoNext,
            GoPrevious
        }

        public ChangeMode Mode;

        public int index;
        public string name;


        public override IEnumerator Execute(GameEvent gameEvent)
        {
            switch (Mode)
            {
                case ChangeMode.GoToIndex:
                    SceneManager.LoadScene(index);
                    break;

                case ChangeMode.GoToScene:
                    SceneManager.LoadScene(name);
                    break;

                case ChangeMode.GoNext:
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                    break;

                case ChangeMode.GoPrevious:
                    int previousSceneIndex = SceneManager.GetActiveScene().buildIndex - 1;

                    if (previousSceneIndex < 0)
                        previousSceneIndex = 0;

                    SceneManager.LoadScene(previousSceneIndex);

                    break;
            }
            yield break;
        }

        public override Color ReturnColor()
        {
            return Color.white;
        }
    }

    public class MoveToTargetFeedback : GameFeedback
    {
        public float Speed, Precision;
        public bool PointTowardsTarget;

        public override IEnumerator Execute(GameEvent gameEvent)
        {
            Transform TargetPos = gameEvent.Target;

            Vector3 Dir = (TargetPos.position - gameEvent.GameObject.transform.position).normalized;

            gameEvent.GameObject.transform.position += Dir * Speed * Time.deltaTime;

            if (PointTowardsTarget)
                gameEvent.GameObject.transform.LookAt(TargetPos);

            if (Vector3.Distance(gameEvent.GameObject.transform.position, TargetPos.position) > Precision)
            {
                yield return new WaitForEndOfFrame();
               yield return Execute(gameEvent);
            }
            else
            {
                yield break;
            }
        }

        public override Color ReturnColor()
        {
            return Color.green;
        }
    }

    public class PlayAnimatorFeedback : GameFeedback
    {
        public enum ActionType
        {
            Trigger,
            Bool,
            Int,
            Float
        }

        public ActionType ValueType;
        public string ValueName;
        public bool BoolValue;
        public int IntValue;
        public float FloatValue;

        public override IEnumerator Execute(GameEvent gameEvent)
        {
            Animator animator = gameEvent.Animator;

            switch (ValueType)
            {
                case ActionType.Trigger:
                    animator.SetTrigger(ValueName);
                    break;
                case ActionType.Bool:
                    animator.SetBool(ValueName, BoolValue);
                    break;
                case ActionType.Int:
                    animator.SetInteger(ValueName, IntValue);
                    break;
                case ActionType.Float:
                    animator.SetFloat(ValueName, FloatValue);
                    break;
            }

            yield return null;
        }

        public override Color ReturnColor()
        {
            return Color.cyan;
        }
    }

    public class PlaySoundFromAudioSourceFeedback : GameFeedback
    {
        [Range(0, 1)]
        public float Volume = 1f;

        [Range(0.3f, 3)]
        public float Pitch = 1f;

        public bool Loop;

        public override IEnumerator Execute(GameEvent gameEvent)
        {
            AudioSource source = gameEvent.AudioSource;
            source.volume = Volume;
            source.pitch = Pitch;
            source.loop = Loop;
            source.Play();
            yield return null;
        }

        public override Color ReturnColor()
        {
            return Color.yellow;
        }
    }

    public class PlayDelegateFeedback : GameFeedback
    {
        public override IEnumerator Execute(GameEvent gameEvent)
        {
            gameEvent.Delegate?.Invoke();
            yield return null;
        }

        public override Color ReturnColor()
        {
            return new Color(0.6f, 0.2f, 1f);
        }
    }

    public class PlayGlobalSoundFeedback : GameFeedback
    {
        public string SoundName;

        public override IEnumerator Execute(GameEvent gameEvent)
        {
            EffectsManager.Instance.audioManager.Play(SoundName);
            yield return null;
        }

        public override Color ReturnColor()
        {
            return Color.yellow;
        }
    }

    public class PlayVfxAtFeedback : GameFeedback
    {
        public string VfxName;
        public Vector3 offset;

        public override IEnumerator Execute(GameEvent gameEvent)
        {
            EffectsManager.Instance.vfxManager.PlayFx(VfxName, gameEvent.GameObject.transform.position + offset);
            yield return null;
        }

        public override Color ReturnColor()
        {
            return Color.blue;
        }
    }
}

using UnityEngine.Audio;
using System;
using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public AudioMixer Mixer;
    public Track[] tracks;
    [SerializeField] private string trackLayer;
    public Sound[] sounds;
    public List<GameObject> playingAudio = new List<GameObject>(100);
    public List<GameObject> notPlayingAudio = new List<GameObject>(100);
    public GameObject AudioObject;
    private GameObject TrackObject;

    void Awake()
    {
        if (Mixer == null)
        {
            Debug.LogError("No audioMixer was found, sounds will not be classified");
        }

        foreach (Sound s in sounds)
        {
            GameObject audioObject = Instantiate(AudioObject, transform);
            notPlayingAudio.Add(audioObject);
            s.source = audioObject.GetComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;

            if (Mixer != null)
            {
                if (Mixer.FindMatchingGroups(s.mixerLayer)[0] != null)
                {
                    s.source.outputAudioMixerGroup = Mixer.FindMatchingGroups(s.mixerLayer)[0];
                }
                else
                {
                    Debug.LogError("Couldn't find Mixer layer " + s.mixerLayer + " for sound " + s.name);
                }
            }

            audioObject.GetComponent<AudioObject>().SoundDone += () => playingAudio.Remove(audioObject);
            audioObject.GetComponent<AudioObject>().SoundDone += () => notPlayingAudio.Add(audioObject);
            audioObject.GetComponent<AudioObject>().SoundDone += () => audioObject.SetActive(false);
            audioObject.SetActive(false);

        }

        //Initialize track
        if (tracks.Length > 0)
        {
            TrackObject = new GameObject();
            TrackObject.transform.parent = transform;
            TrackObject.name = "Track";
            TrackObject.AddComponent<AudioSource>();
            TrackObject.GetComponent<AudioSource>().outputAudioMixerGroup = Mixer.FindMatchingGroups(trackLayer)[0];
            TrackObject.GetComponent<AudioSource>().loop = true;
        }
    }

    public void PlayTrack(string name, bool blend)
    {
        Track t = Array.Find(tracks, track => track.name == name);
        if (t != null)
        {
            if (TrackObject.GetComponent<AudioSource>().clip != t.clip)
            {
                var currentTime = TrackObject.GetComponent<AudioSource>().time;

                TrackObject.GetComponent<AudioSource>().clip = t.clip;

                TrackObject.GetComponent<AudioSource>().Play();

                if (blend)
                {
                    TrackObject.GetComponent<AudioSource>().time = currentTime;
                }
                else
                {
                    TrackObject.GetComponent<AudioSource>().time = 0;
                }
            }
        }
        else
        {
            Debug.LogError("No track by the name '" + name + "' found");
        }
    }

    public void StopTrack()
    {
        if (TrackObject.GetComponent<AudioSource>().clip != null)
        {

            TrackObject.GetComponent<AudioSource>().Stop();

        }
        else
        {
            Debug.LogError("No track by the name '" + name + "' found");
        }
    }


    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s != null)
        {
            bool canPlay = false;

            if (s.loop)
            {
                if (!IsPlaying(s.name))
                    canPlay = true;
            }
            else
            {
                canPlay = true;
            }


            if (canPlay)
            {
                GameObject NewAudioObject = null;
                foreach (var audioObject in notPlayingAudio)
                {
                    if (s.source.clip == audioObject.GetComponent<AudioSource>().clip && audioObject.GetComponent<AudioSource>().isPlaying == false)
                    {
                        audioObject.SetActive(true);
                        notPlayingAudio.Remove(audioObject);
                        playingAudio.Add(audioObject);
                        NewAudioObject = audioObject;
                        break;
                    }
                }

                if (NewAudioObject == null)
                {
                    NewAudioObject = Initialize(s);
                }

                if (s == null)
                {
                    Debug.LogError("Sound :" + name + " doesn't exist");
                    return;
                }
                s.source.Play();

                StartCoroutine(NewAudioObject.GetComponent<AudioObject>().SoundPlayed());
            }
        }
        else
        {
            Debug.LogError("No sound by the name '" + name + "' found");
        }
    }

    public void Play(string name, Vector3 point)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s != null)
        {
            GameObject NewAudioObject = null;
            foreach (var audioObject in notPlayingAudio)
            {
                if (s.source.clip == audioObject.GetComponent<AudioSource>().clip && audioObject.GetComponent<AudioSource>().isPlaying == false)
                {
                    audioObject.SetActive(true);
                    notPlayingAudio.Remove(audioObject);
                    playingAudio.Add(audioObject);
                    NewAudioObject = audioObject;
                    break;
                }
            }

            if (NewAudioObject == null)
            {
                NewAudioObject = Initialize(s);
            }

            if (s == null)
            {
                Debug.LogError("Sound :" + name + " doesn't exist");
                return;
            }

            AudioSource.PlayClipAtPoint(s.source.clip, point);

            StartCoroutine(NewAudioObject.GetComponent<AudioObject>().SoundPlayed());
        }
        else
        {
            Debug.LogError("No sound by the name '" + name + "' found");
        }
    }

    public bool IsPlaying(string name)
    {
        foreach (var sound in playingAudio)
        {
            Sound s = Array.Find(sounds, s => s.name == name);
            if (s.name == name)
                return true;
        }

        return false;
    }

    public void StopSound(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogError("Sound :" + name + " doesn't exist");
            return;
        }
        s.source.Stop();
        s.source.gameObject.GetComponent<AudioObject>().SoundDone?.Invoke();
    }

    private GameObject Initialize(Sound s)
    {
        GameObject audioObject = Instantiate(AudioObject, transform);
        notPlayingAudio.Add(audioObject);
        s.source = audioObject.GetComponent<AudioSource>();
        s.source.clip = s.clip;

        s.source.volume = s.volume;
        s.source.pitch = s.pitch;
        s.source.loop = s.loop;

        audioObject.GetComponent<AudioObject>().SoundDone += () => playingAudio.Remove(audioObject);
        audioObject.GetComponent<AudioObject>().SoundDone += () => notPlayingAudio.Add(audioObject);
        audioObject.GetComponent<AudioObject>().SoundDone += () => audioObject.SetActive(false);

        return audioObject;
    }

}

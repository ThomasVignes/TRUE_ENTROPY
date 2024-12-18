using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ThemeManager : MonoBehaviour
{
    [SerializeField] List<Area> areas = new List<Area>();
    [SerializeField] AudioSource overrideAudio, startAudio;
    [SerializeField] string startAreaExperimental;

    string currentArea;
    float currentVolume;
    AudioSource currentAudioSource;

    string currentOverride;
    bool overrideAmbiance;
    float overrideTime;

    public void Init()
    {
        foreach (var area in areas)
        {
            area.Init();
        }

        if (startAudio != null)
            startAudio.Play();
        else if (startAreaExperimental != "")
            NewArea(startAreaExperimental);
    }

    public void NewArea(string areaName)
    {
        if (overrideAmbiance)
            return;

        foreach (var item in areas)
        {
            if (item.Name == areaName)
            {
                item.Music.volume = item.OriginalVolume;

                if (item.Name != currentArea)
                {
                    item.Music.Play();

                    currentAudioSource = item.Music;
                    currentArea = item.Name;
                    currentVolume = currentAudioSource.volume;
                }
            }
            else
                item.Music.Stop();
        }
    }

    public void NewArea(string areaName, float volume)
    {
        if (overrideAmbiance)
            return;

        foreach (var item in areas)
        {
            if (item.Name == areaName)
            {
                if (!item.ImmuneExperimental)
                    item.Music.volume = volume;

                if (item.Name != currentArea)
                {
                    item.Music.Play();

                    currentAudioSource = item.Music;
                    currentArea = item.Name;
                    currentVolume = currentAudioSource.volume;
                }
            }
            else
                item.Music.Stop();
        }
    }

    public void ResumeAmbiance()
    {
        NewArea(currentArea);
    }

    public void StopAmbiance()
    {
        foreach (var item in areas)
        {
            item.Music.Stop();
        }
    }

    public void PlayEndAmbiance()
    {
        overrideAmbiance = true;

        overrideAudio.Play();
    }

    public void OverrideAmbiance(string areaName)
    {
        StopAmbiance();

        overrideAmbiance = true;

        

        foreach (var item in areas)
        {
            if (item.Name == areaName)
            {
                currentOverride = areaName;

                overrideAudio.clip = item.Music.clip;
                overrideAudio.volume = item.Music.volume;
                overrideAudio.loop = item.Music.loop;
                

                overrideAudio.Play();
            }
        }
    }

    public void StopOverride()
    {
        bool same = currentOverride == currentArea;

        if (same)
            overrideTime = overrideAudio.time;

        overrideAudio.Stop();

        overrideAmbiance = false;

        foreach (var item in areas)
        {
            if (item.Name == currentArea)
            {
                item.Music.volume = item.OriginalVolume;
                item.Music.Play();

                currentAudioSource = item.Music;
                currentArea = item.Name;
                currentVolume = currentAudioSource.volume;

                if (same)
                    currentAudioSource.time = overrideTime;
            }
        }
    }

    public void StopOverride(string resumeTheme)
    {
        bool same = currentOverride == resumeTheme;

        if (same)
            overrideTime = overrideAudio.time;

        overrideAudio.Stop();

        overrideAmbiance = false;

        foreach (var item in areas)
        {
            if (item.Name == resumeTheme)
            {
                item.Music.Play();

                currentAudioSource = item.Music;
                currentArea = item.Name;
                currentVolume = currentAudioSource.volume;

                if (same)
                    currentAudioSource.time = overrideTime;
            }
        }
    }

    public void SetAmbianceVolume(float sound)
    {
        if (currentAudioSource != null)
            currentAudioSource.volume = currentVolume * sound;

        if (overrideAmbiance)
            overrideAudio.volume = currentVolume * sound;
    }
}

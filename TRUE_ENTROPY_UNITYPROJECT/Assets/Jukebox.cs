using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jukebox : MonoBehaviour
{
    [SerializeField] List<string> musicNames = new List<string>();

    int current;
    bool active;

    public void Activate()
    {
        if (active)
            return;

        active = true;

        GameManager.Instance.OverrideAmbiance(musicNames[current]);
    }

    public void PlayNext()
    {
        if (!active)
            return;

        current++;

        if (current >= musicNames.Count) 
        { 
            current = 0;
        }

        GameManager.Instance.OverrideAmbiance(musicNames[current]);
    }
}

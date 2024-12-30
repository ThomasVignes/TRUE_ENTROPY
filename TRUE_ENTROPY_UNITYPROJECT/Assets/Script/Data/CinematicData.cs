using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Cinematic")]
public class CinematicData : ScriptableObject
{
    public string ID;
    public string Ambience;
    public float BlackScreenDuration;
    public CinematicLine[] lines;
    public float EndBlackScreenDuration;
    public bool NoEndBlackscreen, NoFadeOut;
    public string ResumeTheme;
}

[System.Serializable]
public class CinematicLine
{
    public string Text;
    public float Duration;
    public PuppetAction[] PuppetActions;
    public int cameraIndex;
    public CameraEffect cameraEffect;
    public string newAmbience;
    public string[] soundEffects;
}

[System.Serializable]
public class PuppetAction
{
    public string PuppetName;
    public string Action;
}

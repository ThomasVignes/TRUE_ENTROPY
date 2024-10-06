using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Dialogue : MonoBehaviour
{
    public string PuppetName;
    public List<Line> Lines = new List<Line>();
}

[System.Serializable]
public class Line
{
    public string Text;
    public string Action;
    public CameraEffect cameraEffect;
    public List<Answer> Answers = new List<Answer>();

}

[System.Serializable]
public class Answer
{
    [Header("General Parameters")]
    public string Text;
    public string ConditionNeeded;
    public bool DialogueEnd;
    public bool HideLeavePrompt;
    public bool BranchToCurrent = true;
    public int BranchIndex;
    public UnityEvent Delegate;

    [Header("Dialogue Jump Parameters")]
    public bool JumpDialogue;
    public int DialogueIndex;


}

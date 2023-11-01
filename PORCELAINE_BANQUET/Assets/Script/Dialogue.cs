using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialogue : MonoBehaviour
{
    public string PuppetName;
    public List<Line> Lines = new List<Line>();
}

[System.Serializable]
public class Line
{
    public string Text;
    public List<Answer> Answers = new List<Answer>();
}

[System.Serializable]
public class Answer
{
    public string Text;
    public bool BranchToCurrent = true;
    public string BranchTo;
}

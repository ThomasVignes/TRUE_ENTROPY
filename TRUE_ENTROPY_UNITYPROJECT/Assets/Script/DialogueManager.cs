using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Whumpus;

public class DialogueManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float delayBeforeDialogue;
    [SerializeField] private float delayBetweenLetters;

    [Header("References")]
    [SerializeField] private List<Puppet> puppets = new List<Puppet>();
    [SerializeField] private List<Dialogue> dialogues = new List<Dialogue>();
    [SerializeField] private GameObject VNInterface, VNScene, Specific, Camera;
    [SerializeField] private TextMeshProUGUI characterDialogue, observationDialogue;
    [SerializeField] private List<GameObject> answerButtons = new List<GameObject>();

    [SerializeField] private List<Transform> puppetPivots = new List<Transform>();
    [SerializeField] private List<Transform> cameraPivots = new List<Transform>();

    private int currentPuppetIndex;
    private GameObject currentPuppet;
    private Animator currentAnimator;
    private int currentDialogue;
    private int currentLine;
    private DialogueBox currentDialogueBox;

    private Vector3 cameraPosOriginal;
    private bool skip;
    private bool writing;
    private bool selecting;
    private bool specific, endSpecific;

    GameManager gm;

    public void Init(GameManager man)
    {
        gm = man;

        foreach (var item in puppets)
        {
            item.OriginalPos = item.VNPuppet.transform.position;
        }

        foreach (var item in answerButtons)
        {
            item.SetActive(false);
        }

        cameraPosOriginal = Camera.transform.position;
    }

    public void Step()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (writing)
                skip = true;

            if (endSpecific)
            {
                EndSpecific();
            }
        }
    }

    #region MainLoop
    public void StartDialogue(string puppet, int index, DialogueBox dialogueBox)
    {
        currentDialogueBox = dialogueBox;

        foreach (var item in puppets)
        {
            if (item.Name == puppet)
            {
                if (currentPuppet != null)
                {
                    puppets[currentPuppetIndex].ResetPos();
                    currentPuppet = null;
                }

                currentPuppetIndex = puppets.IndexOf(item);

                currentPuppet = item.VNPuppet;
                currentPuppet.transform.position = puppetPivots[item.PivotIndex].position;
                currentPuppet.transform.rotation = puppetPivots[item.PivotIndex].rotation;


                Camera.transform.SetParent(cameraPivots[item.CameraIndex], true);

                Camera.transform.position = cameraPivots[item.CameraIndex].position;
                Camera.transform.rotation = cameraPivots[item.CameraIndex].rotation;

                currentAnimator = currentPuppet.GetComponentInChildren<Animator>();
                break;
            }
        }

        currentDialogue = index;

        LoadDialogue(true);
    }

    public void JumpToOtherDialogue(string puppet, int index)
    {
        Puppet newPuppet = null;

        foreach (var item in puppets)
        {
            if (item.Name == puppet)
            {
                newPuppet = item;
                break;
            }
        }

        currentDialogue = 0;
        currentLine = 0;

        if (newPuppet != puppets[currentPuppetIndex])
        {
            currentAnimator.SetTrigger("Reset");
            puppets[currentPuppetIndex].ResetPos();

            StartDialogue(puppet, index, currentDialogueBox);
        }
        else
        {
            if (currentDialogueBox != null)
                currentDialogueBox.End();

            currentDialogueBox = null;

            currentDialogue = index;
            LoadDialogue(true);
        }
    }

    public void LoadDialogue(bool hasDelay)
    {
        foreach (var item in answerButtons)
        {
            item.SetActive(false);
        }

        characterDialogue.text = "";

        StartCoroutine(C_Dialogue(hasDelay));
    }

    IEnumerator C_Dialogue(bool hasDelay)
    {
        VNScene.SetActive(true);
        Line line = dialogues[currentDialogue].Lines[currentLine];

        string text = line.Text;

        if (line.Action != "")
        {
            WhumpusUtilities.ResetAllAnimatorTriggers(currentAnimator);

            currentAnimator.SetTrigger(line.Action);
        }

        if (line.cameraEffect != CameraEffect.None)
        {
            gm.CameraEffectManager.PlayEffect(line.cameraEffect);
        }

        yield return new WaitForSeconds(delayBeforeDialogue);

        VNInterface.SetActive(true);

        characterDialogue.text = "";

        writing = true;

        foreach (char c in text)
        {
            characterDialogue.text += c;

            EffectsManager.Instance.audioManager.Play("SmallClick");

            if (skip)
            {
                break;
            }

            yield return new WaitForSeconds(delayBetweenLetters);
        }

        writing = false;

        if (skip)
        {
            characterDialogue.text = text;
            skip = false;
        }

        yield return new WaitForSeconds(delayBetweenLetters);
         
        for (int i = 0; i < 3; i++)
        {
            if (i < line.Answers.Count)
            {
                if (line.Answers[i].Text != "")
                {
                    string txt = line.Answers[i].Text;

                    if (line.Answers[i].DialogueEnd && !line.Answers[i].JumpDialogue && !line.Answers[i].HideLeavePrompt)
                    {
                        string[] spl = txt.Split('(');

                        if (spl.Length > 1)
                        {
                            if ((spl[1].ToLower())[0] == 'l')
                            {
                                txt = spl[0];
                            }
                        }

                        txt += "(Leave)";
                    }

                    if (line.Answers[i].ConditionNeeded == "")
                    {
                        answerButtons[i].SetActive(true);
                        answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = txt;
                    }
                    else
                    {
                        if (GameManager.Instance.ConditionMet(line.Answers[i].ConditionNeeded))
                        {
                            answerButtons[i].SetActive(true);
                            answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = txt;
                        }
                    }
                }
            }
        }

        selecting = true;
    }

    public void TryEndDialogue()
    {
        VNInterface.SetActive(false);
        VNScene.SetActive(false);
        GameManager.Instance.SetVNMode(false, false);

        if (currentAnimator != null)
            currentAnimator.SetTrigger("Reset");

        if (puppets.Count > 0)
            puppets[currentPuppetIndex].ResetPos();

        Camera.transform.position = cameraPosOriginal;

        currentDialogue = 0;
        currentLine = 0;

        if (currentDialogueBox != null)
        {
            currentDialogueBox.End();
            currentDialogueBox = null;
        }
    }
    #endregion

    public void SelectAnswer(int index)
    {
        if (!selecting)
            return;

        selecting = false;

        EffectsManager.Instance.audioManager.Play("SmallValidate");

        Answer answer = dialogues[currentDialogue].Lines[currentLine].Answers[index];

        answer.Delegate?.Invoke();

        if (answer.JumpDialogue)
        {
            JumpToOtherDialogue(dialogues[answer.DialogueIndex].PuppetName, answer.DialogueIndex);
            return;
        }
            

        if (answer.DialogueEnd)
        {
            TryEndDialogue();
            return;
        }

        if (answer.BranchToCurrent)
            currentLine++;
        else
            currentLine = answer.BranchIndex;

        LoadDialogue(false);
    }

    public void WriteSpecific(string text)
    {
        StartCoroutine(C_Specific(text));
    }

    IEnumerator C_Specific(string text)
    {
        specific = true;

        Specific.SetActive(true);

        observationDialogue.text = "";

        writing = true;

        foreach (char c in text)
        {
            observationDialogue.text += c;

             EffectsManager.Instance.audioManager.Play("SmallClick");

            if (skip)
            {
                break;
            }

            yield return new WaitForSeconds(delayBetweenLetters);
        }

        writing = false;

        if (skip)
        {
            observationDialogue.text = text;
            skip = false;
        }

        endSpecific = true;
    }

    public void EndSpecific()
    {
        specific = false;
        endSpecific = false;

        Specific.SetActive(false);

        GameManager.Instance.EndComment();
    }
}

[System.Serializable]
public class Puppet
{
    public string Name;
    public GameObject VNPuppet;
    public int PivotIndex;
    public int CameraIndex;

    public Vector3 OriginalPos;

    public void ResetPos()
    {
        VNPuppet.transform.position = OriginalPos;
    }
}

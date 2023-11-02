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
    [SerializeField] private GameObject VNInterface, VNScene;
    [SerializeField] private TextMeshProUGUI characterDialogue;
    [SerializeField] private List<GameObject> answerButtons = new List<GameObject>();

    [SerializeField] private Transform puppetPivot;

    private int currentPuppetIndex;
    private GameObject currentPuppet;
    private Animator currentAnimator;
    private int currentDialogue;
    private int currentLine;
    private DialogueBox currentDialogueBox;

    private bool skip;
    private bool writing;
    private bool selecting;

    public void Init()
    {
        foreach (var item in puppets)
        {
            item.OriginalPos = item.VNPuppet.transform.position;
        }

        foreach (var item in answerButtons)
        {
            item.SetActive(false);
        }
    }

    public void Step()
    {
        if (Input.GetKeyDown(KeyCode.Space) && writing)
        {
            skip = true;
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
                currentPuppet.transform.position = puppetPivot.position;
                currentPuppet.transform.rotation = puppetPivot.rotation;

                currentAnimator = currentPuppet.GetComponentInChildren<Animator>();
                break;
            }
        }

        currentDialogue = index;

        LoadDialogue(true);
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

        yield return new WaitForSeconds(delayBeforeDialogue);

        VNInterface.SetActive(true);

        characterDialogue.text = "";

        writing = true;

        foreach (char c in text)
        {
            characterDialogue.text += c;
            
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
                    answerButtons[i].SetActive(true);
                    answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = line.Answers[i].Text;
                }
            }
        }

        selecting = true;
    }

    public void TryEndDialogue()
    {
        VNInterface.SetActive(false);
        VNScene.SetActive(false);
        GameManager.Instance.SetVNMode(false);
        currentAnimator.SetTrigger("Reset");
        puppets[currentPuppetIndex].ResetPos();

        currentDialogue = 0;
        currentLine = 0;

        currentDialogueBox.End();
        currentDialogueBox = null;
    }
    #endregion

    public void SelectAnswer(int index)
    {
        if (!selecting)
            return;

        selecting = false;

        Answer answer = dialogues[currentDialogue].Lines[currentLine].Answers[index];

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
}

[System.Serializable]
public class Puppet
{
    public string Name;
    public GameObject VNPuppet;

    public Vector3 OriginalPos;

    public void ResetPos()
    {
        VNPuppet.transform.position = OriginalPos;
    }
}

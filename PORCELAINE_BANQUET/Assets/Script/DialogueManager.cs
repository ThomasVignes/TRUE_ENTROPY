using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float delayBeforeDialogue;

    [Header("References")]
    [SerializeField] private List<Puppet> puppets = new List<Puppet>();
    [SerializeField] private List<Dialogue> dialogues = new List<Dialogue>();
    [SerializeField] private GameObject VNInterface, VNScene;
    [SerializeField] private TextMeshProUGUI characterDialogue;

    [SerializeField] private Transform puppetPivot;

    private int currentPuppetIndex;
    private GameObject currentPuppet;
    private Animator currentAnimator;
    private int currentDialogue;
    private int currentLine;

    public void Init()
    {
        foreach (var item in puppets)
        {
            item.OriginalPos = item.VNPuppet.transform.position;
        }
    }

    public void StartDialogue(string puppet, int index)
    {
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
                currentAnimator.SetTrigger("Start");
                break;
            }
        }

        StartCoroutine(C_DelayedStart(index));
    }

    IEnumerator C_DelayedStart(int index)
    {
        VNScene.SetActive(true);
        characterDialogue.text = dialogues[currentDialogue].Lines[currentLine].Text;

        yield return new WaitForSeconds(delayBeforeDialogue);

        VNInterface.SetActive(true);
        
        currentDialogue = index;
    }

    public void TryEndDialogue()
    {
        VNInterface.SetActive(false);
        VNScene.SetActive(false);
        puppets[currentPuppetIndex].ResetPos();
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

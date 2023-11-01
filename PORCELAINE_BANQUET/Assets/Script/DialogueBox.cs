using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueBox : Interactable
{
    [Header("DialogueBox Settings (Local)")]
    [SerializeField] private float delayBeforeSwitch;
    [SerializeField] private string puppet;
    [SerializeField] private int dialogueReference;

    public override void Interact()
    {
        base.Interact();

        StartCoroutine(C_SwitchDialogue());
    }


    IEnumerator C_SwitchDialogue()
    {
        yield return new WaitForSeconds(delayBeforeSwitch);

        GameManager.Instance.SetVNMode(true);
        GameManager.Instance.DialogueManager.StartDialogue(puppet, dialogueReference);

    }
}

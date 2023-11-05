using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class DialogueBox : Interactable
{
    [Header("DialogueBox Settings (Local)")]
    public UnityEvent OnDialogueEnd;
    [SerializeField] private bool once;
    [SerializeField] private float delayBeforeDelegates;
    [SerializeField] private float delayBeforeSwitch;
    [SerializeField] private string puppet;
    [SerializeField] private int dialogueReference;

    protected override void InteractEffects()
    {
        StartCoroutine(C_SwitchDialogue());
    }

    public void End()
    {
        StartCoroutine(C_End());
    }

    IEnumerator C_End()
    {
        yield return new WaitForSeconds(delayBeforeDelegates);

        OnDialogueEnd?.Invoke();

        if (once)
            gameObject.SetActive(false);
    }

    IEnumerator C_SwitchDialogue()
    {
        yield return new WaitForSeconds(delayBeforeSwitch);

        GameManager.Instance.SetVNMode(true);
        GameManager.Instance.DialogueManager.StartDialogue(puppet, dialogueReference, this);

    }
}

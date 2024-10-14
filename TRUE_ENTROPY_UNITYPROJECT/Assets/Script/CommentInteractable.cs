using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CommentInteractable : Interactable
{
    [SerializeField] private string comment;
    public UnityEvent OnCommentEnd;

    protected override void InteractEffects()
    {
         GameManager.Instance.WriteComment(comment, this);
    }
}

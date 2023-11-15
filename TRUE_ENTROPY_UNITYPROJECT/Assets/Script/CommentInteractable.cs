using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommentInteractable : Interactable
{
    [SerializeField] private string comment;

    protected override void InteractEffects()
    {
         GameManager.Instance.WriteComment(comment);
    }
}

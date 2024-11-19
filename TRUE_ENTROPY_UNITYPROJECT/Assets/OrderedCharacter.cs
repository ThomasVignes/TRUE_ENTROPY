using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderedCharacter : Character
{
    [SerializeField] GameObject indicator;

    bool selected;

    public bool Selected {  get { return selected; } }

    public override void Init()
    {
        base.Init();

        indicator.SetActive(false);
    }

    public void Select(bool selected)
    {
        this.selected = selected;

        animator.SetBool("Controlled", selected);
    }
}

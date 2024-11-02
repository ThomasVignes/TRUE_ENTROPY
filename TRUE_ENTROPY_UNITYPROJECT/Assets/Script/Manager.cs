using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    protected GameManager gm;

    public virtual void Init(GameManager gameManager)
    {
        gm = gameManager;
    }

    public virtual void Step()
    {

    }

    public virtual void UpdateOnArea()
    {

    }
}

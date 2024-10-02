using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ChapterManagerGeneric : MonoBehaviour
{
    public bool Intro;

    protected GameManager gameManager;

    public virtual void Init(GameManager gameManager)
    {
        this.gameManager = gameManager; 
    }

    public abstract void StartGame();

    public abstract void RestartGame();

    public abstract void IntroStep();

    public abstract void Death(string message);
    public abstract void EndChapter();
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : Manager
{
    [SerializeField] EnemyAI[] enemies;

    public override void Init(GameManager gameManager)
    {
        base.Init(gameManager);

        foreach (EnemyAI enemy in enemies) { enemy.Init(); }
    }
}

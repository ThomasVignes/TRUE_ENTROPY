using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lifeform : MonoBehaviour
{
    int HP;

    public void Hurt()
    {
        Hurt(1);
    }

    public void Hurt(int damage)
    {
        HP -= damage;

        if (HP <= 0)
            Death();

    }

    public void Death()
    {

    }
}

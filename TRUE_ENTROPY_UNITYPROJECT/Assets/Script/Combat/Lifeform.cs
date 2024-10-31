using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Lifeform : MonoBehaviour
{
    public int HP;
    public Character Character;
    public UnityEvent OnDeath;


    bool dead;
    public void Hurt()
    {
        Hurt(1);
    }

    public void Hurt(int damage)
    {
        if (dead)
            return;

        HP -= damage;

        if (HP <= 0)
            Death();

    }

    public void Stun(float stunDamage)
    {
        if (dead)
            return;

        Character.Stun(stunDamage);
    }

    public void Death()
    {
        if (dead)
            return;
        
        OnDeath?.Invoke();

        dead = true;
    }
}

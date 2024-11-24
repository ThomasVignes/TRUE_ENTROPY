using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimbShield : MonoBehaviour
{
    public bool Active = true;
    public int HP;

    public void Absorb(int damage)
    {
        HP -= damage;   

        if (HP <= 0 )
            Active = false;
    }
}

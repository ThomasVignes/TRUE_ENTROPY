using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Whumpus;

public class TargetLimb : MonoBehaviour
{
    public bool Resistant;
    public int HP = 4;
    public RagdollLimb limb;
    public bool HasBeenCut;
    public UnityEvent OnHit, OnCut;

    private void Awake()
    {
        limb = GetComponent<RagdollLimb>();    
    }

    public void Hit(int damage, float force, Vector3 dir)
    {

        //Keep leg cutting ?
        if (!HasBeenCut && ! Resistant)
        {
            HP -= damage;

            if (HP <= 0)
            {
                HasBeenCut = true;
                OnCut.Invoke();

                //HitStop
                //SlowMoEffector.Instance.Hit(limb.rb, force/4, dir);

                //Gore
                //BloodManager.Instance.SpawnBlood(limb.transform.position, limb.transform.parent.position + Vector3.Normalize(limb.transform.position - limb.transform.parent.position) * 0.2f, limb.transform.parent);

                limb.CutLimb();

                /*
                Blood[] bloods = GetComponentsInChildren<Blood>();

                foreach (var item in bloods)
                {
                    Destroy(item.gameObject);
                }
                */
            }
            else
            {
                //HitStop
                //SlowMoEffector.Instance.Hit(limb.rb, force, dir);
            }
        }
        else
        {
            //HitStop
            //SlowMoEffector.Instance.Hit(limb.rb, force, dir);
        }

        OnHit.Invoke();
    }
}

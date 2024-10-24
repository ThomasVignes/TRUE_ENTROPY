using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Florp : MonoBehaviour
{
    [SerializeField] float extensionSpeed;

    bool active;
    float scale;

    private void Update()
    {
        if (!active)
            return;

        if (scale < 1)
            scale += extensionSpeed * Time.deltaTime;
        else
        {
            scale = 1;
            active = false;
        }

        transform.localScale = new Vector3(1, scale, 1);
    }

    [ContextMenu("Trigger")]
    public void Trigger()
    {
        active = true;
        scale = 0;

        transform.localScale = new Vector3(1, scale, 1);

        //EffectsManager.Instance.audioManager.Play("FleshWhip");
    }
}

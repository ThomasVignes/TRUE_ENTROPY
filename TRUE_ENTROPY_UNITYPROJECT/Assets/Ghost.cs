using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Whumpus;
using DG.Tweening;
using UnityEngine.Rendering.Universal;

public class Ghost : MonoBehaviour
{
    [SerializeField] LayerMask PlayerMask;
    [SerializeField] List<SkinnedMeshRenderer> skinnedMeshRenderer = new List<SkinnedMeshRenderer>();
    [SerializeField] List<MeshRenderer> meshRenderer = new List<MeshRenderer>();
    [SerializeField] DecalProjector decal;
    [SerializeField] Animator animator;

    bool fading;
    


    private void OnEnable()
    {
        fading = false;
        foreach (var renderer in skinnedMeshRenderer)
        {
            renderer.material.DOFade(1, 0.01f);
        }

        foreach (var renderer in meshRenderer)
        {
            renderer.material.DOFade(1, 0.01f);
        }

        decal.gameObject.SetActive(true);

        animator.SetFloat("IdleState", Random.Range(0, 3));
    }

    private void OnTriggerEnter(Collider other)
    { 
        if (other.gameObject.layer == WhumpusUtilities.ToLayer(PlayerMask) && !fading)
        {
            fading = true;
            foreach (var renderer in skinnedMeshRenderer)
            {
                renderer.material.DOFade(0, 0.4f);
            }

            foreach (var renderer in meshRenderer)
            {
                renderer.material.DOFade(0, 0.4f);
            }

            decal.gameObject.SetActive(false);
        }
    }
}

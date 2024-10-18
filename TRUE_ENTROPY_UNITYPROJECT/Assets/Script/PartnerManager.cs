using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class PartnerManager : MonoBehaviour
{
    [Header("AI")]
    [SerializeField] float reactionTime;
    [SerializeField] float partnerStopDistance, partnerResumeDistance;
    [SerializeField] float partnerRunDistance, partnerStopRunDistance;

    [Header("References")]
    [SerializeField] GameObject startPartnerPrefab;
    [SerializeField] Transform partnerStart;

    Character partner;
    GameManager gameManager;
    float reactionTimer;
    bool running;

    public Character Partner {  get { return partner; } }   

    public void Init(GameManager gm)
    {
        gameManager = gm;

        if (startPartnerPrefab != null)
        {
            Transform start = partnerStart;

            if (start == null)
                start = gameManager.Player.transform;

            var chara = Instantiate(startPartnerPrefab, start.position, start.rotation);
            partner = chara.GetComponentInChildren<Character>();
        }

        reactionTimer = Time.time + reactionTime;
    }

    public void Step()
    {
        if (partner == null)
            return;

        if (reactionTimer < Time.time)
        {
            var player = gameManager.Player;
            float dist = Vector3.Distance(player.transform.position, new Vector3(partner.transform.position.x, player.transform.position.y, partner.transform.position.z));

            if (dist > partnerResumeDistance)
            {
                partner.SetDestination(player.transform.position);
            }

            RunManagement(dist);

            if (dist < partnerStopDistance)
            {
                partner.Pause();
            }

            reactionTimer = Time.time + reactionTime;
        }
    }

    public void ConstantStep()
    {

    }

    public void SetPartner(Character partner)
    {
        this.partner = partner;
    }

    private void RunManagement(float dist)
    {
        if (dist > partnerRunDistance && !running)
        {
            partner.ToggleRun(true);
            running = true;
        }

        if (dist < partnerStopRunDistance && running)
        {
            partner.ToggleRun(false);
            running = false;
        }
    }
}

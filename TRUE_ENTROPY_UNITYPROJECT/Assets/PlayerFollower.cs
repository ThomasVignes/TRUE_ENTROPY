using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollower : MonoBehaviour
{
    PlayerController player;

    void Update()
    {
        if (player == null)
        {
            player = GameManager.Instance.Player;
        }
        else
        {
            transform.position = player.transform.position;
        }
    }
}

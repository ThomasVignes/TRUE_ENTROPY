using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GhostManager : Manager
{
    [SerializeField] List<GameObject> ghosts = new List<GameObject>();
    
    public void UpdateGhosts()
    {
        int[] rand = new int[] { Random.Range(0, ghosts.Count), Random.Range(0, ghosts.Count) };

        foreach (GameObject ghost in ghosts) 
        { 
            if (rand.Contains(ghosts.IndexOf(ghost))) 
            { 
                ghost.SetActive(true);
            }
            else
            {
                ghost.SetActive(false);
            }
        }
    }
}

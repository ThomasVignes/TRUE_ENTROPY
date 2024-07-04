using System.Collections;
using UnityEngine;

public class HitstopManager : MonoBehaviour
{
    // Variables for hitstop
    private bool isHitstopActive = false;
    [SerializeField] private float hitstopDuration = 0.1f; // Adjust the duration as needed

    // Method to enable hitstop
    public void StartHitstop()
    {
        if (!isHitstopActive)
        {
            Time.timeScale = 0f;
            isHitstopActive = true;
            StartCoroutine(StopTime());
        }
    }

    // Coroutine to resume time after hitstopDuration
    private IEnumerator StopTime()
    {
        yield return new WaitForSecondsRealtime(hitstopDuration);
        Time.timeScale = 1f;
        isHitstopActive = false;
    }
}
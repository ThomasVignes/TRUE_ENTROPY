using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DisclaimerManager : MonoBehaviour
{
    [SerializeField] Toggle copyrightToggle;

    public void UpdateCopyright()
    {
        PersistentData.Instance.CopyrightFree = copyrightToggle.isOn;
    }

    public void Continue()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}

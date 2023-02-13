using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class reload : MonoBehaviour
{
    public void ReloadOnClick()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(0);
    }
}

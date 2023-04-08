using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class reload : MonoBehaviour
{
    public void ReloadOnClick()
    {
        Destroy(GameObject.Find("GameplayManager"));
        SceneManager.LoadScene(0);
        Time.timeScale = 1.0f;
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        
    }
}

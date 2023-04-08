using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class GarageCinematicTimer : MonoBehaviour
{
    private float timer = 0;
    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer >= 30)
        {
            SceneManager.LoadScene(1);
            SceneManager.UnloadSceneAsync(4);
        }

        if(Input.GetKeyUp(KeyCode.Escape))
        {
            SceneManager.LoadScene(1);
            SceneManager.UnloadSceneAsync(4);
        }
    }
}

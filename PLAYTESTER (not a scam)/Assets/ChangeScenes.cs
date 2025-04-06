using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class ChangeScenes : MonoBehaviour
{
    public float changeTime;
    public string mainScene;
    public string sceneName2;


    // Update is called once per frame
    private void Update()
    {
        //hardcoded to time duration of second clip, since this script will only be used for the cutscene

        changeTime -= Time.deltaTime;
        if (changeTime <= 2.85 && changeTime > 0)
        {
            SceneManager.LoadScene(sceneName2);
        }

        else if (changeTime <= 0)
        {
            SceneManager.LoadScene(mainScene);
        }
    }
}

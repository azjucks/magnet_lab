using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSceneManager : MonoBehaviour
{
    
    public void Level_Basile()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    public void Level_Hugo()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(2);
    }

    public void Level_Clement()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }


    public void Level_Maxine()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(3);
    }

    public void Main_Menu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Settings()
    {

    }

}
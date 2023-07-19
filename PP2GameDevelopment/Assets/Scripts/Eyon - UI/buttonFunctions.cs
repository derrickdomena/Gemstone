using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{
    
    public void resume()
    {
        gameManager.instance.stateUnpaused();
    }

    //this will be used next build for now when trying to reload scene it tries to get a player that isnt there 
    //unless its level 1 because thats where the player lives 
    public void restart()
    {
        gameManager.instance.stateUnpaused();

        if (true) //make this false to bring back the player dupe bug :D
        {
            Destroy(gameManager.instance.player);
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void quit()
    {
        Application.Quit();
    }

    
    public void WIN()
    {
        gameManager.instance.stateUnpaused();

        if (true) //make this false to bring back the player dupe bug :D
        {
            Destroy(gameManager.instance.player);
        }
        SceneManager.LoadScene(1);
    }
}

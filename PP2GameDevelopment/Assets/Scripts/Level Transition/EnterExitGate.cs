using System.Collections;
using System.Collections.Generic;
//using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnterExitGate : MonoBehaviour
{
    private int dungeonCount = 0;
    public void Interact()
    {
        gameManager.instance.anim.SetTrigger("FadeOut");
        //if (SceneManager.GetActiveScene().buildIndex != 1)
        //{
        //    if(dungeonCount < 2)
        //    {
        //        SceneManager.LoadScene(1);
        //    }
        //    else SceneManager.LoadScene(3);
        //}
        //else
        //{
        //    dungeonCount++;
        //    SceneManager.LoadScene(3);
        //}
        SceneManager.LoadScene(2);
    }
}

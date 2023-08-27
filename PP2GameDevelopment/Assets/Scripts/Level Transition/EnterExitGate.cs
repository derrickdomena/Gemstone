using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnterExitGate : MonoBehaviour
{

    public void Interact()
    {
        if(gameManager.instance.playerScript.weaponList.Count > 0)
        {
            gameManager.instance.anim.SetTrigger("FadeOut");

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        
    }
}

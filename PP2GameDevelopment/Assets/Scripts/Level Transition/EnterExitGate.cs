using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnterExitGate : MonoBehaviour
{

    public void Interact()
    {
        gameManager.instance.anim.SetTrigger("FadeOut");
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}

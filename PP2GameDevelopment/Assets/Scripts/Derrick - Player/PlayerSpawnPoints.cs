using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnPoints : MonoBehaviour
{
    // Start is called before the first frame update
    public void Start()
    {  
        gameManager.instance.gemCount = gameManager.instance.playerScript.gems;
        gameManager.instance.gemText.text = gameManager.instance.gemCount.ToString();
        //gameManager.instance.player.transform.position = transform.position.normalized;
    }

}

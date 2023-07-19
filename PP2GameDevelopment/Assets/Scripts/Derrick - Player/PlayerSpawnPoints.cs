using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnPoints : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameManager.instance.player.transform.position = transform.position;
        gameManager.instance.gemCount = gameManager.instance.playerScript.gems;
        gameManager.instance.gemText.text = gameManager.instance.gemCount.ToString(); 
    }

}

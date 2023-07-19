using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnPoints : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameManager.instance.player.transform.position = transform.position;
    }

}

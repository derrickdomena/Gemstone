using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    Transform player;
    public Camera cam;

    private void Start()
    {
        player = gameManager.instance.player.transform;
    }

    private void Update()
    {
        if (gameManager.instance.isMiniMap)
        {
            cam.orthographicSize = 100;
        }
        else
        {
            cam.orthographicSize = 25;
        } 
    }

    private void LateUpdate()
    {
        Vector3 newPosition = player.position;
        newPosition.y = transform.position.y;
        transform.position = newPosition;
    }
}

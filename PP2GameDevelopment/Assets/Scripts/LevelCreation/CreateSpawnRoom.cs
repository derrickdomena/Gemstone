using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CreateSpawnRoom : MonoBehaviour
{
    [SerializeField] List<GameObject> spawnRooms = new List<GameObject>();

    void Start()
    {
        int randomRoom = Random.Range(0, spawnRooms.Count);

        // Instantiate the first room at the center of the grid
        Vector2Int center = new Vector2Int(gameManager.instance.level.GetLength(0) / 2, gameManager.instance.level.GetLength(1) / 2);

        if (gameManager.instance.rooms.Count > 0)
        {
            // Instantiate the first room from the list at the transform's position and with the same rotation as the original room
            GameObject room = Instantiate(spawnRooms[randomRoom], transform.position, transform.rotation);

            // Place the first room in the center of the level grid
            gameManager.instance.level[center.x, center.y] = room;
        }
        else
        {
            //Debug.LogWarning("No rooms to instantiate.");
        }
    }
}

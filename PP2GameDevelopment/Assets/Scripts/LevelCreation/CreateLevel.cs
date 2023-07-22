using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CreateLevel : MonoBehaviour
{
    [SerializeField] List<GameObject> rooms = new List<GameObject>();
    [SerializeField] private List<GameObject> nodePositions = new List<GameObject>();

    public GameObject[,] roomsGrid;

    private void Start()
    {
        Generate();
    }
    private void Generate()
    {
        int rotation = 0;

        for (int i = 0; i < nodePositions.Count; i++)
        {
            int randomRoom = Random.Range(0, rooms.Count);


            if (nodePositions[i].name == "RoomPOSNorth")
            {
                rotation = 180;
            }
            else if (nodePositions[i].name == "RoomPOSEast")
            {
                rotation = -90;
            }
            else if (nodePositions[i].name == "RoomPOSSouth")
            {
                rotation = 0;
            }
            else if (nodePositions[i].name == "RoomPOSWest")
            {
                rotation = 90;
            }

            Instantiate(rooms[randomRoom], nodePositions[i].transform.position, Quaternion.Euler(0, rotation, 0));
        }
    }
}

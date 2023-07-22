using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CreateSpawnRoom : MonoBehaviour
{
    [SerializeField] List<GameObject> spawnRooms = new List<GameObject>();

    void Start()
    {
        Generate();
    }

    private void Generate()
    {
        int randomRoom = Random.Range(0, spawnRooms.Count);
        
        Instantiate(spawnRooms[randomRoom], transform.position , transform.rotation);

    }
}

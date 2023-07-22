using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ChunkStats", menuName = "ScriptableObject/ChunkStats", order = 1)]
public class LevelChunks : ScriptableObject
{
    [field: SerializeField] List<GameObject> spawnRooms = new List<GameObject>();

    [field: SerializeField] List<GameObject> shopRooms = new List<GameObject>();

    [field: SerializeField] List<GameObject> sideRooms = new List<GameObject>();
}

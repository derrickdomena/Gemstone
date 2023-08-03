using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class GeneratingNavMesh : MonoBehaviour
{
    [SerializeField] NavMeshSurface navMeshSurface;
    bool navGenerated = false;


    // Update is called once per frame
    void Update()
    {
        if (gameManager.instance.generated == true && !navGenerated)
        {
            navMeshSurface.BuildNavMesh();
            navGenerated = true;
        }
    }
}

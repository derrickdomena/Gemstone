using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpScare : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(-1.9f, 3.3f, -1.47f) * -Time.time;
    }
}

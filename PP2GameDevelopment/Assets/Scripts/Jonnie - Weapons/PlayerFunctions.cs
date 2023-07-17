using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFunctions : MonoBehaviour
{
    public static PlayerFunctions instance;
    void Awake()
    {
        instance = this;
    }
    
}

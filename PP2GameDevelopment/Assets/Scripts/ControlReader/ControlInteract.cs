using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlInteract : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            float interactRange = 3f;
            Collider[] colliderArray = Physics.OverlapSphere(transform.position, interactRange);
            foreach (Collider collider in colliderArray)
            {
                if (collider.TryGetComponent(out ControlSign sign))
                {
                    sign.Interact();
                }
            }
        }
    }
    //Allows other classes to check to see if the sign is nearby to interact with
    public ControlSign GetInteractableObject()
    {
        float interactRange = 3f;
        Collider[] colliderArray = Physics.OverlapSphere(transform.position, interactRange);
        foreach (Collider collider in colliderArray)
        {
            if (collider.TryGetComponent(out ControlSign sign))
            {
                return sign;
            }
        }
        return null;
    }
}

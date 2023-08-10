using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractDungeon : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            float interactRange = 6f;
            Collider[] colliderArray = Physics.OverlapSphere(transform.position, interactRange);
            foreach (Collider collider in colliderArray)
            {
                if (collider.TryGetComponent(out EnterExitGate gate))
                {
                    gate.Interact();
                }
            }
        }
    }
    //Allows other classes to check to see if there is a gate nearby to interact with
    public EnterExitGate GetInteractableObject()
    {
        float interactRange = 6f;
        Collider[] colliderArray = Physics.OverlapSphere(transform.position, interactRange);
        foreach (Collider collider in colliderArray)
        {
            if (collider.TryGetComponent(out EnterExitGate gate))
            {
                return gate;
            }
        }
        return null;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            float interactRange = 10f;
            Collider[] colliderArray = Physics.OverlapSphere(transform.position, interactRange);
            foreach(Collider collider in colliderArray)
            {
                if(collider.TryGetComponent(out NPCInteractable npcInteractable))
                {
                    npcInteractable.Interact();
                }
            }
        }
    }
    //Allows other classes to check to see if there is an NPC nearby to interact with
    public NPCInteractable GetInteractableObject()
    {
        float interactRange = 2f;
        Collider[] colliderArray = Physics.OverlapSphere(transform.position, interactRange);
        foreach (Collider collider in colliderArray)
        {
            if (collider.TryGetComponent(out NPCInteractable npcInteractable))
            {
                return npcInteractable;
            }
        }
        return null;
    }

    //Allows other classes to check to see if there is a gate nearby to interact with
    public GameObject GetInteractableGate()
    {
        float interactRange = 2f;
        Collider[] colliderArray = Physics.OverlapSphere(transform.position, interactRange);
        foreach (Collider collider in colliderArray)
        {
            if (collider.TryGetComponent(out GameObject gate))
            {
                return gate;
            }
        }
        return null;
    }
}

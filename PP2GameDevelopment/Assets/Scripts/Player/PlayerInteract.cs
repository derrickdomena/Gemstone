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
            float interactRange = 6.5f;
            Collider[] colliderArray = Physics.OverlapSphere(transform.position, interactRange);
            foreach(Collider collider in colliderArray)
            {
                if(collider.TryGetComponent(out NPCInteractable npcInteractable))
                {
                    npcInteractable.Interact();
                }
                if (collider.TryGetComponent(out ControlSign sign))
                {
                    sign.Interact();
                }
                if (collider.TryGetComponent(out EnterExitGate gate))
                {
                    gate.Interact();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            float interactRange = 6.5f;
            Collider[] colliderArray = Physics.OverlapSphere(transform.position, interactRange);
            foreach (Collider collider in colliderArray)
            {
                if (collider.TryGetComponent(out NPCInteractable npcInteractable))
                {
                    npcInteractable.Talk();
                }
            }
        }
    }
    //Allows other classes to check to see if there is an NPC nearby to interact with
    public NPCInteractable GetInteractableObject()
    {
        float interactRange = 6.5f;
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
    public EnterExitGate GetInteractObject()
    {
        float interactRange = 3f;
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
    public ControlSign GetInteractable()
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

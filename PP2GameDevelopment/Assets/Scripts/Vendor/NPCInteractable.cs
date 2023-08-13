using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteractable : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public void Interact()
    {
        gameManager.instance.Vendor();
    }

    public void Talk()
    {
        ChatBubble.Create(transform.transform, new Vector3(-.3f, 1.7f, 1f), "Welcome, my name is Bob!");
        animator.SetTrigger("Talk");
    }
}

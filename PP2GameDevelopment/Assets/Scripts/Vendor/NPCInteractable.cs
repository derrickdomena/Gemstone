using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteractable : MonoBehaviour
{
    private Animator animator;
    AudioManager audioManager;
    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        animator = GetComponent<Animator>();
    }
    public void Interact()
    {
        gameManager.instance.Vendor();
    }

    public void Talk()
    {
        audioManager.PlaySFXInteractSounds(audioManager.bobInteractSound);
        ChatBubble.Create(transform.transform, new Vector3(-.3f, 1.7f, 1f), "Welcome, my name is Bob!");
        animator.SetTrigger("Talk");
    }
}

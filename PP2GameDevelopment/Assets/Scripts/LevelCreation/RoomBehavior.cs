using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class RoomBehavior : MonoBehaviour
{
    public float doorSpeed = 1.5f;
    private bool shouldMove = false;
    public GameObject hiddenRoomIcon;
    AudioSource doorAudioSource;

    GameObject doors;
    GameObject dust;
    GameObject spawner;

    Vector3 doorClosePOS;
    Vector3 doorOpenPOS;

    float openDoorThreshold = 0.05f;

    bool played = false;
    bool enemiesTriggered = false;
    bool eventDone = false;
    bool doorsClosed = false;
    bool roomDecremented = false;
    private void Awake()
    {

    }
    void Start()
    {
        GetDust();
        GetDoors();
        GetDoorsPositions();
        GetSpawner();
        doorAudioSource = GetComponent<AudioSource>();
        GetHiddenRoomIcon();
    }

    void Update()
    {
        if (shouldMove && !eventDone)
        {
             
            doors.SetActive(true);
            dust.SetActive(true);

            doors.transform.position = Vector3.Lerp(doors.transform.position, doorClosePOS, Time.deltaTime * doorSpeed);
            if (!doorAudioSource.isPlaying && !played)
            {
                doorAudioSource.Play();
                played = true;
            }
            if (Vector3.Distance(doors.transform.position, doorClosePOS) < 0.252f)
            {
                dust.SetActive(false);
                if (spawner != null)
                {
                    spawner.SetActive(true);
                }
                enemiesTriggered = true;
            }
            if (Vector3.Distance(doors.transform.position, doorClosePOS) < openDoorThreshold)
            {
                doors.transform.position = doorClosePOS;
                shouldMove = false;
                eventDone = true;
                played = false;
            }
        }

        if (enemiesTriggered && gameManager.instance.enemiesRemaining == 0 && !doorsClosed)
        {
            if (!roomDecremented)
            {
                roomDecremented = true;
                gameManager.instance.roomCount--;
            }

            dust.SetActive(true);
            doors.transform.position = Vector3.Lerp(doors.transform.position, doorOpenPOS, Time.deltaTime * doorSpeed);

            if (!doorAudioSource.isPlaying && !played)
            {
                doorAudioSource.Play();
                played = true;
            }

            if (Vector3.Distance(doors.transform.position, doorOpenPOS) < 0.252f)
            {
                dust.SetActive(false);
            }

            if (Vector3.Distance(doors.transform.position, doorOpenPOS) < openDoorThreshold)
            {
                doors.transform.position = doorOpenPOS;
                doors.SetActive(false);
                enemiesTriggered = false;
                doorsClosed = true;
                played = false;
            }
        }
    }

    private void GetSpawner()
    {
        foreach (Transform child in gameObject.transform)
        {
            if (child.name == "Spawner")
            {
                spawner = child.gameObject;
            }
        }
    }
    private void GetDoors()
    {
        foreach (Transform child in gameObject.transform)
        {
            if (child.name == "Doors")
            {
                doors = child.gameObject;
            }
        }
    }
    private void GetDoorsPositions()
    {
        foreach (Transform child in gameObject.transform)
        {
            if (child.name == "DoorsClosePOS")
            {
                doorClosePOS = child.position;
            }
            else if (child.name == "DoorsOpenPOS")
            {
                doorOpenPOS = child.position;
            }
        }
    }
    
    private void GetDust()
    {
        foreach (Transform child in gameObject.transform)
        {
            if (child.name == "Dust")
            {
                dust = child.gameObject;
            }
        }
    }

    private void GetHiddenRoomIcon()
    {
        foreach (Transform child in gameObject.transform)
        {
            if (child.name == "RoomIcon")
            {
                hiddenRoomIcon = child.gameObject;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            shouldMove = true;
            hiddenRoomIcon.SetActive(false);
        }
    }
}

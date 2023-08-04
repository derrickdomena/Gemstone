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

    bool played = false;
    bool enemiesTriggered = false;
    bool eventDone = false;

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
            StartCoroutine(Shake());
            doors.transform.position = Vector3.Lerp(doors.transform.position, doorClosePOS, Time.deltaTime * doorSpeed);
            if (!doorAudioSource.isPlaying && !played)
            {
                doorAudioSource.Play();
                played = true;
            }
            if (Vector3.Distance(doors.transform.position, doorClosePOS) < 0.252f)
            {
                dust.SetActive(false);
                spawner.SetActive(true);
                enemiesTriggered = true;
            }
            if (Vector3.Distance(doors.transform.position, doorClosePOS) < 0.001f)
            {
                shouldMove = false;
                eventDone = true;
            }
        }

        if (enemiesTriggered && gameManager.instance.enemiesRemaining == 0)
        {           
            dust.SetActive(true);
            StartCoroutine(Shake());
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
            if (Vector3.Distance(doors.transform.position, doorOpenPOS) < 0.001f)
            {
                doors.SetActive(false);
                enemiesTriggered = false;
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


    public float shakeDuration = 2f; // Duration of the shake in seconds
    public float shakeMagnitude = 0.1f; // Magnitude of the shake

    public IEnumerator Shake()
    {
        Vector3 originalPos = Camera.main.transform.localPosition;

        float totalDistanceToTravel = Vector3.Distance(doors.transform.position, doorClosePOS);
        float currentShakeMagnitude = shakeMagnitude;

        while (shouldMove)
        {
            float x = UnityEngine.Random.Range(-1f, 1f) * currentShakeMagnitude;
            float y = UnityEngine.Random.Range(-1f, 1f) * currentShakeMagnitude;

            Camera.main.transform.localPosition = originalPos + new Vector3(x, y, 0);

            // Calculate the remaining distance to travel
            float remainingDistance = Vector3.Distance(doors.transform.position, doorClosePOS);

            // Use remaining distance to total distance ratio to calculate dampening
            float dampeningFactor = remainingDistance / totalDistanceToTravel;

            // Apply the dampening factor using smooth step interpolation
            currentShakeMagnitude = Mathf.Lerp(0, shakeMagnitude, dampeningFactor * dampeningFactor * (3.0f - 2.0f * dampeningFactor));

            yield return null;
        }

        Camera.main.transform.localPosition = originalPos;
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

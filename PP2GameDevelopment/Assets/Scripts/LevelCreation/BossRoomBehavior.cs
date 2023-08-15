
using UnityEngine;


public class BossRoomBehavior : MonoBehaviour
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
    bool eventDone = false;
    bool doorsClosed = false;
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

    enum DoorState
    {
        Closed,
        Opening,
        Closing,
        Opened
    }

    DoorState doorState = DoorState.Closed;

    void Update()
    {
        // External conditions to switch states.
        if (doorState == DoorState.Opened && shouldMove && !eventDone)
        {
            doorState = DoorState.Closing;
        }
        else if (doorState == DoorState.Closed && gameManager.instance.roomCount - 3 == 0 && !doorsClosed)
        {
            doorsClosed = true;
            doorState = DoorState.Opening;
        }

        // Handling door behavior based on its current state.
        switch (doorState)
        {
            case DoorState.Closing:
                HandleDoorClosing();
                break;
            case DoorState.Opening:
                HandleDoorOpening();
                break;
            default:
                break;
        }
    }

    void HandleDoorClosing()
    {
        if (shouldMove && !eventDone)
        {
            doors.SetActive(true);
            dust.SetActive(true);

            MoveDoorTowards(doors.transform.position, doorClosePOS);

            if (Vector3.Distance(doors.transform.position, doorClosePOS) < openDoorThreshold)
            {
                dust.SetActive(false);
                if (spawner != null)
                {
                    spawner.SetActive(true);
                }

                doors.transform.position = doorClosePOS;
                doorState = DoorState.Closed;
                ResetPlayedFlag();
            }
        }
    }

    void HandleDoorOpening()
    {
        if (gameManager.instance.roomCount - 3 == 0)
        {
            dust.SetActive(true);
            MoveDoorTowards(doors.transform.position, doorOpenPOS);
            gameManager.instance.StartCoroutine(gameManager.instance.MiniBossRoomTextTimer());

            if (Vector3.Distance(doors.transform.position, doorOpenPOS) < openDoorThreshold)
            {
                dust.SetActive(false);
                doors.transform.position = doorOpenPOS;
                doors.SetActive(false);
                doorState = DoorState.Opened;
                ResetPlayedFlag();
            }
        }
    }

    void MoveDoorTowards(Vector3 currentPos, Vector3 targetPos)
    {
        doors.transform.position = Vector3.Lerp(currentPos, targetPos, Time.deltaTime * doorSpeed);
        PlayDoorAudio();
    }

    void PlayDoorAudio()
    {
        if (!doorAudioSource.isPlaying && !played)
        {
            doorAudioSource.Play();
            played = true;
        }
    }

    void ResetPlayedFlag()
    {
        played = false;
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

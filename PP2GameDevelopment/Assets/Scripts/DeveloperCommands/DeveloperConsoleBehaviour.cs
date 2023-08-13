using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static UnityEngine.InputSystem.InputAction;
public class DeveloperConsoleBehaviour : MonoBehaviour
{
    [SerializeField] private string prefix = string.Empty;
    [SerializeField] private ConsoleCommand[] commands = new ConsoleCommand[0];

    [Header("UI")]
    [SerializeField] private GameObject uiCanvas = null;
    [SerializeField] private TMP_InputField inputField = null;

    private float pauseTimeScale;

    private static DeveloperConsoleBehaviour instance;

    private DeveloperConsole developerConsole;

    private DeveloperConsole DeveloperConsole
    {
        get
        {
            if(developerConsole != null) {
            return developerConsole;}
            return developerConsole = new DeveloperConsole(prefix, commands);
        }
    }

    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.RightBracket)) {
            Toggle();
        }
    }
    public void Toggle()
    {
        //is the uiCanvas already open?
        if(uiCanvas.activeSelf)
        {
            //Yes. then put the time scale back to the pauseTimeScale and set active to false
            Time.timeScale = pauseTimeScale;
            uiCanvas.SetActive(false);
        }
        else
        {
            //No. Then set pauseTimeScale to whatever timescale is current
            //set the timescale to 0 and set the canvase to true
            //set inputField active
            pauseTimeScale = Time.timeScale;
            Time.timeScale = 0;
            uiCanvas.SetActive(true);
            inputField.ActivateInputField();
        }
    }

    public void ProcessCommand(string inputValue)
    {
        DeveloperConsole.ProcessInput(inputValue);

        inputField.text = string.Empty;
    }
}

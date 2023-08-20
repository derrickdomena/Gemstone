using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private Slider loadingSlider;

    private void Awake()
    {
        loadingScreen = GameObject.Find("LoadingScreen");
        mainMenu = GameObject.Find("MainMenu");
        loadingSlider = GameObject.Find("LoadingScreen/LoadingSlider").GetComponent<Slider>();
        loadingScreen.SetActive(false);
    }

    public void LoadLevel(int level)
    {

        mainMenu.SetActive(false);
        loadingScreen.SetActive(true);
        if (level == 3)
        {
            StartCoroutine(CutsceneTimer());
        }
        else
        {
            StartCoroutine(LoadLevelASync(level));
        }
    }

    IEnumerator LoadLevelASync(int level)
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(level);

        while (!loadOperation.isDone)
        {
            float progressValue = Mathf.Clamp01(loadOperation.progress / 0.9f);
            loadingSlider.value = progressValue;
            yield return null;
        }
    }
    IEnumerator CutsceneTimer()
    {
        yield return new WaitForSeconds(2);
        LoadLevel(4);
    }
}

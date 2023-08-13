using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class LevelCleared : MonoBehaviour
{
    public float animationDuration = 2f; // How long the animation takes in seconds.
    public TextMeshProUGUI KillCountText;

    public PlayerStats playerStats;
    float elapsedTime = 0f;
    float currentStatValue = 0f;
    int incrementCounter = 0; // Counter to keep track of increments
    float currentAnimationDuration; // Start with the default duration

    private void Start()
    {
        currentAnimationDuration = animationDuration;
        ShowStat(playerStats.NumberOfKills);
    }
    public void ShowStat(int finalValue)
    {
        StartCoroutine(IncrementStatAnimation(finalValue));
    }

    private IEnumerator IncrementStatAnimation(int finalValue)
    {
        float elapsedTime = 0f;
        float currentStatValue = 0f;

        while (currentStatValue < finalValue)
        {
            elapsedTime += Time.deltaTime / currentAnimationDuration;

            // Speed up effect: Reduce the currentAnimationDuration every 5 increments, which means it will increment faster.
            if (incrementCounter++ % 5 == 0)
            {
                currentAnimationDuration *= 0.9f; // Make it 10% faster, adjust the value as needed
            }

            currentStatValue = Mathf.Lerp(0, finalValue, elapsedTime);

            // If we've overshot due to the lerp and deltaTime, clamp to the final value.
            if (currentStatValue > finalValue)
            {
                currentStatValue = finalValue;
            }

            KillCountText.text = Mathf.FloorToInt(currentStatValue).ToString();

            yield return null;
        }
    }
}

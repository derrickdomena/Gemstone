using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerStatsMenu : MonoBehaviour
{
    [SerializeField] private Transform container;
    [SerializeField] private GameObject statTemplate;

    public int playerHP;
    public int playerTotalDamage;
    public float PlayerCritChance;
    public float PlayerCritDamage;
    public int playerShootDamage;
    public int playerGems;
    GameObject hp;
    GameObject totalDam;
    GameObject critChance;
    GameObject critDam;
    GameObject shootDam;
    GameObject gems;

    public void ShowStats()
    {
        UpdateStats();
        hp.transform.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = null;
        hp.transform.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = playerHP.ToString();
        
        totalDam.transform.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = null;
        totalDam.transform.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = playerTotalDamage.ToString();

        critChance.transform.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = null;
        critChance.transform.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = PlayerCritChance.ToString();

        critDam.transform.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = null;
        critDam.transform.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = PlayerCritDamage.ToString();

        shootDam.transform.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = null;
        shootDam.transform.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = playerShootDamage.ToString();


        gems.transform.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = null;
        gems.transform.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = playerGems.ToString();
    }

    public void UpdateStats()
    {
        playerHP = gameManager.instance.playerScript.hpMax;
        playerTotalDamage = gameManager.instance.playerScript.totalDamage;
        PlayerCritChance = gameManager.instance.playerScript.critChance;
        PlayerCritDamage = gameManager.instance.playerScript.critDam + gameManager.instance.playerScript.shootDamage;
        playerShootDamage = gameManager.instance.playerScript.shootDamage;
        playerGems = gameManager.instance.gemCount;
    }

    public void PopulateStats()
    {
        hp = Instantiate(statTemplate, container);
        totalDam = Instantiate(statTemplate, container);
        critChance = Instantiate(statTemplate, container);
        critDam = Instantiate(statTemplate, container);
        shootDam = Instantiate(statTemplate, container);
        gems = Instantiate(statTemplate, container);
    }
}

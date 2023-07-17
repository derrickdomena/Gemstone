using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBattle : MonoBehaviour
{

    public enum Stage
    {
        WaitingToStart,
        Stage1,
        Stage2,
        Stage3
    }
    [SerializeField] private ColliderTrigger1 trigger;
    [SerializeField] private BossAI spider;
    private Stage stage;

    private void Awake()
    {
        stage = Stage.WaitingToStart;
        
    }

    public void Start()
    {
        trigger.OnPlayerEnterTrigger += ColliderTrigger_OnPlayerEnterTrigger;
        spider.GetHealthSystem().OnDamaged += BossBattle_OnDamaged;
        spider.GetHealthSystem().OnDead += BossBattle_OnDead;
    }
    private void BossBattle_OnDead(object sender, System.EventArgs e)
    {
        //Boss is dead
        gameManager.instance.bossHP.SetActive(false);
        Debug.Log("Boss Battle is over!");
    }

    private void BossBattle_OnDamaged(object sender, System.EventArgs e)
    {
        switch(stage)
        {
            default:
            case Stage.Stage1:
                //if enemy hp reaches a certain threshold do something
                if(spider.GetHealthSystem().GetHealthPercent() == .5f) {
                    //75%
                    StartNextStage();
                    spider.PhaseTwo();
                }
                break;
            case Stage.Stage2:
                if (spider.GetHealthSystem().GetHealthPercent() == .25f)
                {
                    //50%
                    StartNextStage();
                    spider.PhaseThree();
                }
                break;
        }
    }
    
    private void StartNextStage()
    {
        switch(stage)
        {
            default:
            case Stage.WaitingToStart:
                stage = Stage.Stage1;
                Debug.Log("Stage 1 started");
                break;
            case Stage.Stage1:
                stage = Stage.Stage2;
                Debug.Log("Stage 2 started");
                break;
            case Stage.Stage2:
                stage = Stage.Stage3;
                Debug.Log("Stage 3 started");
                break;
        }
    }

    private void ColliderTrigger_OnPlayerEnterTrigger(object sender, System.EventArgs e)
    {
        StartBattle();
        gameManager.instance.bossHP.SetActive(true);
        trigger.OnPlayerEnterTrigger -= ColliderTrigger_OnPlayerEnterTrigger;
    }
    private void StartBattle()
    {
        Debug.Log("StartBattle");
        StartNextStage();
        stage = Stage.Stage1;
        //Spawn enemies
    }
}

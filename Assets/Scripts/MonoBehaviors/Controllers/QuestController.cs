using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;

public class QuestController : MonoBehaviour
{
    //public Text questionDisplayText;
    public Text scoreDisplayText;
    public Text timeRemainingDisplayText;
    public Text questNameText;
    //public Text questJournalText;
    public InputField questJournalInput;

    public Button[] activityButton = new Button[numActivitySlots];
    public const int numActivitySlots = 4;
    public Slider questSlider;

    public GameObject questDisplay;
    public GameObject roundEndDisplay;
    
    public string questJournal;

    private DataController dataController;
    private QuestData currentQuestData;
    private KindnessActsData[] activityPool;
    private bool[] actComplete;
    private int activitiesCompleted;

    private bool isRoundActive;
    private float timeRemaining;
    // private int activityIndex;
    private int playerScore;

    // Use this for initialization
    void Start()
    {
        dataController = FindObjectOfType<DataController>();
        //EventManager.StartListening("questready", DisplayQuest);

        //currentQuestData = dataController.GetActiveQuestData();
        StartCoroutine(DisplayQuest());

  //      timeRemaining = currentQuestData.timeLimitInSeconds;

  //      UpdateTimeRemainingDisplay();

        playerScore = dataController.playerProfile.kindnessUnits;
        scoreDisplayText.text = "KP: " + playerScore.ToString();

//        isRoundActive = false;

    }

    IEnumerator DisplayQuest()
    {
        /*
        questNameText.text = currentQuestData.name;
        Debug.Log("DisplayQuest: journal" + currentQuestData.questJournal + "," + questJournalInput.text);
        questJournalInput.text = currentQuestData.questJournal;
        pkSlider.value = currentQuestData.playerKindness;
        ckSlider.value = currentQuestData.communityKindness;
        rrSlider.value = currentQuestData.recipientReaction;
        */
        while (!dataController.questDataLoaded)
            yield return new WaitForSeconds(5);
        currentQuestData = dataController.GetActiveQuestData();
        activityPool = currentQuestData.kindnessActs;
        Debug.Log("Display quest: activitypool lenght: "+activityPool.Length.ToString());
        actComplete = currentQuestData.actComplete;
        ShowActivities();
        
    }

    private void ShowActivities()
    {

        activitiesCompleted = 0;
        for (int i = 0; i < activityPool.Length; i++)
        {
            Debug.Log(string.Format("Adding activity {0} of {1} to pool", i, activityPool.Length));

            activityButton[i].GetComponentInChildren<Text>().text = activityPool[i].actionText;
            if (actComplete[i])
            {
                activitiesCompleted++;
                activityButton[i].interactable = false;
            } else {
                activityButton[i].interactable = true;
            }

        }
        questSlider.value = activitiesCompleted;
    }
    
    public void CompeleteQuestActivity(int i)
    {
        currentQuestData.actComplete[i] = true;
        activityButton[i].interactable = false;
        playerScore += activityPool[i].kindnessReward;
        activitiesCompleted++;

        // display score
        scoreDisplayText.text = "KP: " + playerScore.ToString();

        // update slider
        questSlider.value = activitiesCompleted;

        // check if done
        if (activitiesCompleted == numActivitySlots) QuestComplete();
        Debug.Log(string.Format("Activity {0} compelete. Total completed {1}. Player Score {2}", i, activitiesCompleted,playerScore));

    }

    private void QuestComplete()
    {
        currentQuestData.isActive = false;
        currentQuestData.questDate = DateTime.Now;
        dataController.playerProfile.kindnessUnits = playerScore;
        dataController.SaveGameData();
        dataController.SavePlayerProfile();
        // create the next one
        NewQuest();
    }
    

    

    public void NewQuest()
    {
        currentQuestData = dataController.CreateNewQuest();
        DisplayQuest();
        timeRemaining = currentQuestData.timeLimitInSeconds;
       // UpdateTimeRemainingDisplay();
        isRoundActive = true;
    }

    public void NextQuest()
    {
        QuestData nextQuestData;
        nextQuestData = dataController.NextQuest();

        if (nextQuestData != null)
        {
            currentQuestData = nextQuestData;
            DisplayQuest();

        }

    }
    public void PrevQuest()
    {
        QuestData prevQuestData;
        prevQuestData = dataController.PrevQuest();
        if (prevQuestData != null)
        {
            currentQuestData = prevQuestData;
            DisplayQuest();
        }
    }

    public void UpdateJournal(string journal)
    {
        Debug.Log("UpdateJournal: " + journal + ",current:" + currentQuestData.questJournal + ", display:" + questJournalInput.text);

        currentQuestData.questJournal = journal;
    }

    
    private void UpdateTimeRemainingDisplay()
    {
        timeRemainingDisplayText.text = "Time: " + Mathf.Round(timeRemaining).ToString();
    }

    


}
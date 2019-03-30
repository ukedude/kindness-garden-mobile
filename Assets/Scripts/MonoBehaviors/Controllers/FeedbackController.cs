using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class FeedbackController : MonoBehaviour
{
    public Slider pkSlider;
    public Slider rrSlider;
    public Slider ckSlider;
    public float playerKindness;
    public float recipientReaction;
    public float communityKindness;
    public GameObject feedbackPanel;

    public const float feedbackWaitTime = 1800.0f; // will change later to  days

    private DataController dataController;
    private float feedbackTimeRemaining;
    private IEnumerator coroutine;

    // Start is called before the first frame update
    void Start()
    {
        dataController = FindObjectOfType<DataController>();
        feedbackTimeRemaining = dataController.playerProfile.feedbackTimeRemaining;
        // need to add logic to adjust for time waited
        coroutine = WaitForFeedback(feedbackWaitTime);
        StartCoroutine(coroutine);

    }

    private IEnumerator WaitForFeedback(float waitTime)
    {
        while (true)
        {
            yield return new WaitForSeconds(waitTime);
            feedbackPanel.SetActive(true);
        }
    }

    
    public void CompleteFeedback()
    {
        feedbackPanel.SetActive(false);
        
        FeedbackData newFeedback = new FeedbackData();
        newFeedback.communityKindness = communityKindness;
        newFeedback.playerKindness = playerKindness;
        newFeedback.recipientReaction = recipientReaction;
        newFeedback.feedbackDate = DateTime.Now.ToString();
        dataController.allFeedbackData.Add(newFeedback);
        dataController.SaveGameData();

    }

    public void RecordPlayerKindness(float pk)
    {
        playerKindness = pk;
    }
    public void RecordRecipientReaction(float rr)
    {
        recipientReaction = rr;
    }
    public void RecordCommunityKindness(float ck)
    {
        communityKindness = ck;
    }

}

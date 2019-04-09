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

    public float feedbackWaitTime = 45.0f; // will change later to  days

    private DataController dataController;
    private float feedbackTimeRemaining;
    private IEnumerator coroutine;

    DateTime currentDate;
    DateTime feedbackDate;

    // Start is called before the first frame update
    void Start()
    {
        dataController = FindObjectOfType<DataController>();
        feedbackTimeRemaining = dataController.playerProfile.feedbackTimeRemaining;

        //Store the current time when it starts
        currentDate = System.DateTime.Now;

        //Grab the old time from the player prefs as a long
        if (PlayerPrefs.HasKey("feedbackDate") == false)
        {
            PlayerPrefs.SetString("feedbackDate", System.DateTime.Now.AddSeconds(60).ToBinary().ToString());
        }
       
        long temp = Convert.ToInt64(PlayerPrefs.GetString("feedbackDate"));

        //Convert the old time from binary to a DataTime variable
        DateTime feedbackDate = DateTime.FromBinary(temp);
        Debug.Log("feedbackDate: " + feedbackDate);

        //Use the Subtract method and store the result as a timespan variable
        TimeSpan difference = currentDate.Subtract(feedbackDate);

        feedbackWaitTime = difference.Seconds;

        if (feedbackWaitTime < 45.0f) feedbackWaitTime = 45.0f;
        Debug.Log("Feedback wait time: " + feedbackWaitTime.ToString());
        
       
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

        //Save next feedback date as a string in the player prefs class
        feedbackDate = System.DateTime.Now.AddSeconds(60);
        PlayerPrefs.SetString("feedbackDate", feedbackDate.ToBinary().ToString());
        Debug.Log("Next feedback date: " + feedbackDate.ToString());

        // add kindness points

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

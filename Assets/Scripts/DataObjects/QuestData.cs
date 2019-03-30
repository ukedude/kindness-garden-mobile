using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class QuestData 
{
    public string name;
    public int timeLimitInSeconds;
    public float timeRemaining;
    public bool isActive;
    public DateTime questDate;
    public KindnessActsData[] kindnessActs = new KindnessActsData[4];
    public bool[] actComplete = new bool[4];
    //public QuestActivity[] questActivities;
    public float playerKindness;
    public float recipientReaction;
    public float communityKindness;
    public string questJournal;
    public bool questComplete;

}

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Unity.Editor;
using Firebase.Database;
// This code came mostly from the Unity tutorial for a quiz game

public class DataController : MonoBehaviour
{
    public List<QuestData> allQuestData;
    public List<FeedbackData> allFeedbackData;
    public Garden myGarden;  // just one for now
    public List<string> myBasket;
    
    public PlayerProfile playerProfile;

    private string gameDataFileName = "data.json";
    public KindnessActsData[] allKindnessActs;

    public bool questDataLoaded;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        // Set this before calling into the realtime database.
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://kindness-garden.firebaseio.com/");

        LoadGameData();
        LoadPlayerProfile();
        //SceneManager.LoadScene("Menu");
        SceneManager.LoadScene("Garden");
    }

    public QuestData GetActiveQuestData()
    {
        Debug.Log(string.Format("GetActiveQuestData {0} of {1}", playerProfile.currentQuestIndex, allQuestData.Count));
        
        Debug.Log(string.Format("Current quest {0} of {1}", playerProfile.currentQuestIndex, allQuestData.Count));
        if (allQuestData.Count == 0)
        {
            playerProfile.currentQuestIndex = 0;
            return CreateNewQuest();

        } else if (!allQuestData[allQuestData.Count-1].isActive) {
            playerProfile.currentQuestIndex = allQuestData.Count;
            return CreateNewQuest();
        }
        else
        {
            playerProfile.currentQuestIndex = allQuestData.Count - 1;
            return allQuestData[playerProfile.currentQuestIndex];
        }

    }
    public QuestData GetCurrentQuestData()
    {
        Debug.Log(string.Format("GetCurrentQuestData {0} of {1}", playerProfile.currentQuestIndex,allQuestData.Count));
        //cleanup quest index if needed
        if (playerProfile.currentQuestIndex > allQuestData.Count - 1)
        {
            playerProfile.currentQuestIndex = 0;
            
        }
        Debug.Log(string.Format("Current quest {0} of {1}", playerProfile.currentQuestIndex, allQuestData.Count));
        if (allQuestData.Count == 0)
        {
            return CreateNewQuest();
            
        } else
        {
            return allQuestData[playerProfile.currentQuestIndex];
        }
        
    }
   
    public QuestData NextQuest()
    {
        Debug.Log(string.Format("NextQuest {0} of {1}", playerProfile.currentQuestIndex, allQuestData.Count));

        if (playerProfile.currentQuestIndex < allQuestData.Count - 1)
        {
            playerProfile.currentQuestIndex++;
            return allQuestData[playerProfile.currentQuestIndex];
        }
        else return null;
        

    }
    public QuestData PrevQuest()
    {
        Debug.Log(string.Format("PrevQuest {0} of {1}", playerProfile.currentQuestIndex, allQuestData.Count));
        if (playerProfile.currentQuestIndex > 0)
        {
            playerProfile.currentQuestIndex--;
            return allQuestData[playerProfile.currentQuestIndex];
        }
        else return null;
        

    }
    public QuestData CreateNewQuest()
    {
        Debug.Log(string.Format("CreateNewQuest {0} of {1}", playerProfile.currentQuestIndex, allQuestData.Count));

        // clean up quest index if needed
        if (playerProfile.currentQuestIndex > allQuestData.Count - 1)
        {
            playerProfile.currentQuestIndex = 0;
            
        }

        // create a new quest object
        QuestData newQuest = new QuestData();
        int iSelected;
        // Randomly get 4 quest items
        for (int i = 0; i < 4;  i++)
        {
            Debug.Log("CreateNewQuest: allKindnessActs.Length.ToString()" + allKindnessActs.Length.ToString());
            try
            {
                iSelected = UnityEngine.Random.Range(0, allKindnessActs.Length);
            }
            catch (Exception e)
            {
                print("error "+e.ToString());
                return null;
            }
            KindnessActsData kact = new KindnessActsData();
            kact = allKindnessActs[iSelected];
            newQuest.kindnessActs[i] = kact;
            Debug.Log(newQuest.kindnessActs[i].actionText);
            newQuest.actComplete[i] = false;

        }

        newQuest.questDate = DateTime.Now;
        newQuest.timeRemaining = 30;
        newQuest.timeLimitInSeconds = 30;

        // Add the quest to the list and update index
        allQuestData.Add(newQuest);
        playerProfile.currentQuestIndex = allQuestData.Count - 1;

        return newQuest;
    }
    // copied from https://forum.unity.com/threads/how-to-load-an-array-with-jsonutility.375735/
        public class JsonHelper
    {
        public static T[] getJsonArray<T>(string json)
        {
            string newJson = "{ \"array\": " + json + "}";
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
            return wrapper.array;
        }

        [Serializable]
        private class Wrapper<T>
        {
            public T[] array;
        }
    }
    public bool IsQuestLoaded()
    {
        return questDataLoaded;
    }
    private void GetActDataFromDB()
    {
        questDataLoaded = false;
        //get data from the database
        FirebaseDatabase.DefaultInstance
      .GetReference("allKindnessActs")
      .GetValueAsync().ContinueWith(task => {
          if (task.IsFaulted)
          {
              // Handle the error...
              Debug.Log("Error retrieving data from DB: " + task.Exception);
          }
          else if (task.IsCompleted)
          {
              DataSnapshot snapshot = task.Result;
              Debug.Log("Kind acts retrieved from DB:" + snapshot.ChildrenCount.ToString());
              // Do something with snapshot...
              //string kadata = "{'allKindnessActs': " + snapshot.GetRawJsonValue() + "}";
              //Debug.Log(kadata);
              //allKindnessActs = JsonUtility.FromJson<KindnessActsData[]>(kadata);
              allKindnessActs = JsonHelper.getJsonArray<KindnessActsData>(snapshot.GetRawJsonValue());
              Debug.Log("Kind acts loaded DB:" + allKindnessActs.Length.ToString());
              questDataLoaded = true;
             // EventManager.TriggerEvent("questready");
              
          }
      });
    }
    private void LoadGameData()
    {
        GetActDataFromDB();

        //string filePath = Path.Combine(Application.streamingAssetsPath, gameDataFileName);
        string filePath = Path.Combine(Application.persistentDataPath, gameDataFileName);
        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
            GameData loadedData = JsonUtility.FromJson<GameData>(dataAsJson);

            // get basket inventory
            myBasket = loadedData.myBasket;
            if (myBasket == null)
            {
                myBasket = new List<string>();
            }

            // Get garden items. 
            // Create a new one if not found.  This will be depricated later when we allow the player to create one.
            myGarden = loadedData.myGarden;
            if (myGarden == null)
            {
                myGarden = new Garden();
                myGarden.gardenName = "My Garden";
                myGarden.gardenLocation = new Vector2(0, 0);
                myGarden.gardenPlants = new List<PlantItem>();
            }
            Debug.Log(string.Format("Loaded {0} garden plant items.", myGarden.gardenPlants.Count));

            // Get quest data.
            // Refine this later.
            QuestData[] qData = loadedData.allQuestData;
            Debug.Log(string.Format("Loaded {0} quests", qData.Length));
            if (qData != null)
            {
                allQuestData = new List<QuestData>(loadedData.allQuestData);
                Debug.Log("Converted quests");
                Debug.Log(string.Format("Loaded {0} quest data items.", loadedData.allQuestData.Length));
                
                
                
            } else
            {
                allQuestData = new List<QuestData>();
                Debug.Log("No quest loaded. Created a new one");
            }

            // Load feedback
            if (loadedData.feedbackData != null)
            {
                allFeedbackData = new List<FeedbackData>(loadedData.feedbackData);
            } else
            {
                allFeedbackData = new List<FeedbackData>();
            }
            // Set kindess act inventory.
            //allKindnessActs = loadedData.allKindnessActs;
            
        }
        else
        {
            Debug.LogError ("Cannot load game data!");

        }



    }
   public void SaveGameData()
    {
        
        GameData gameData = new GameData();

        gameData.allKindnessActs = allKindnessActs;
        gameData.allQuestData = allQuestData.ToArray();
        gameData.feedbackData = allFeedbackData.ToArray();
        gameData.myBasket = myBasket;
        gameData.myGarden = myGarden;
        Debug.Log(string.Format("Saving {0} quest data items.",gameData.allQuestData.Length));
        Debug.Log(string.Format("Saving {0} garden plant items.", gameData.myGarden.gardenPlants.Count));


        

        // save to local file
        // streaming assets dosen't work on Android
        string dataAsJson = JsonUtility.ToJson(gameData,true);
        string filePath = Path.Combine(Application.streamingAssetsPath, gameDataFileName);
        string filePath2 = Path.Combine(Application.persistentDataPath, gameDataFileName);

        File.WriteAllText(filePath, dataAsJson);
        File.WriteAllText(filePath2, dataAsJson);
        Debug.Log("Game Data Saved");
        Debug.Log(filePath2);

        // Save to firebase
        // need to add code to make sure logged in
        AuthenticationController auth = FindObjectOfType<AuthenticationController>();
        string userId = auth.userID;

        //string userId = Firebase.Auth.FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        // Get the root reference location of the database.
        DatabaseReference mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
        string questJson = JsonUtility.ToJson(gameData.allQuestData[playerProfile.currentQuestIndex], true);
        string key = mDatabaseRef.Child("quests").Child(userId).Push().Key;
        mDatabaseRef.Child("quests").Child(userId).Child(key).SetRawJsonValueAsync(questJson);
    }
    // This function could be extended easily to handle any additional data we wanted to store in our PlayerProgress object
    private void LoadPlayerProfile()
    {
        // Create a new PlayerProgress object
        playerProfile = new PlayerProfile();

        // If PlayerPrefs contains a key called "highestScore", set the value of playerProgress.highestScore using the value associated with that key
        if (PlayerPrefs.HasKey("highestScore"))
        {
            playerProfile.highestScore = PlayerPrefs.GetInt("highestScore");
        }
        if (PlayerPrefs.HasKey("currentQuestIndex"))
        {
            playerProfile.currentQuestIndex = PlayerPrefs.GetInt("currentQuestIndex");
        }
        if (PlayerPrefs.HasKey("playerName"))
        {
            playerProfile.playerName = PlayerPrefs.GetString("playerName");
        }
        if (PlayerPrefs.HasKey("playerAge"))
        {
            playerProfile.playerAge = PlayerPrefs.GetInt("playerAge");
        }
        if (PlayerPrefs.HasKey("timePreference"))
        {
            playerProfile.timePreference = PlayerPrefs.GetString("timePreference");
        }
        if (PlayerPrefs.HasKey("fundsPreference"))
        {
            playerProfile.fundsPreference = PlayerPrefs.GetString("fundsPreference");
        }
        //   if (PlayerPrefs.HasKey("plantUnits"))
        //   {
        //       playerProfile.plantUnits = PlayerPrefs.GetInt("plantUnits");
        //   }
        //   if (PlayerPrefs.HasKey("waterUnits"))
        //   {
        //       playerProfile.waterUnits = PlayerPrefs.GetInt("waterUnits");
        //   }
        //   if (PlayerPrefs.HasKey("foodUnits"))
        //   {
        //       playerProfile.foodUnits = PlayerPrefs.GetInt("foodUnits");
        //   }
        if (PlayerPrefs.HasKey("kindnessUnits"))
        {
            playerProfile.kindnessUnits = PlayerPrefs.GetInt("kindnessUnits");
        }
        if (PlayerPrefs.HasKey("questTimeRemaining"))
        {
            playerProfile.questTimeRemaining = PlayerPrefs.GetFloat("questTimeRemaining");
        } else
        {
            playerProfile.questTimeRemaining = 0.0f;
        }
        if (PlayerPrefs.HasKey("feedbackTimeRemaining"))
        {
            playerProfile.feedbackTimeRemaining = PlayerPrefs.GetFloat("feedbackTimeRemaining");
        }
        else
        {
            playerProfile.feedbackTimeRemaining = 0.0f;
        }
        Debug.Log("Player prefs loaded");
    }

    // This function could be extended easily to handle any additional data we wanted to store in our PlayerProgress object
    public void SavePlayerProfile()
    {
        // Save the value playerProgress.highestScore to PlayerPrefs, with a key of "highestScore"
        PlayerPrefs.SetInt("highestScore", playerProfile.highestScore);
        PlayerPrefs.SetInt("currentQuestIndex", playerProfile.currentQuestIndex);
        PlayerPrefs.SetString("playerName", playerProfile.playerName);
        PlayerPrefs.SetInt("playerAge", playerProfile.playerAge);
        PlayerPrefs.SetString("timePrefence", playerProfile.timePreference);
        PlayerPrefs.SetString("fundsPreference", playerProfile.fundsPreference);
        //PlayerPrefs.SetInt("plantUnits", playerProfile.plantUnits);
        //PlayerPrefs.SetInt("waterUnits", playerProfile.waterUnits);
        //PlayerPrefs.SetInt("foodUnits", playerProfile.foodUnits);
        PlayerPrefs.SetInt("kindnessUnits", playerProfile.kindnessUnits);
        PlayerPrefs.Save();
        Debug.Log("Player prefs saved");
    }



}

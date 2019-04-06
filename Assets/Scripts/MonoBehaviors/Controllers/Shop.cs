using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public Text scoreDisplayText;
    public Image[] itemImages = new Image[numItemSlots];
    public Item[] items = new Item[numItemSlots];
    public Text[] cost = new Text[numItemSlots];

    public const int numItemSlots = 9;

    private DataController dataController;
    private Inventory myInventory;

    private void Start()
    {
        dataController = FindObjectOfType<DataController>();
        myInventory = FindObjectOfType<Inventory>();

        int playerScore = dataController.playerProfile.kindnessUnits;
        scoreDisplayText.text = "KP: " + playerScore.ToString();

        for (int i = 0; i < numItemSlots; i++)
        {
            if (items[i] != null)
            {
                cost[i].text = items[i].cost.ToString();
                itemImages[i].sprite = items[i].sprite;
                itemImages[i].enabled = true;
            } else
            {
                cost[i].text = "";
                itemImages[i].enabled = false;
            }
        }
        

    }
    public void BuyItem(Item itemToAdd)
    {
        if (dataController.playerProfile.kindnessUnits > itemToAdd.cost)
        {
            dataController.playerProfile.kindnessUnits -= itemToAdd.cost;
            int playerScore = dataController.playerProfile.kindnessUnits;
            dataController.SavePlayerProfile();
            scoreDisplayText.text = "KP: " + playerScore.ToString();
            dataController.myBasket.Add(itemToAdd.itemName);
            dataController.SaveGameData();
            myInventory.AddItem(itemToAdd);
        }
        
    }
    
    
}

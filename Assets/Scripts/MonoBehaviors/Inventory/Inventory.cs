using UnityEngine;
using UnityEngine.UI;


public class Inventory : MonoBehaviour
{
    public Image[] itemImages = new Image[numItemSlots];
    public Item[] items = new Item[numItemSlots];
    public const int numItemSlots = 9;

    private DataController dataController;
    //   private Mapbox.Examples.GardenController gardenController;
    private GardenController gardenController;
    private void Start()
    {
        dataController = FindObjectOfType<DataController>();
        //        gardenController = FindObjectOfType<Mapbox.Examples.GardenController>();
        gardenController = FindObjectOfType<GardenController>();
        foreach (string plant in dataController.myBasket)
        {
            var item = (Item)Resources.Load("Items/" + plant, typeof(Item));
            AddItem(item);
        }
        //DontDestroyOnLoad(gameObject);
        
    }
    public void AddItem(Item itemToAdd)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null)
            {
                items[i] = itemToAdd;
                itemImages[i].sprite = itemToAdd.sprite;
                itemImages[i].enabled = true;
                
                return;
            }
        }
    }
    
    public void UseItem(int i)
    {
        gardenController.AddPlant(items[i]);
        dataController.myBasket.Remove(items[i].itemName);
        dataController.SaveGameData();
        RemoveItem(items[i]);
    }
    
    public void RemoveItem(Item itemToRemove)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == itemToRemove)
            {
                items[i] = null;
                itemImages[i].sprite = null;
                itemImages[i].enabled = false;
                
                return;
            }
        }
    }
}


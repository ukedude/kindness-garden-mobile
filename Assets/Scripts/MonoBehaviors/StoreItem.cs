using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreItem : MonoBehaviour
{

    public Item item;
    //public Text cost;
    private Inventory inventory;    // Reference to the Inventory component.

    // Start is called before the first frame update
    void Start()
    {
        inventory = FindObjectOfType<Inventory>();
        //GameObject itemModel = Instantiate(item.itemModel);
        //itemModel.transform.SetParent(this.transform);

      //  cost.text = item.itemName+" Cost: "+item.cost.ToString();
    }

    void OnMouseDown()
    {
        Debug.Log("Object clicked: " + gameObject.name);
        inventory.AddItem(item);
    }

}

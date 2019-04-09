using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloneGarden : MonoBehaviour
{
    private GardenController gardenController;
    
    // Start is called before the first frame update
    void Start()
    {
        gardenController = FindObjectOfType<GardenController>();
        GameObject newObject = Instantiate(gardenController.myGardenObject, this.transform);
        newObject.transform.localScale = new Vector3(.02f, .02f, .02f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

namespace Mapbox.Examples
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.SceneManagement;
    using Mapbox.Utils;
    using Mapbox.Unity.Map;
    using Mapbox.Unity.Utilities;
    using Mapbox.Unity.Location;
    using System;

    public class GardenController : MonoBehaviour
    {

        [SerializeField]
        AbstractMap _map;

        [SerializeField]
        [Geocode]
        string[] _locationStrings;
        Vector2d[] _locations;

        [SerializeField]
        float _spawnScale = 100f;

        [SerializeField]
        GameObject _markerPrefab;

        private DataController dataController;
        private Garden myGarden;
        private GameObject myGardenObject;
        List<GameObject> _spawnedObjects;

        // Start is called before the first frame update
        void Start()
        {
            dataController = FindObjectOfType<DataController>();
            myGarden = dataController.myGarden;
            DisplayGardenOnMap();
        }

        void DisplayGardenOnMap()
        {
            _locationStrings = new string[1];   // only have one for now
            _locationStrings[0] = myGarden.gardenLatLong;
            _locations = new Vector2d[_locationStrings.Length];
            _spawnedObjects = new List<GameObject>();
            for (int i = 0; i < _locationStrings.Length; i++)
            {
                var locationString = _locationStrings[i];
                _locations[i] = Conversions.StringToLatLon(locationString);
                var instance = Instantiate(_markerPrefab);
                instance.transform.localPosition = _map.GeoToWorldPosition(_locations[i], true);
                instance.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
                _spawnedObjects.Add(instance);
            }
        }
        void LoadGarden()
        {
            myGardenObject = (GameObject)Instantiate(_markerPrefab, myGarden.gardenLocation, Quaternion.identity);
            foreach (PlantItem pi in myGarden.gardenPlants)
            {
                //Rigidbody plantClone = (Rigidbody)Instantiate(plant, pi.plantLocation, Quaternion.identity);
                //plantClone.transform.SetParent(myGardenObject.transform);

            }

        }
        private void Update()
        {
            int count = _spawnedObjects.Count;
            for (int i = 0; i < count; i++)
            {
                var spawnedObject = _spawnedObjects[i];
                var location = _locations[i];
                spawnedObject.transform.localPosition = _map.GeoToWorldPosition(location, true);
                spawnedObject.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
            }
        }
        bool _isInitialized;

        ILocationProvider _locationProvider;
        ILocationProvider LocationProvider
        {
            get
            {
                if (_locationProvider == null)
                {
                    _locationProvider = LocationProviderFactory.Instance.DefaultLocationProvider;
                }

                return _locationProvider;
            }
        }
        public void PlaceGarden()
        {
  //          var map = LocationProviderFactory.Instance.mapManager;
            //transform.localPosition = map.GeoToWorldPosition(LocationProvider.CurrentLocation.LatitudeLongitude);
            Debug.Log("PLaceGarden");
            myGarden.gardenLatLong = string.Format("{0}, {1}", LocationProvider.CurrentLocation.LatitudeLongitude.x, LocationProvider.CurrentLocation.LatitudeLongitude.y);
            Debug.Log(myGarden.gardenLatLong);
            dataController.SaveGameData();
            _locations[0] = LocationProvider.CurrentLocation.LatitudeLongitude;
            
            Debug.Log(LocationProvider.CurrentLocation.LatitudeLongitude);
        }
        // Update is called once per frame
        /*
        void Update()
        {
            if (Input.GetMouseButtonDown(0) && GUIUtility.hotControl == 0)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    Debug.Log("Name = " + hit.collider.name);
                    Debug.Log("Tag = " + hit.collider.tag);
                    Debug.Log("Hit Point = " + hit.point);
                    Debug.Log("Object position = " + hit.collider.gameObject.transform.position);
                    Debug.Log("--------------");
                }
                if (hit.collider.name == "Ground")
                {
                    //Rigidbody plantClone = (Rigidbody)Instantiate(plant, hit.point, Quaternion.identity);
                    //plantClone.transform.SetParent(myGardenObject.transform);
                    PlantItem pItem = new PlantItem();
                    pItem.plantLocation = hit.point;
                    pItem.plantStage = "New";
                    pItem.plantType = "Tulip";
                    myGarden.gardenPlants.Add(pItem);
                }
            }

        }
        */
        public void ReturnToMenu()
        {
            dataController.SaveGameData();
            dataController.SavePlayerProfile();
            SceneManager.LoadScene("scene-menu");
        }
    }
}
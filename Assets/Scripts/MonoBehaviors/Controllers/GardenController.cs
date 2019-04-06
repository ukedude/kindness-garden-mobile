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
       // Vector2d[] _locations;

        [SerializeField]
        float _spawnScale = 100f;

        [SerializeField]
        float _plantScale = 10f;

        [SerializeField]
        GameObject _markerPrefab;

        private DataController dataController;
        private Garden myGarden;
        private GameObject myGardenObject;
        private Vector3 gardenCenter;
        List<GameObject> _spawnedObjects;
        List<Vector2d> _locations;

        // Start is called before the first frame update
        void Start()
        {
            dataController = FindObjectOfType<DataController>();
            myGarden = dataController.myGarden;
            _spawnedObjects = new List<GameObject>();
            _locations = new List<Vector2d>();
            DisplayGardenOnMap();
            LoadGarden();
        }

        void DisplayGardenOnMap()
        {

            Vector2d location;
            
            location = Conversions.StringToLatLon(myGarden.gardenLatLong);
            _locations.Add(location);
            myGardenObject = Instantiate(_markerPrefab);
            gardenCenter = myGardenObject.transform.localPosition = _map.GeoToWorldPosition(location, true);
            myGardenObject.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
            _spawnedObjects.Add(myGardenObject);

        }
        void LoadGarden()
        {
            foreach (PlantItem pi in myGarden.gardenPlants)
            {
                Debug.Log("Plant type" + pi.plantType);
                var item = (Item)Resources.Load("Items/"+pi.plantType, typeof(Item));
                var instance = Instantiate(item.itemModel);
                instance.transform.localPosition = pi.plantLocation;
                instance.transform.localScale = new Vector3(_plantScale, _plantScale, _plantScale);
                instance.transform.SetParent(myGardenObject.transform);
                
                //Rigidbody plantClone = (Rigidbody)Instantiate(plant, pi.planLocation, Quaternion.identity);
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
        public void AddPlant(Item newItem)
        {
            Vector2d myLocationLatLon = LocationProvider.CurrentLocation.LatitudeLongitude;
            Vector2d gardenCenterLatLon = _locations[0];

            Vector3 myLocationWorld = _map.GeoToWorldPosition(myLocationLatLon, true);
            Vector3 gardenCenterWorld = _map.GeoToWorldPosition(gardenCenterLatLon, true);

            PlantItem pi = new PlantItem();
            pi.plantType = newItem.itemName;
            pi.plantStage = "New";
            pi.plantLocation = gardenCenterWorld - myLocationWorld;
            myGarden.gardenPlants.Add(pi);

            var item = (Item)Resources.Load("Items/" + pi.plantType, typeof(Item));
            var instance = Instantiate(item.itemModel);
            instance.transform.localPosition = pi.plantLocation;
            instance.transform.localScale = new Vector3(_plantScale, _plantScale, _plantScale);
            instance.transform.SetParent(myGardenObject.transform);

            dataController.SaveGameData();
        }
        public void ReturnToMenu()
        {
            dataController.SaveGameData();
            dataController.SavePlayerProfile();
            SceneManager.LoadScene("scene-menu");
        }
    }
}
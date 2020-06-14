using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BinSpawnLocationRandomiser : MonoBehaviour
{

    public List<Transform> locations;

    public int numOfBinsToSpawn;
    public GameObject bin;

    private void Start()
    {

        for (int i = 0; i < numOfBinsToSpawn; i++)
        {
            Debug.Log(locations.Count.ToString());
            int randomLocation = Random.Range(0, locations.Count);
            
            Instantiate(bin, locations[randomLocation]);
            locations.RemoveAt(randomLocation);
        }

    }

}

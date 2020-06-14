using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bin : MonoBehaviour
{
    [SerializeField]
    private Transform spawnLocation;

    public float timeOfLastSpawn { get; private set; }
    
    public bool hasObjectCurrently
    {
        get
        {
            return spawnLocation.childCount > 0;
        }
    }

    public void SpawnObject(GameObject prefab)
    {
        GameObject instance = GameObject.Instantiate(prefab);
        for (int index = 0; index < spawnLocation.childCount; index++)
            Destroy(spawnLocation.GetChild(index).gameObject);
        instance.transform.parent = spawnLocation;
        instance.transform.localPosition = Vector3.zero;
        instance.GetComponent<Rigidbody>().isKinematic = true;
        timeOfLastSpawn = Time.time;
    }
}

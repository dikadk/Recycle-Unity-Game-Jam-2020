using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBikeList : MonoBehaviour
{
    [SerializeField]
    private List<Bike> possBikes = new List<Bike>();
    
    public Bike GetRandomBike()
    {
        int count = possBikes.Count;
        Bike bike = possBikes[Random.Range(0, count )];
        return bike;
    }
}

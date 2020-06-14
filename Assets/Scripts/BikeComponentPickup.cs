using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ABXY.AssetLink;

public class BikeComponentPickup : MonoBehaviour
{
    [SerializeField]
    private R.BikeComponentTypes _componentType = R.BikeComponentTypes.Nothing;
    public R.BikeComponentTypes componentType { get { return _componentType; } }
}

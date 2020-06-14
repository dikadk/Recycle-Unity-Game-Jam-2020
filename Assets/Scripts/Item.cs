using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody),typeof(Collider))]
public class Item : MonoBehaviour
{
    public bool isHeld
    {
        get
        {
            Transform currentParent = transform.parent;
            return currentParent != null && currentParent.GetComponent<ItemSlot>() != null;
        }
    }
}

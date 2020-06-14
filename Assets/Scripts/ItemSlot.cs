using ABXY.AssetLink;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ItemSlot : MonoBehaviour
{
    public bool currentlyHoldingItem
    {
        get
        {
            return this.transform.childCount > 0;
        }
    }

    private List<Item> itemsInRange = new List<Item>();

    public void OnPickupPressed(InputAction.CallbackContext input)
    {
        TruncateItems();
        if (input.performed)
        {
            if (!currentlyHoldingItem && itemsInRange.Count > 0)// pickling up
            {
                Item item = itemsInRange[0];
                item.transform.parent = this.transform;
                item.GetComponent<Rigidbody>().isKinematic = true;
                item.transform.localPosition = Vector3.zero;
                item.transform.rotation = this.transform.rotation;
            }
            else if (currentlyHoldingItem)// dropping
            {
                Item item = this.GetComponentInChildren<Item>();
                if (item != null)
                {
                    item.transform.parent = null;
                    item.GetComponent<Rigidbody>().isKinematic = false;
                }
            }
        }
    }
    private void Awake()
    {
        GetComponent<SphereCollider>().radius = R.Character_Controller_Properties.GrabRadius;
    }

    private void TruncateItems()
    {
        for (int index = 0; index < itemsInRange.Count; index++)
            if (itemsInRange[index] == null)
                itemsInRange.RemoveAt(index);
    }

    private void OnTriggerEnter(Collider other)
    {
        Item item = other.GetComponent<Item>();
        if (item != null && !itemsInRange.Contains(item))
            itemsInRange.Add(item);
    }

    private void OnTriggerExit(Collider other)
    {
        Item item = other.GetComponent<Item>();
        if (item != null)
            itemsInRange.Remove(item);
    }
}

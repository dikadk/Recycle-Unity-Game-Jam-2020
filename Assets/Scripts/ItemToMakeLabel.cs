using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ItemToMakeLabel : MonoBehaviour
{
    private Material billboardMaterial;
    private Text text;

    private void Awake()
    {
        var materials = GetComponent<MeshRenderer>().materials;
        foreach (Material m in materials)
        {
            Debug.Log(m.name);
            if (m.name.StartsWith("billboard"))
                billboardMaterial = m;
        }
    }

    public void updateBillboardTexture(Texture itemToMakeTexture, int index, int total)
    {

        if (!billboardMaterial)
            return;

        billboardMaterial.SetTexture("_MainTex", itemToMakeTexture);

        text.text = string.Format("{0}/{1}", index, total);
    }
}

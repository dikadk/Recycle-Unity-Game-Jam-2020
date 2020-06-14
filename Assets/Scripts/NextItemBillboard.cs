using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class NextItemBillboard : MonoBehaviour
{
    [SerializeField]
    RawImage imageView;

    [SerializeField]
    private TextMeshProUGUI counter;

    public void OnNewItem(Texture2D image, int progress, int total)
    {
        imageView.texture = image;
        counter.text = progress + "/" + total;
    }
}

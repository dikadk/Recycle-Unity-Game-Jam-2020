using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelDrive : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    const float SpinSpeed = 5.0f;    
    void Update()    // Update is called once per frame
    {
        this.transform.Rotate(Vector3.down, SpinSpeed);
    }
}

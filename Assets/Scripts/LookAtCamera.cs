using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Camera camera = Camera.main != null ? Camera.main : Camera.current;
        if (camera != null)
        {
            Vector3 cameraVector = camera.transform.TransformDirection(Vector3.forward);
            this.transform.LookAt(this.transform.position + cameraVector);
        }
    }
}

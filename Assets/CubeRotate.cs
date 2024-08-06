using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeRotate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation *= Quaternion.Euler(0, Time.deltaTime * 10f, 0);
        transform.position += 0.0001f * Vector3.up * Mathf.Sin(Time.time * 0.1f);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slingshot : MonoBehaviour
{
    float compass_input;

    // Start is called before the first frame update
    void Start()
    {
        Input.compass.enabled = true;
        Input.location.Start();
    }

    // Update is called once per frame
    void Update()
    {
        compass_input = Input.compass.magneticHeading;
    }

    void FixedUpdate()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, compass_input, -90), Time.deltaTime * 3);
    }
}

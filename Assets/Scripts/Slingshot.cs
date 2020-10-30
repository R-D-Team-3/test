﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slingshot : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        Input.compass.enabled = true;
        Input.location.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        transform.rotation = Quaternion.Euler(0, Input.compass.magneticHeading, -90);
    }
}

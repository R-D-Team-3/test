using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Android;
using System.Collections.Specialized;
using System.Linq;
using System.Diagnostics;

public class Location : MonoBehaviour
{
    public Vector3 startPos;
    public Vector3 newPos;
    public float latitude;
    public float longitude;
    public Boolean firstTime = true;
    public Boolean isUpdating = false;
    public float[] latBuffer = new float[5];
    public float[] longBuffer = new float[5];
    public int location = 0;
    public int counter = 0;


    private void Start()
    {
        
    }

    private void FixedUpdate()
    {
        startPos = GameObject.Find("Player").transform.position;
        GameObject.Find("Player").transform.position = Vector3.MoveTowards(startPos, newPos*60000, Time.deltaTime);

        if (!isUpdating)
        {
           StartCoroutine(GetLocation());
        }
    }

    /// <summary>
    /// Coroutines using IEnumerator is Unity's built-in way of doing asynchronous function calls.
    /// 
    /// </summary>
    /// <returns></returns>
    IEnumerator GetLocation()
    {
        isUpdating = true;

        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
            Permission.RequestUserPermission(Permission.CoarseLocation);
        }
        // First, check if user has location service enabled
        if (!Input.location.isEnabledByUser)
            yield return new WaitForSeconds(10); //Basically: sleep for 10 seconds (only works in coroutines!)
        // Start service before querying location
        Input.location.Start();
        // Wait until service initializes
        int maxWait = 100;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }
        // Service didn't initialize in 10 seconds
        if (maxWait < 1)
        {
            UnityEngine.Debug.Log("Timed out");
            yield break;
        }
        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            UnityEngine.Debug.Log("Unable to determine device location");
            yield break;
        }
        else
        {
            // Access granted and location value could be retrieved
            LocationInfo gpsReadout = Input.location.lastData;

            
            latBuffer[location] = gpsReadout.latitude;
            longBuffer[location] = gpsReadout.longitude;
            location++;
            if(location == 5)
            {
                location = 0;
            }

            if (firstTime && counter<20)
            {
                latitude = gpsReadout.latitude;
                longitude = gpsReadout.longitude;
                UnityEngine.Debug.Log("first");
                for (int i = 0; i<5; i++)
                {
                    latBuffer[i] = latitude;
                    longBuffer[i] = longitude;
                    
                }
                counter++;
                firstTime = false;
            }
            for(int i = 0; i<latBuffer.Length; i++)
            {
                UnityEngine.Debug.Log(latBuffer[i]);
            }
            newPos = new Vector3(latitude - latBuffer.Average(), 0, longitude - longBuffer.Average());

            isUpdating = false;
            UnityEngine.Debug.Log("Location: " + gpsReadout.latitude + " " + gpsReadout.longitude);
            UnityEngine.Debug.Log("loc" + newPos.ToString("F8"));
        }

        // Stop service if there is no need to query location updates continuously
        // isUpdating = !isUpdating;
        //Input.location.Stop();
    }
}

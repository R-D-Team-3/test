using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;
using System;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.SceneManagement;



public class ImageRecognition : MonoBehaviour
{
    [SerializeField]
    public Text notificationText;

    private ARTrackedImageManager _aRTrackedImageManager;

    private float sliderCount;

    [SerializeField]
    private Slider slider;

    [SerializeField]
    private Text sliderText;


    private void Awake()
    {
        _aRTrackedImageManager = FindObjectOfType<ARTrackedImageManager>();
        sliderCount = 0;
        slider.value = sliderCount;
        sliderText.text = 0f * 100f + "%";
    }

    public void OnEnable()
    {
        _aRTrackedImageManager.trackedImagesChanged += OnImageChanged;
    }

    public void OnDisable()
    {
        _aRTrackedImageManager.trackedImagesChanged -= OnImageChanged;
    }

    public void OnImageChanged(ARTrackedImagesChangedEventArgs args)
    {
        ARTrackedImage trackedImage = null;

        for (int i = 0; i < args.added.Count; i++)
        {
            sliderCount += (float)0.005;
            slider.value = sliderCount;
            sliderText.text = Mathf.RoundToInt(sliderCount * 100f) + "%";
        }

        for (int i = 0; i < args.updated.Count; i++)
        {
            trackedImage = args.updated[i];
            if (trackedImage.trackingState == TrackingState.Tracking)
            {
                sliderCount += (float)0.005;
                slider.value = sliderCount;
                sliderText.text = Mathf.RoundToInt(sliderCount * 100f) + "%";
            }
            else
            {
                sliderCount = (float)0.00;
                slider.value = sliderCount;
                sliderText.text = Mathf.RoundToInt(sliderCount * 100f) + "%";
            }
        }

        for (int i = 0; i < args.removed.Count; i++)
        {
            sliderCount = (float)0.00;
            slider.value = sliderCount;
            sliderText.text = Mathf.RoundToInt(sliderCount * 100f) + "%";
        }

        if(slider.value == 1){
            //hier code om terug uit camerascene te gaan nadat scannen voltooid is
            
            StartCoroutine(SendNotification("Antenna succesfully captured!", 3));
            SceneManager.UnloadScene("ImageRecognitionScene");
        }

    }
    IEnumerator SendNotification(string text, int time)  //  <-  its a standalone method
    {
        notificationText.text = text;
        yield return new WaitForSeconds(time);
        notificationText.text = "";
    }
}

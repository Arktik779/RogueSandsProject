using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public Transform cameraTransform; // Reference to the camera's transform

    // Shake parameters
    public float shakeDuration = 0.5f;
    public float shakeMagnitude = 0.2f;
    private float shakeTimeRemaining;
    private Vector3 originalPosition;

    private void Start()
    {
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform; // Default to main camera
        }
        originalPosition = cameraTransform.localPosition;
    }

    private void Update()
    {
        if (shakeTimeRemaining > 0)
        {
            // Apply shake by adding a random offset
            cameraTransform.localPosition = originalPosition + Random.insideUnitSphere * shakeMagnitude;

            shakeTimeRemaining -= Time.deltaTime;
        }
        else if (shakeTimeRemaining <= 0 && cameraTransform.localPosition != originalPosition)
        {
            // Reset the camera to its original position
            cameraTransform.localPosition = originalPosition;
        }
    }

    public void TriggerShake(float duration, float magnitude)
    {
        shakeTimeRemaining = duration;
        shakeMagnitude = magnitude;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GoGo : MonoBehaviour
{
    #region Member Variables

    [Header("Transforms")] 
    public Transform head;
    public Transform rightController;
    public Transform rightHandMesh;
    public Transform leftController;
    public Transform leftHandMesh;

    [Header("Input Actions")]
    public InputActionProperty rightHandGrab;
    public InputActionProperty leftHandGrab;
    
    [Header("Collider")]
    public HandCollider rightHandCollider;
    public HandCollider leftHandCollider;

    [Header("Parameters")] 
    public float maxDistance = 9f;
    public float activationOffset = 0.35f;
    public float maxReachDistance = 1f;

    private GameObject rightGrabbedObject;
    private GameObject leftGrabbedObject;

    #endregion

    #region MonoBeaviour Callbacks

    private void Update()
    {
        UpdateHands();
        UpdateGrab();
    }

    #endregion

    #region Custom Methods

    public void UpdateHands()
    {
        // TODO Excercise 4.3
        // hand movement calculation
        float distance = Vector3.Distance(rightController.position, head.position);
        float newDistance;
        if(distance < activationOffset)
        {
            newDistance = distance;
            
        }
        else
        {
            newDistance = distance + 20* Mathf.Pow((distance - activationOffset),2);
            
        }
        Debug.Log(newDistance);
    }

    public void UpdateGrab()
    {
        // TODO Excercise 4.3
        // grab calculation
    }

    #endregion
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class Homer : MonoBehaviour
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

    [Header("HOMER Parameters")] 
    public LayerMask layerMask;
    public LineRenderer rightRay;
    public LineRenderer leftRay;
    
    private GameObject rightGrabbedObject;
    private GameObject leftGrabbedObject;
    
    // homer parameters
    private float initHeadHandDisRight;
    private float initHeadObjectDisRight;
    private float initHeadHandDisLeft;
    private float initHeadObjectDisLeft;

    #endregion

    #region MonoBehaviour Callbacks

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
    }
    
    public void UpdateGrab()
    {
        // TODO Excercise 4.3
        // grab calculation
    }

    #endregion
}

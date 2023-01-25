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
    public float headOffset = 0.2f;
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
        Vector3 adjustedHeadPosition = head.position;
        adjustedHeadPosition.y -= headOffset;
        
        // right Hand
        Vector3 headToRightHand = rightController.position - adjustedHeadPosition;

        float rightHandDistance = headToRightHand.magnitude;

        if (rightHandDistance > activationOffset)
        {
            float offsetFactor = Mathf.Clamp((rightHandDistance - activationOffset) / (maxReachDistance - activationOffset), 0, 1);
            
            float handOffset = maxDistance * Mathf.Pow(offsetFactor, 2);

            rightHandMesh.position = rightController.position + headToRightHand.normalized * handOffset;
        }
        else
        {
            rightHandMesh.transform.localPosition = Vector3.zero;
        }
        
        // left Hand
        Vector3 headToLeftHand = leftController.position - adjustedHeadPosition;

        float leftHandDistance = headToLeftHand.magnitude;

        if (leftHandDistance > activationOffset)
        {
            float offsetFactor =
                Mathf.Clamp((leftHandDistance - activationOffset) / (maxReachDistance - activationOffset), 0, 1);

            float handOffset = maxDistance * Mathf.Pow(offsetFactor, 2);

            leftHandMesh.position = leftController.position + headToLeftHand.normalized * handOffset;
        }
        else
        {
            leftHandMesh.transform.localPosition = Vector3.zero;
        }
    }

    public void UpdateGrab()
    {
        if (rightHandGrab.action.WasPressedThisFrame())
        {
            if (rightGrabbedObject == null && rightHandCollider.isColliding && rightHandCollider.collidingObject != leftGrabbedObject)
            {
                rightGrabbedObject = rightHandCollider.collidingObject;
                rightGrabbedObject.transform.SetParent(rightHandCollider.transform, true);
                rightGrabbedObject.GetComponent<MaterialHandler>().Grab(true);
            }
        }
        else if (rightHandGrab.action.WasReleasedThisFrame())
        {
            if (rightGrabbedObject != null)
            {
                rightGrabbedObject.GetComponent<MaterialHandler>().Grab(false);
                rightGrabbedObject.transform.SetParent(null, true);
                rightGrabbedObject = null;
            }
        }

        if (leftHandGrab.action.WasPressedThisFrame())
        {
            if (leftGrabbedObject == null && leftHandCollider.isColliding &&
                leftHandCollider.collidingObject != rightGrabbedObject)
            {
                leftGrabbedObject = leftHandCollider.collidingObject;
                leftGrabbedObject.transform.SetParent(leftHandCollider.transform, true);
                leftGrabbedObject.GetComponent<MaterialHandler>().Grab(true);
            }
        }
        else if (leftHandGrab.action.WasReleasedThisFrame())
        {
            if (leftGrabbedObject != null)
            {
                leftGrabbedObject.GetComponent<MaterialHandler>().Grab(false);
                leftGrabbedObject.transform.SetParent(null, true);
                leftGrabbedObject = null;
            }
        }
    }

    #endregion
}

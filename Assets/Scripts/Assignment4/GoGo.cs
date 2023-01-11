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

    private Transform savedParentTransform;

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
        //float distance = Vector3.Distance(rightController.position, head.position);
        float newDistance;
        Vector3 handRelativeTohead =  rightController.position - head.position;
        Vector3 direction = handRelativeTohead.normalized;
        float distance = handRelativeTohead.magnitude;
        float k = (maxDistance - maxReachDistance) / Mathf.Pow(maxReachDistance - activationOffset, 2);
        if (distance < activationOffset)
        {
            newDistance = distance;
            
        }
        else
        {
            newDistance = distance + k * Mathf.Pow((distance - activationOffset),2);  
        }
        rightHandMesh.position = 
            new Vector3(
                head.position.x + direction.x * newDistance,
                head.position.y + direction.y * newDistance,
                head.position.z + direction.z * newDistance
            );

        Debug.Log(Vector3.Distance(head.position , rightHandMesh.position));
        
    }

    public void UpdateGrab()
    {
        // TODO Excercise 4.3
        // grab calculation
        if (rightHandGrab.action.IsPressed())
        {
            if (rightGrabbedObject == null && rightHandCollider.isColliding &&
                rightHandCollider.collidingObject != leftGrabbedObject)
            {
                rightGrabbedObject = rightHandCollider.collidingObject;
                savedParentTransform = rightGrabbedObject.transform.parent;
                rightGrabbedObject.transform.SetParent(rightHandCollider.transform, true);

            }

        }
        else if (rightHandGrab.action.WasReleasedThisFrame() && rightGrabbedObject != null)
        {
            rightGrabbedObject.transform.SetParent(savedParentTransform, true);
            rightGrabbedObject = null;
        }
    }

    #endregion
}

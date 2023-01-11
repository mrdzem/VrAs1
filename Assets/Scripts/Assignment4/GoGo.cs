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

    private Transform savedRightParentTransform;
    private Transform savedLeftParentTransform;

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
        float newRightDistance;
        Vector3 rightHandRelativeTohead = rightController.position - head.position;
        Vector3 rightDirection = rightHandRelativeTohead.normalized;
        float rightDistance = rightHandRelativeTohead.magnitude;
        float rightK = (maxDistance - maxReachDistance) / Mathf.Pow(maxReachDistance - activationOffset, 2);

        float newLeftDistance;
        Vector3 leftHandRelativeTohead = leftController.position - head.position;
        Vector3 leftDirection = leftHandRelativeTohead.normalized;
        float leftDistance = leftHandRelativeTohead.magnitude;
        float leftK = (maxDistance - maxReachDistance) / Mathf.Pow(maxReachDistance - activationOffset, 2);


        if (rightDistance < activationOffset)
        {
            newRightDistance = rightDistance;
            
        }
        else
        {
            newRightDistance = rightDistance + rightK * Mathf.Pow((rightDistance - activationOffset),2);  
        }
        rightHandMesh.position = 
            new Vector3(
                head.position.x + rightDirection.x * newRightDistance,
                head.position.y + rightDirection.y * newRightDistance,
                head.position.z + rightDirection.z * newRightDistance
            );



        if (leftDistance < activationOffset)
        {
            newLeftDistance = leftDistance;

        }
        else
        {
            newLeftDistance = leftDistance + leftK * Mathf.Pow((leftDistance - activationOffset), 2);
        }
        leftHandMesh.position =
            new Vector3(
                head.position.x + leftDirection.x * newLeftDistance,
                head.position.y + leftDirection.y * newLeftDistance,
                head.position.z + leftDirection.z * newLeftDistance
            );


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
                savedRightParentTransform = rightGrabbedObject.transform.parent;
                rightGrabbedObject.transform.SetParent(rightHandCollider.transform, true);

            }

        }
        else if (rightHandGrab.action.WasReleasedThisFrame() && rightGrabbedObject != null)
        {
            rightGrabbedObject.transform.SetParent(savedRightParentTransform, true);
            rightGrabbedObject = null;
        }

        if (leftHandGrab.action.IsPressed())
        {
            if (leftGrabbedObject == null && leftHandCollider.isColliding &&
                leftHandCollider.collidingObject != rightGrabbedObject)
            {
                leftGrabbedObject = leftHandCollider.collidingObject;
                savedLeftParentTransform = leftGrabbedObject.transform.parent;
                leftGrabbedObject.transform.SetParent(leftHandCollider.transform, true);

            }

        }
        else if (leftHandGrab.action.WasReleasedThisFrame() && leftGrabbedObject != null)
        {
            leftGrabbedObject.transform.SetParent(savedLeftParentTransform, true);
            leftGrabbedObject = null;
        }
    }

    #endregion
}

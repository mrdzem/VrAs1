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
        Vector3 startPoint = head.position;
        startPoint.y -= 0.2f;
        
        // right hand
        Vector3 rightBodyHandRay = rightController.position - startPoint;

        if (rightGrabbedObject == null)
        {
            RaycastHit hit;
            if (Physics.Raycast(startPoint, rightBodyHandRay.normalized, out hit, Single.PositiveInfinity, layerMask))
            {
                rightHandMesh.position = hit.point;
                rightHandMesh.rotation = rightController.rotation;

                initHeadHandDisRight = rightBodyHandRay.magnitude;
                initHeadObjectDisRight = (hit.point - startPoint).magnitude;

                rightRay.enabled = true;
                rightRay.SetPositions(new Vector3[] {rightController.position, hit.point});
            }
            else
            {
                rightHandMesh.localPosition = Vector3.zero;
                rightHandMesh.localRotation = Quaternion.identity;
                
               rightRay.enabled = false;
            }
        }
        else
        {
            float headHandDis = (rightController.position - startPoint).magnitude;
            float distance = (headHandDis * initHeadObjectDisRight) / initHeadHandDisRight;

            rightHandMesh.position = rightController.position + (rightBodyHandRay.normalized * distance);
            rightHandMesh.rotation = rightController.rotation;

            rightRay.enabled = false;
        }
        
        // left hand
        Vector3 leftBodyHandRay = leftController.position - startPoint;

        if (leftGrabbedObject == null)
        {
            RaycastHit hit;
            if (Physics.Raycast(startPoint, leftBodyHandRay.normalized, out hit, Single.PositiveInfinity, layerMask))
            {
                leftHandMesh.position = hit.point;
                leftHandMesh.rotation = leftController.rotation;

                initHeadHandDisLeft = leftBodyHandRay.magnitude;
                initHeadObjectDisLeft = (hit.point - startPoint).magnitude;

                leftRay.enabled = true;
                leftRay.SetPositions(new Vector3[] {leftController.position, hit.point});
            }
            else
            {
                leftHandMesh.localPosition = Vector3.zero;
                leftHandMesh.localRotation = Quaternion.identity;
                
                leftRay.enabled = false;
            }
        }
        else
        {
            float headHandDis = (leftController.position - startPoint).magnitude;
            float distance = (headHandDis * initHeadObjectDisLeft) / initHeadHandDisLeft;

            leftHandMesh.position = leftController.position + (leftBodyHandRay.normalized * distance);
            leftHandMesh.rotation = leftController.rotation;

            leftRay.enabled = false;
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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class VirtualHands : MonoBehaviour
{
    #region Member Variables

    [Header("Input Actions")] 
    public InputActionProperty rightHandGrab;
    public InputActionProperty leftHandGrab;
    public InputActionProperty switchMode;
    
    [Header("Collider")]
    public HandCollider rightHandCollider;
    public HandCollider leftHandCollider;

    [Header("Parameters")] 
    public bool reparent;

    private GameObject rightGrabbedObject;
    private GameObject leftGrabbedObject;

    private Matrix4x4 rightOffsetMat;
    private Matrix4x4 leftOffsetMat;

    #endregion

    #region MonoBehaviour Callbacks

    private void Update()
    {
        if (switchMode.action.WasPressedThisFrame())
            reparent = !reparent;
        
        SnapGrab(); // comment out when implementing your solutions
        
        /*
        if (reparent)
        {
            ReparentingGrab();
        }
        else
        {
            GrabCalculation();
        }
        */
        
    }

    #endregion

    #region Custom Methods

    public void SnapGrab()
    {
        if (rightHandGrab.action.IsPressed())
        {
            if (rightGrabbedObject == null && rightHandCollider.isColliding &&
                rightHandCollider.collidingObject != leftGrabbedObject)
            {
                rightGrabbedObject = rightHandCollider.collidingObject;
            }
            else if (rightGrabbedObject != null)
            {
                rightGrabbedObject.transform.position = rightHandCollider.transform.position;
                rightGrabbedObject.transform.rotation = rightHandCollider.transform.rotation;
            }
        }
        else if (rightHandGrab.action.WasReleasedThisFrame())
        {
            rightGrabbedObject = null;
        }
    }

    public void ReparentingGrab()
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

    public void GrabCalculation()
    {
        if (rightHandGrab.action.IsPressed())
        {
            if (rightGrabbedObject == null && rightHandCollider.isColliding &&
                rightHandCollider.collidingObject != leftGrabbedObject)
            {
                // initial offset calculation
                rightGrabbedObject = rightHandCollider.collidingObject;
                rightOffsetMat = GetTransformationMatrix(rightHandCollider.transform, true).inverse *
                                GetTransformationMatrix(rightGrabbedObject.transform, true);
                
                rightGrabbedObject.GetComponent<MaterialHandler>().Grab(true);
            }
            else if (rightGrabbedObject != null)
            {
                Matrix4x4 newTransform = GetTransformationMatrix(rightHandCollider.transform, true) * rightOffsetMat;

                rightGrabbedObject.transform.position = newTransform.GetColumn(3);
                rightGrabbedObject.transform.rotation = newTransform.rotation;
            }
        }
        else if(rightGrabbedObject != null)
        {
            rightGrabbedObject.GetComponent<MaterialHandler>().Grab(false);
            rightGrabbedObject = null;
            rightOffsetMat = Matrix4x4.identity;
        }
        
        if (leftHandGrab.action.IsPressed())
        {
            if (leftGrabbedObject == null && leftHandCollider.isColliding &&
                leftHandCollider.collidingObject != rightGrabbedObject)
            {
                // initial offset calculation
                leftGrabbedObject = leftHandCollider.collidingObject;
                leftOffsetMat = GetTransformationMatrix(leftHandCollider.transform, true).inverse *
                                GetTransformationMatrix(leftGrabbedObject.transform, true);
                
                leftGrabbedObject.GetComponent<MaterialHandler>().Grab(true);
            }
            else if (leftGrabbedObject != null)
            {
                Matrix4x4 newTransform = GetTransformationMatrix(leftHandCollider.transform, true) * leftOffsetMat;

                leftGrabbedObject.transform.position = newTransform.GetColumn(3);
                leftGrabbedObject.transform.rotation = newTransform.rotation;
            }
        }
        else if(leftGrabbedObject != null)
        {
            leftGrabbedObject.GetComponent<MaterialHandler>().Grab(false);
            leftGrabbedObject = null;
            leftOffsetMat = Matrix4x4.identity;
        }
    }

    public Matrix4x4 GetTransformationMatrix(Transform t, bool world = true)
    {
        if (world)
        {
            return Matrix4x4.TRS(t.position, t.rotation, t.lossyScale);
        }
        else
        {
            return Matrix4x4.TRS(t.localPosition, t.localRotation, t.localScale);
        }
    }
    
    #endregion
}

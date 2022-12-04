using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FlyPad : MonoBehaviour
{
    public enum InputMapping
    {
        PositionControl,
        VelocityControl,
        AccelerationControl
    }

    [Header("Transforms")]
    public Transform padTransform;
    public Transform headTransform;

    public InputActionProperty centerPadUnderHeadAction;
    public InputActionProperty accelerationBrakeAction;

    [Header("General Settings")]
    public InputMapping inputMapping;
    [Range(0.0f, 0.9f)]
    [Tooltip("Describes a Deadzone Radius (around the flypad center) within which no input is applied to transform updates. Input is normalized based on this threshold.")]
    public float inputMagnitudeThreshold = 0.2f;

    [Header("Mapping Specific Variables")]
    public float positionControlScaleFactor = 30;
    public float maximumVelocity = 50;
    public float maximumAcceleration = 0.05f;
    public float accelerationBrakeScaleFactor = 0.1f;

    private Vector3 currentVelocity = Vector3.zero;
    private Vector3 padOrigin; // *Hint1 
    private Vector3 offsetToCenter;  // *Hint2 

    /*
     * *Hint1: set padOrigin whenever CenterPadUnderHead() is called 
     *    -> Use it for your PositionControl(...) implementation
     * *Hint2: set offsetToCenter whenever CenterPadUnderHead() is called 
     *    -> offsetToCenter should describe the [x,z]-offset between headTransform.position and transform.position.
     *       The offset needs to be applied as a final step in your PositionControl(...) implementation.
     *       Otherwise, your actual jump position will not align with the preview avatar position.
     */

    private void Start()
    {
        CenterPadUnderHead();
    }

    private void Update()
    {
        if (centerPadUnderHeadAction.action.WasPressedThisFrame())
            CenterPadUnderHead();
        else
            EvaluateInput();
    }

    public void CenterPadUnderHead()
    {
        // Task 3.1 TODO 
        padOrigin = padTransform.position;
        padTransform.position += new Vector3(CalculateUserPadPosition().x, 0, CalculateUserPadPosition().y);
        currentVelocity = Vector3.zero;

    }

    private void EvaluateInput()
    {
        Vector2 userPadPosition = CalculateUserPadPosition();

        switch (inputMapping)
        {
            case InputMapping.PositionControl:
                PositionControl(userPadPosition);
                break;
            case InputMapping.VelocityControl:
                VelocityControl(userPadPosition);
                break;
            case InputMapping.AccelerationControl:
                AccelerationControl(userPadPosition);
                break;
        }
    }

    private void PositionControl(Vector2 userPadPosition)
    {
        // Task 3.1 TODO 
        transform.position = new Vector3(padOrigin.x + userPadPosition.x*positionControlScaleFactor, padOrigin.y, padOrigin.z + userPadPosition.y * positionControlScaleFactor);
        
    }

    private void VelocityControl(Vector2 userPadPosition)
    {
        // Task 3.1 TODO
        if (CalculateScaledInputMagnitude(CalculateUserPadPosition()) > 0)
        {
            transform.position = new Vector3(
                transform.position.x + userPadPosition.x * maximumVelocity * Time.deltaTime, 
                transform.position.y, 
                transform.position.z + userPadPosition.y * maximumVelocity * Time.deltaTime
            );
        }
        

    }

    private void AccelerationControl(Vector2 userPadPosition)
    {
        // Task 3.1 (3.2 optional) TODO
        if (CalculateScaledInputMagnitude(CalculateUserPadPosition()) > 0)
        {
            float x_break = currentVelocity.x * userPadPosition.x;
            float z_break = currentVelocity.z * userPadPosition.y;
            if (x_break < 0 && z_break < 0)
            {
                currentVelocity = new Vector3(
                currentVelocity.x + userPadPosition.x * accelerationBrakeScaleFactor * Time.deltaTime,
                transform.position.y,
                currentVelocity.z + userPadPosition.y * accelerationBrakeScaleFactor * Time.deltaTime
                );
            }
            else if(x_break >= 0 && z_break < 0)
            {
                currentVelocity = new Vector3(
                currentVelocity.x + userPadPosition.x * maximumAcceleration * Time.deltaTime,
                transform.position.y,
                currentVelocity.z + userPadPosition.y * accelerationBrakeScaleFactor * Time.deltaTime
                );
            }
            else if (x_break < 0 && z_break >= 0)
            {
                currentVelocity = new Vector3(
                currentVelocity.x + userPadPosition.x * accelerationBrakeScaleFactor * Time.deltaTime,
                transform.position.y,
                currentVelocity.z + userPadPosition.y * maximumAcceleration * Time.deltaTime
                );
            }
            else if (x_break >= 0 && z_break >= 0)
            {
                currentVelocity = new Vector3(
                currentVelocity.x + userPadPosition.x * maximumAcceleration * Time.deltaTime,
                transform.position.y,
                currentVelocity.z + userPadPosition.y * maximumAcceleration * Time.deltaTime
                );
            }

            
        }
        
        transform.position = new Vector3(
                transform.position.x + currentVelocity.x,
                transform.position.y,
                transform.position.z + currentVelocity.z
            );
    }

    // Returns the users position relative to the pads origin on the xz-Plane ([x,z] are in Range [-1,1])
    //    Does not account for inputMagnitudeThreshold (see instead CalculateScaledInputMagnitude())
    private Vector2 CalculateUserPadPosition()
    {
        float userXPos = Mathf.Clamp((headTransform.position.x - padTransform.position.x) / (padTransform.lossyScale.x / 2), -1f, 1f);
        float userZPos = Mathf.Clamp((headTransform.position.z - padTransform.position.z) / (padTransform.lossyScale.x / 2), -1f, 1f);

        return new Vector2(userXPos, userZPos);
    }

    // Returns the users input magnitude scaled between [0, 1] depending on inputMagnitudeThreshold
    private float CalculateScaledInputMagnitude(Vector2 userPadPosition)
    {
        var clampedMagnitude = Mathf.Clamp(userPadPosition.magnitude, 0f, 1f);
        return clampedMagnitude < inputMagnitudeThreshold ? 0 : (clampedMagnitude - inputMagnitudeThreshold) / (1 - inputMagnitudeThreshold);
    }

}

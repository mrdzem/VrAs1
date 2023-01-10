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
    private Vector3 padOrigin;
    private Vector3 offsetToCenter;

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
        var padPosition = padTransform.position;
        padPosition.x = headTransform.position.x;
        padPosition.z = headTransform.position.z;
        padTransform.position = padPosition;
        padOrigin = padPosition;
        offsetToCenter = transform.position - headTransform.position;
        offsetToCenter.y = transform.position.y;
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
        float inputMagnitude = CalculateScaledInputMagnitude(userPadPosition);
        userPadPosition.Normalize();
        var padOriginXZDisplacement = positionControlScaleFactor * inputMagnitude * userPadPosition;
        transform.position = new Vector3(
            padOrigin.x + padOriginXZDisplacement.x,
            transform.position.y,
            padOrigin.z + padOriginXZDisplacement.y
        );
        transform.position += offsetToCenter;
    }

    private void VelocityControl(Vector2 userPadPosition)
    {
        float inputMagnitude = CalculateScaledInputMagnitude(userPadPosition);
        if (inputMagnitude <= 0)
        {
            currentVelocity = Vector3.zero;
            return;
        }
        
        var direction = new Vector3(userPadPosition.x, 0f, userPadPosition.y);
        float speed = maximumVelocity * inputMagnitude;
        currentVelocity = direction.normalized * (speed * Time.deltaTime);
        transform.Translate(currentVelocity, Space.World);
    }

    private void AccelerationControl(Vector2 userPadPosition)
    {
        float brakeInput = accelerationBrakeAction.action.ReadValue<float>();
        if(brakeInput > 0.1)
        {
            brakeInput = accelerationBrakeScaleFactor * ((brakeInput - 0.1f) / (1.0f - 0.1f));
            currentVelocity -= brakeInput * Time.deltaTime * currentVelocity.normalized;
        }
        else
        {
            float inputMagnitude = CalculateScaledInputMagnitude(userPadPosition);
            Vector3 direction = new Vector3(userPadPosition.x, 0f, userPadPosition.y);
            float acceleration = maximumAcceleration * inputMagnitude;
            currentVelocity += direction.normalized * (acceleration * Time.deltaTime);
        }

        if(currentVelocity.magnitude > maximumVelocity)
            currentVelocity = currentVelocity.normalized * maximumVelocity;
        
        transform.Translate(currentVelocity, Space.World);
    }

    // Returns the users position relative to the pads origin on the xz-Plane (x and z vary between -1 and 1)
    private Vector2 CalculateUserPadPosition()
    {
        float userXPos = Mathf.Clamp((headTransform.position.x - padTransform.position.x) / (padTransform.lossyScale.x / 2), -1f, 1f);
        float userZPos = Mathf.Clamp((headTransform.position.z - padTransform.position.z) / (padTransform.lossyScale.x / 2), -1f, 1f);

        return new Vector2(userXPos, userZPos);
    }

    private float CalculateScaledInputMagnitude(Vector2 userPadPosition)
    {
        var clampedMagnitude = Mathf.Clamp(userPadPosition.magnitude, 0f, 1f);
        return clampedMagnitude < inputMagnitudeThreshold ? 0 : (clampedMagnitude - inputMagnitudeThreshold) / (1 - inputMagnitudeThreshold);
    }

}

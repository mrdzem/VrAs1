using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class AnchoredJumpingNavigation : MonoBehaviour
{
    public InputActionProperty jumpAction;
    public InputActionProperty resetAction;

    public GameObject previewAvatarPrefab;
    public Material previewMaterial;

    private bool rayIsActive = false;
    private bool anchoredJumpIsActive = false;
    private GameObject rightController;
    private GameObject xrUserCamera;
    private GameObject previewHitpoint;
    private GameObject pointOfInterest;
    private GameObject previewAvatar;
    private XRInteractorLineVisual lineVisual;
    private RaycastHit hit;

    private Vector3 startPosition;
    private Quaternion startRotation;
    private Vector3 currentHitPosition;
    private Vector3 pointOfInterestPosition;
    private Vector3 jumpingTargetPosition;

    private LineRenderer offsetRenderer;

    public LayerMask myLayerMask;

    public float rayActivationThreshhold = 0.01f;
    public float jumpActivationThreshhold = 0.9f;

    public UnityEvent jumpPerformed = new UnityEvent();


    private void Awake()
    {
        rightController = GameObject.Find("RightHand Controller");
        xrUserCamera = GameObject.Find("Main Camera");
        previewHitpoint = InitPreviewHitpoint();
        pointOfInterest = InitPointOfInterest();
        previewAvatar = Instantiate(previewAvatarPrefab, Vector3.zero, Quaternion.identity);
        previewAvatar.SetActive(false);
        lineVisual = GameObject.Find("RightHand Controller").GetComponent<XRInteractorLineVisual>();

        if (offsetRenderer == null)
        {
            offsetRenderer = this.gameObject.GetComponent<LineRenderer>();
            if (offsetRenderer == null)
            {
                offsetRenderer = this.gameObject.AddComponent<LineRenderer>();
            }
        }
        offsetRenderer.startWidth = 0.01f;
        offsetRenderer.positionCount = 2;

        startPosition = this.transform.position;
        startRotation = this.transform.rotation;
    }

    // Start is called before the first frame update
    void Start()
    {
        lineVisual.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateOffsetToCenter();

        float jumpActionValue = jumpAction.action.ReadValue<float>();
        if (jumpActionValue > rayActivationThreshhold && !rayIsActive)
        {
            rayIsActive = true;
            lineVisual.enabled = rayIsActive;
        }
        else if (jumpActionValue < rayActivationThreshhold && rayIsActive)
        {
            rayIsActive = false;
            lineVisual.enabled = rayIsActive;
        }

        if (rayIsActive)
        {
            Vector3 controllerPos = rightController.transform.position;
            Vector3 controllerDir = rightController.transform.TransformDirection(Vector3.forward) * 10;
            if (Physics.Raycast(controllerPos, controllerDir, out hit, Mathf.Infinity, myLayerMask))
            {
                currentHitPosition = hit.point;

                // jump action triggered -> show avatar
                if (jumpActionValue > jumpActivationThreshhold && !anchoredJumpIsActive)
                {
                    anchoredJumpIsActive = true;
                    
                    // Set Anchor (your point of interest)
                    SetPointOfInterest(currentHitPosition);
                    ShowPointOfInterest();

                    ShowHitpoint(currentHitPosition);
                    // Show Avatar
                    float userHeight = xrUserCamera.transform.position.y - this.transform.position.y;
                    Quaternion avatarOrientation = Quaternion.LookRotation(xrUserCamera.transform.position - new Vector3(currentHitPosition.x, xrUserCamera.transform.position.y, currentHitPosition.z));
                    ShowAvatar(currentHitPosition + new Vector3(0, userHeight, 0), avatarOrientation);
                }
                else if (anchoredJumpIsActive) // jump action active -> update
                {
                    SetJumpingTarget(currentHitPosition);

                    ShowHitpoint(currentHitPosition);

                    // Update Avatar
                    float userHeight = xrUserCamera.transform.position.y - this.transform.position.y;
                    Vector3 previewAvatarPosition = new Vector3(currentHitPosition.x, currentHitPosition.y + userHeight, currentHitPosition.z);
                    Quaternion avatarOrientation = Quaternion.LookRotation(previewAvatar.transform.position - new Vector3(pointOfInterestPosition.x, previewAvatar.transform.position.y, pointOfInterestPosition.z));
                    ShowAvatar(previewAvatarPosition, avatarOrientation);
                }
                else
                {
                    ShowHitpoint(currentHitPosition);
                }
            }
            else
            {
                HideHitpoint();
                HidePointOfInterest();
                HideAvatar();
            }
        }
        else
        {
            HideHitpoint();
            HidePointOfInterest();
            HideAvatar();
        }

        // jump action triggered and released -> perform jump
        if (anchoredJumpIsActive && jumpActionValue < jumpActivationThreshhold)
        {
            PerformJump();
            anchoredJumpIsActive = false;

            HideHitpoint();
            HidePointOfInterest();
            HideAvatar();
        }

        if (resetAction.action.WasPressedThisFrame())
        {
            ResetXROrigin();
        }
    }

    private void UpdateOffsetToCenter()
    {
        // Calculate the offset between the platform center and the camera in the xz plane
        Vector3 a = this.transform.position;
        Vector3 b = new Vector3(xrUserCamera.transform.position.x, this.transform.position.y, xrUserCamera.transform.position.z);

        // visualize the offset as a line on the ground
        offsetRenderer.positionCount = 2; // line renderer visualizes a line between N (here 2) vertices
        offsetRenderer.SetPosition(0, a); // set pos 1
        offsetRenderer.SetPosition(1, b); // set pos 2
    }

    private void PerformJump()
    {
        // Matrix Solution
        Quaternion goalOrientation = Quaternion.LookRotation(new Vector3(pointOfInterestPosition.x, previewAvatar.transform.position.y, pointOfInterestPosition.z) - previewAvatar.transform.position);
        Vector3 goalRotY = new Vector3(0f, goalOrientation.eulerAngles.y, 0f);
        Matrix4x4 goalMat = Matrix4x4.TRS(jumpingTargetPosition, Quaternion.Euler(goalRotY), new Vector3(1, 1, 1));

        Vector3 headYRot = new Vector3(0f, xrUserCamera.transform.localRotation.eulerAngles.y, 0f);
        Vector3 headXZPos = new Vector3(xrUserCamera.transform.localPosition.x, 0f, xrUserCamera.transform.localPosition.z);
        Matrix4x4 headMat = Matrix4x4.TRS(headXZPos, Quaternion.Euler(headYRot), new Vector3(1, 1, 1));
        Matrix4x4 newMat = goalMat * Matrix4x4.Inverse(headMat);
        transform.position = newMat.GetColumn(3);
        transform.rotation = newMat.rotation;
        transform.localScale = newMat.lossyScale;

        jumpPerformed.Invoke();

        // Calculate required turn angle
        //Quaternion camLocalRot = xrUserCamera.transform.localRotation;
        //Quaternion platformLocalRot = transform.localRotation;
        //Quaternion avatarOrientation = Quaternion.LookRotation(new Vector3(currentHitPosition.x, previewAvatar.transform.position.y, currentHitPosition.z) - previewAvatar.transform.position);
        //float goalAngle = avatarOrientation.eulerAngles.y;
        //float turnAngle = goalAngle - (camLocalRot.eulerAngles.y + platformLocalRot.eulerAngles.y);

        //// Rotate platform
        //transform.Rotate(0f, turnAngle, 0f);

        //// Adjust offset to new platform orientation
        //Vector3 a = this.transform.position;
        //Vector3 b = new Vector3(xrUserCamera.transform.position.x, this.transform.position.y, xrUserCamera.transform.position.z);
        //centerOffset = b - a;

        //// jump to new position and apply offset
        //transform.position = jumpingTargetPosition - centerOffset;
    }
    private void SetJumpingTarget(Vector3 targetPos)
    {
        jumpingTargetPosition = targetPos;
    }

    private void SetPointOfInterest(Vector3 targetPos)
    {
        pointOfInterestPosition = targetPos;
    }

    private GameObject InitPreviewHitpoint()
    {
        GameObject hitpoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        hitpoint.transform.localScale = new Vector3(0.08f, 0.08f, 0.08f);
        hitpoint.GetComponent<Renderer>().material = previewMaterial;
        hitpoint.SetActive(false);
        return hitpoint;
    }
    
    private GameObject InitPointOfInterest()
    {
        GameObject poi = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        poi.transform.localScale = new Vector3(0.08f, 2f, 0.08f);
        poi.GetComponent<Renderer>().material = previewMaterial;
        poi.SetActive(false);
        return poi;
    }

    private void ShowHitpoint(Vector3 worldPos)
    {
        previewHitpoint.SetActive(true); // show
        previewHitpoint.transform.position = worldPos;
    }

    private void HideHitpoint()
    {
        previewHitpoint.SetActive(false); // hide
    }
    
    private void ShowPointOfInterest()
    {
        pointOfInterest.SetActive(true); // show
        pointOfInterest.transform.position = pointOfInterestPosition;
    }

    private void HidePointOfInterest()
    {
        pointOfInterest.SetActive(false); // hide
    }

    private void ShowAvatar(Vector3 worldPos, Quaternion worldRot)
    {
        previewAvatar.SetActive(true); // show
        previewAvatar.transform.position = worldPos;
        previewAvatar.transform.rotation = worldRot;
    }

    private void HideAvatar()
    {
        previewAvatar.SetActive(false); // hide
    }

    private void ResetXROrigin()
    {
        transform.position = startPosition;
        transform.rotation = startRotation;
    }
}

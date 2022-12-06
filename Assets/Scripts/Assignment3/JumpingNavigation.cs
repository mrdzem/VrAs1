using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class JumpingNavigation : MonoBehaviour
{
    public enum JumpingState
    {
        Idle,
        ShowRay,
        TargetSet,
        PerformJump
    }

    public InputActionProperty jumpAction;
    public InputActionProperty resetAction;

    public GameObject previewAvatarPrefab;
    public Material previewMaterial;

    private JumpingState currentState = JumpingState.Idle;
    private bool rayIsActive = false;
    private GameObject rightController;
    private GameObject xrUserCamera;
    private GameObject previewHitpoint;
    private GameObject previewAvatar;
    private XRInteractorLineVisual lineVisual;
    private RaycastHit hit;

    private Vector3 startPosition;
    private Quaternion startRotation;
    private Vector3 currentHitPosition;
    private Vector3 jumpingTargetPosition;

    private LineRenderer debugOffsetRenderer;

    public LayerMask myLayerMask;
    public LayerMask jumpingLayerMask;

    public float rayActivationThreshhold = 0.01f;
    public float jumpActivationThreshhold = 0.9f;

    public UnityEvent jumpPerformed = new UnityEvent();

    private void Awake()
    {
        rightController = GameObject.Find("RightHand Controller");
        xrUserCamera = GameObject.Find("Main Camera");
        previewHitpoint = InitPreviewHitpoint();
        previewAvatar = Instantiate(previewAvatarPrefab, Vector3.zero, Quaternion.identity);
        previewAvatar.SetActive(false);
        lineVisual = rightController.GetComponent<XRInteractorLineVisual>();

        SetupDebugOffsetRenderer();

        startPosition = this.transform.position;
        startRotation = this.transform.rotation;
        
        currentState = JumpingState.Idle;
    }

    // Start is called before the first frame update
    void Start()
    {
        lineVisual.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        DebugUpdateOffsetToCenter(); // This helper function visualizes 

        float jumpActionValue = jumpAction.action.ReadValue<float>();
        if (jumpActionValue > rayActivationThreshhold && currentState == JumpingState.Idle)
        {
            currentState = JumpingState.ShowRay;
            lineVisual.enabled = true;
        } else if (jumpActionValue < rayActivationThreshhold && currentState == JumpingState.ShowRay)
        {
            currentState = JumpingState.Idle;
            lineVisual.enabled = false;
        }

        // Task 3.3 TODO
        if(rayActivationThreshhold < jumpActionValue && jumpActivationThreshhold > jumpActionValue)
        {
            previewHitpoint.SetActive(true);
            Physics.Raycast(rightController.transform.position, rightController.transform.forward, out hit, Mathf.Infinity ,jumpingLayerMask);
            previewHitpoint.transform.position = hit.point;
            jumpingTargetPosition = hit.point;
        }
        else if(jumpActivationThreshhold < jumpActionValue)
        {
            Physics.Raycast(rightController.transform.position, rightController.transform.forward, out hit, Mathf.Infinity, jumpingLayerMask);
            previewAvatar.SetActive(true);
            previewAvatar.transform.position = jumpingTargetPosition + new Vector3(0,1,0);
            Vector3 targetPos = hit.point;
            targetPos.y = previewAvatar.transform.position.y;
            Vector3 targetPosDirection = previewAvatar.transform.position - targetPos;
            previewAvatar.transform.rotation = Quaternion.LookRotation(targetPosDirection, Vector3.up);
        }
        else
        {
            previewHitpoint.SetActive(false);
            previewAvatar.SetActive(false);
        }
        


        if (resetAction.action.WasPressedThisFrame())
        {
            ResetXROrigin();
        }
    }



    private void PerformJump()
    {
        // Task 3.3 TODO
        

        jumpPerformed.Invoke();
    }

    private GameObject InitPreviewHitpoint()
    {
        GameObject hitpoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        hitpoint.transform.localScale = new Vector3(0.08f, 0.08f, 0.08f);
        hitpoint.GetComponent<Renderer>().material = previewMaterial;
        hitpoint.SetActive(false);
        return hitpoint;
    }

    private void SetupDebugOffsetRenderer()
    {
        // Render a line between your platform origin and your camera position (projected to the XZ plane)
        if (debugOffsetRenderer == null)
        {
            debugOffsetRenderer = this.gameObject.GetComponent<LineRenderer>();
            if (debugOffsetRenderer == null)
            {
                debugOffsetRenderer = this.gameObject.AddComponent<LineRenderer>();
            }
        }
        debugOffsetRenderer.startWidth = 0.01f;
        debugOffsetRenderer.positionCount = 2;
    }

    private void DebugUpdateOffsetToCenter()
    {
        // Calculate the offset between the platform center and the camera in the xz plane
        Vector3 a = this.transform.position;
        Vector3 b = new Vector3(xrUserCamera.transform.position.x, this.transform.position.y, xrUserCamera.transform.position.z);

        // visualize the offset as a line on the ground
        debugOffsetRenderer.positionCount = 2; // line renderer visualizes a line between N vertices (here 2 vertices)
        debugOffsetRenderer.SetPosition(0, a); // set pos 1
        debugOffsetRenderer.SetPosition(1, b); // set pos 2
    }

    // Task 3.3 TODO
    

    private void ResetXROrigin()
    {
        transform.position = startPosition;
        transform.rotation = startRotation;
    }
}

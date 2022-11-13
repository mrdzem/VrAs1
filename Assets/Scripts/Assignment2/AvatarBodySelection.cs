using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AvatarBodySelection : MonoBehaviour
{
    public List<GameObject> bodyPrefabs = new List<GameObject>();

    public InputActionProperty switchBodyAction; //Joystick LR

    public LayerMask mirrorLayer;

    public Transform headTransform;

    private GameObject bodyInstance;

    private int currentBodyIndex = 0;

    private bool disableInputHandling = false;

    private int layerMask = 1 << 7;


    // Start is called before the first frame update
    void Start()
    {
        AttachBodyPrefab(0);
    }

    // Update is called once per frame
    void Update()
    {
            int nextBodyIndex = CalcNextBodyIndex();
        if(nextBodyIndex != -1 && IsLookingAtMirror())
            AttachBodyPrefab(nextBodyIndex);
    }

    int CalcNextBodyIndex() // -1 means invalid aka "do nothing"
    {
        
        if (switchBodyAction.action.WasPressedThisFrame() && IsLookingAtMirror())
        {
            if (switchBodyAction.action.ReadValue<Vector2>().x > 0)
            {
                currentBodyIndex++;
            }
            else if (switchBodyAction.action.ReadValue<Vector2>().x < 0)
            {
                currentBodyIndex--;
                if(currentBodyIndex == -1)
                {
                    currentBodyIndex = bodyPrefabs.Count - 1;
                }

            }

            Debug.Log(currentBodyIndex % bodyPrefabs.Count);
        }

        
        return currentBodyIndex % bodyPrefabs.Count;
    }

    bool IsLookingAtMirror()
    {
        RaycastHit hit;     
        return Physics.Raycast(this.transform.parent.position, this.transform.parent.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask);
    }

    private void AttachBodyPrefab(int index)
    {
        if(index < 0 || index >= bodyPrefabs.Count)
        {
            Debug.LogWarning("Body selection offers no prefab at given index '" + index + "'");
            return;
        }

        if(bodyInstance != null)
        {
            Destroy(bodyInstance);
            bodyInstance = null;
        }

        currentBodyIndex = index;
        bodyInstance = Instantiate(bodyPrefabs[currentBodyIndex], transform);
    }
}

using System.Collections;using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class GameScript : MonoBehaviour
{
    public GameObject GameSphere;
    public InputActionProperty gameActivation;

    private List<GameObject> spheres = new List<GameObject>();
    private List<Vector3> sphereDistances = new List<Vector3>();
    private float radius = 0.6f;

    public GameObject rightHandCollider;
    public GameObject leftHandCollider;

    public InputActionProperty rightHandGrab;
    public InputActionProperty leftHandGrab;

    private bool isGrabbed = false;

    // Start is called before the first frame update
    void Start()
    {
        //Generate game spheres
        for(int i = 0; i < 5; i++)
        {
            var instance = Instantiate(GameSphere, this.transform);
            spheres.Add(instance);
            sphereDistances.Add(new Vector3(0,0,0));
            spheres[i].SetActive(false);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        //Updating position of game spheres
        if (gameActivation.action.WasPressedThisFrame())
        {
            for(int i = 0; i < spheres.Count; i++)
            {
                if (spheres[i].activeSelf)
                {
                    spheres[i].SetActive(false);
                }
                else
                {
                    spheres[i].SetActive(true);
                }

                float cornerAngle =

                    -(2f * Mathf.PI + ((2f * Mathf.PI) / ((float)spheres.Count * 3f) * i)
                    - 23f / 30f * Mathf.PI
                    + this.transform.eulerAngles.y*Mathf.Deg2Rad);
                Debug.Log(this.transform.rotation.y);

                Vector3 currentCorner = new Vector3(Mathf.Cos(cornerAngle) * radius, -0.3f, Mathf.Sin(cornerAngle) * radius) + this.transform.position;

                spheres[i].transform.position =  currentCorner;
                spheres[0].GetComponent<Renderer>().material.color = Color.blue;
                spheres[spheres.Count-1].GetComponent<Renderer>().material.color = Color.yellow;

                sphereDistances[i] = this.transform.position - spheres[i].transform.position;
            } 
        }
        for (int i = 0; i < spheres.Count; i++)
        {
            spheres[i].transform.position = this.transform.position - sphereDistances[i] + spheres[i].transform.forward*0.2f;
        }

        if(rightHandGrab.action.WasPressedThisFrame()){

            if (rightHandCollider.GetComponent<Collider>().bounds.Intersects(spheres[0].GetComponent<Collider>().bounds))
            {

            }

            

        }






    }
}

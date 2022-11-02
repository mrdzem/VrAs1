using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VegetationGenerator : MonoBehaviour
{

    [Serializable]
    public struct vegetationPrefabsStruct
    {
        public GameObject prefab;
        public int numObject;
    }

    [SerializeField]
    private List<vegetationPrefabsStruct> prefabs = new List<vegetationPrefabsStruct>();

    public List<GameObject> vegetationPrefabs = new List<GameObject>();

    private List<GameObject> instances = new List<GameObject>();

    public List<Collider> restrictedBounds = new List<Collider>();

    public int numObjects = 30;

    public Vector3 vegetationBoundsMin = new Vector3(-30, 0, -30);

    public Vector3 vegetationBoundsMax = new Vector3(30, 0, 30);

    public bool reset = false;

    

    // Start is called before the first frame update
    void Start()
    {
        GenerateVegetation();
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: Exercise 1.2 -> 3.)
        // check & handle "reset" to regenerate vegetation instances

        if (reset)
        {
            reset = false;
            ClearVegetation();
            GenerateVegetation();
        }
    }

    void ClearVegetation()
    {
        // TODO: part of Exercise 1.2 -> 3.)
        for (int i = instances.Count; i > 0; i--)
        {
            GameObject toDestroy = instances[i - 1];
            Destroy(toDestroy);
            instances.Remove(instances[i-1]);
        }
    }

    void GenerateVegetation()
    {
        // TODO: Exercise 1.2 -> 1.)
        // Instantiate & transform random "vegetationPrefab"

        // your code here

        // WITHOUT BONUS //

        //for (int i = 0; i < numObjects; i++)
        //{
        //    int randomIndex = UnityEngine.Random.Range(0, vegetationPrefabs.Count);

        //    GameObject somePlant = Instantiate(vegetationPrefabs[randomIndex], this.gameObject.transform);

        //    somePlant.transform.position = new Vector3(
        //        UnityEngine.Random.Range(vegetationBoundsMin.x, vegetationBoundsMax.x),
        //        0,
        //        UnityEngine.Random.Range(vegetationBoundsMin.z, vegetationBoundsMax.z)
        //    );
        //    somePlant.transform.rotation = new Quaternion(0, UnityEngine.Random.Range(0, 360), 0, 0);
        //    instances.Add(somePlant);
        //}

        // WITH BONUS //
        foreach(vegetationPrefabsStruct pre in prefabs)
        {
            for (int i = 0; i < pre.numObject; i++)
            {

                GameObject somePlant = Instantiate(
                    pre.prefab,
                    new Vector3(
                        UnityEngine.Random.Range(vegetationBoundsMin.x, vegetationBoundsMax.x),
                        0,
                        UnityEngine.Random.Range(vegetationBoundsMin.z, vegetationBoundsMax.z)
                    ),
                    new Quaternion(
                        0,
                        UnityEngine.Random.Range(-1.0f, 1.0f),
                        0,
                        UnityEngine.Random.Range(-1.0f, 1.0f)
                    ),
                    this.gameObject.transform);

                instances.Add(somePlant);
            }
        }
        

        // Collisions need to be resolved at a later time,
        // because Unity physics loop (Unity-internal evaluation of collisions)
        // runs separate from Update() loop
        StartCoroutine(ResolveCollisions());
    }

    IEnumerator ResolveCollisions()
    {
        yield return new WaitForSeconds(2);
        bool resolveAgain = false;

        // TODO: Exercise 1.2 -> 2.)
        // check & handle bounds intersection of each instance with "restrictedBounds"

        // your code here

        for (int i = 0; i < instances.Count; i++)
        {
            Collider instanceCollider = instances[i].GetComponent<Collider>();
            if (IsInRestrictedBounds(instanceCollider))
            {
                resolveAgain = true;
                instances[i].transform.position = new Vector3(
                    UnityEngine.Random.Range(vegetationBoundsMin.x, vegetationBoundsMax.x),
                    0,
                    UnityEngine.Random.Range(vegetationBoundsMin.z, vegetationBoundsMax.z)
                );
            }

        }
        // resolve again (delayed), after new random transform applied to colliding instances
        if (resolveAgain) StartCoroutine(ResolveCollisions());
    }

    bool IsInRestrictedBounds(Collider co)
    {
        // TODO: part of Exercise 1.2-> 2.)
        Collider HouseCollider = GameObject.Find("House").GetComponent<Collider>();
        if (co.bounds.Intersects(HouseCollider.bounds)) return true;
        return false;
    }
}

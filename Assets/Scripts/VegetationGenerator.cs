using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VegetationGenerator : MonoBehaviour
{
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
    }

    void ClearVegetation()
    {
        // TODO: part of Exercise 1.2 -> 3.)  
    }

    void GenerateVegetation()
    {
        // TODO: Exercise 1.2 -> 1.)
        // Instantiate & transform random "vegetationPrefab"

        // your code here
        for (int i = 0; i < numObjects; i++)
        {
            int someIndex = Random.Range(0, vegetationPrefabs.Count);
            GameObject somePlant = vegetationPrefabs[someIndex];
            Vector3 plantVector = new Vector3(
                Random.Range(vegetationBoundsMin.x, vegetationBoundsMax.x),
                0,
                Random.Range(vegetationBoundsMin.z, vegetationBoundsMax.z)
            );
            Instantiate(somePlant, this.gameObject.transform);

            somePlant.transform.position = plantVector;
            somePlant.transform.rotation = new Quaternion(0, Random.Range(0, 360), 0, 0);
            instances.Add(somePlant);
            //to continue (nel)
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

        // resolve again (delayed), after new random transform applied to colliding instances
        if (resolveAgain)
            StartCoroutine(ResolveCollisions());
    }

    bool IsInRestrictedBounds(Collider co)
    {
        // TODO: part of Exercise 1.2-> 2.)
        return true;
    }
}
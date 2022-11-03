using UnityEngine;

namespace Vrsys
{
    public class VerticalTorsoEstimation : MonoBehaviour
    {
        [Tooltip("The head transform which is used to align the body to. If nothing is specified, transform of parent GameObject will be used.")]
        public Transform headTransform;

        private void Awake()
        {
            if (headTransform == null)
            {
                headTransform = transform.parent;
            }
        }

        void Update()
        {
            // TODO Exercise 1.6
            transform.rotation = Quaternion.LookRotation(headTransform.parent.forward,Vector3.up);
            this.transform.localPosition = new Vector3(0,0,-0.2f);
        }
    }
}

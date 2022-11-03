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

            float Rotation;
            if (headTransform.eulerAngles.x <= 180f)
            {
                Rotation = headTransform.eulerAngles.x;
            }
            else
            {
                Rotation = headTransform.eulerAngles.x - 360f;
            }

            this.transform.localRotation = Quaternion.LookRotation(headTransform.forward, Vector3.up);
            
            this.transform.eulerAngles = new Vector3(0, headTransform.eulerAngles.y, headTransform.eulerAngles.z);

            if (Rotation > 0.0f)
            {
                float dislocation = Rotation / 170.0f;
                this.transform.localPosition =
                    new Vector3(
                        0,
                        -Mathf.Abs(dislocation * headTransform.rotation.z),
                        -Mathf.Abs(dislocation * headTransform.rotation.y)
                        
                        
                        
                    );
            } else {
                transform.localPosition = Vector3.zero;
            }
            
        }
    }
}

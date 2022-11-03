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
            //this.transform.localPosition = new Vector3(0, 0, 0);
            //print(headTransform.eulerAngles.x);
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
            this.transform.localPosition = new Vector3(0, 0, 0);
            this.transform.eulerAngles = new Vector3(0, headTransform.eulerAngles.y, headTransform.eulerAngles.z);


            if ( Rotation > 20f ) {
                print(Rotation);
                //this.transform.localRotation = Quaternion.LookRotation(headTransform.forward, Vector3.up);                
                this.transform.localPosition = new Vector3(0, 0, Rotation*-0.005f);
                //this.transform.eulerAngles = new Vector3(0, headTransform.eulerAngles.y, headTransform.eulerAngles.z);

            }  
            /*else
            {
                this.transform.localRotation = Quaternion.LookRotation(headTransform.forward, Vector3.up);
                this.transform.localPosition = new Vector3(0, 0, 0);
                this.transform.eulerAngles = new Vector3(0, headTransform.eulerAngles.y, headTransform.eulerAngles.z);
            }*/


            //this.transform.localRotation = Quaternion.LookRotation(headTransform.forward,Vector3.up);
            //this.transform.localPosition = new Vector3(0,0,0);
            //this.transform.Translate(new Vector3(0, 0, 0), headTransform);
            //this.transform.eulerAngles = new Vector3(0, headTransform.eulerAngles.y, headTransform.eulerAngles.z);
        }
    }
}

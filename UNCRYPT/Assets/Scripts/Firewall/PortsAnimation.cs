using UnityEngine;
using UnityEngine.Serialization;


public class PortsAnimation : MonoBehaviour
{
    [SerializeField] private GameObject innerCircle;
    [SerializeField] private GameObject outerCircle;
    [SerializeField] private float innerCircleSpeed, outerCircleSpeed;

    private void Update()
    {
        innerCircle.transform.Rotate(Vector3.forward * (innerCircleSpeed * Time.deltaTime));
        outerCircle.transform.Rotate(Vector3.forward * (outerCircleSpeed * Time.deltaTime));
    }
    
}

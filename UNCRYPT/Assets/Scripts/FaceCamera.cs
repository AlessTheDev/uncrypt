using UnityEngine;
using UnityEngine.Serialization;

public class FaceCamera : MonoBehaviour
{
    private Transform _cameraTransform;
    public bool flip;
    
    [SerializeField] private float rotationThreshold = 45f;
    
    private float _lastRotationY;

    private void Start()
    {
        _cameraTransform = Camera.main.transform;
        _lastRotationY = transform.rotation.eulerAngles.y;
    }

    void Update()
    {
        Vector3 directionToCamera = _cameraTransform.position - transform.position;

        directionToCamera.y = 0;

        Quaternion rotation = Quaternion.LookRotation(directionToCamera);

        float currentRotationY = 0;
        if (Mathf.Abs(Mathf.DeltaAngle(_lastRotationY, rotation.y)) * Mathf.Rad2Deg >= rotationThreshold)
        {
            currentRotationY += rotation.eulerAngles.y;
            _lastRotationY = currentRotationY;
        }
        
        currentRotationY += flip ? 0 : 180; 

        transform.rotation = Quaternion.Euler(0, currentRotationY, 0);
    }
}
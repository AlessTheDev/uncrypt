using UnityEngine;
using UnityEngine.Serialization;

namespace FileSystemLevel
{
    public class FloatingObject : MonoBehaviour
    {
        private static readonly int WaveSpeedProperty = Shader.PropertyToID("_WaveSpeed");
        private static readonly int AmplitudeProperty = Shader.PropertyToID("_WaveAmplitude");
        private static readonly int HeightFrequencyProperty = Shader.PropertyToID("_HeightFrequency");

        [SerializeField] private Material waterMaterial;
        [SerializeField] private float lateralMovement;
        [SerializeField] private float minMovingFactor;
        [SerializeField] private float maxMovingFactor;
        [SerializeField] private float minRotationSpeed;
        [SerializeField] private float maxRotationSpeed;

        private Vector3 _startPos;
        private float _xMovingFactor;
        private float _zMovingFactor;

        private float _waveSpeed;
        private float _amplitude;
        private float _heightFrequency;

        private float _rotationSpeed;

        private void Start()
        {
            _waveSpeed = waterMaterial.GetFloat(WaveSpeedProperty);
            _amplitude = waterMaterial.GetFloat(AmplitudeProperty);
            _heightFrequency = waterMaterial.GetFloat(HeightFrequencyProperty);
            
            _startPos = transform.localPosition;
            _xMovingFactor = Random.Range(minMovingFactor, maxMovingFactor);
            _zMovingFactor = Random.Range(minMovingFactor, maxMovingFactor);
            
            _rotationSpeed = Random.Range(minRotationSpeed, maxRotationSpeed);
        }

        public void UpdateObject()
        {
            float time = Time.timeSinceLevelLoad;
            float waveOffset = _waveSpeed * time + transform.localPosition.x * _heightFrequency;

            transform.localPosition = _startPos + new Vector3(
                Mathf.Cos(time * _xMovingFactor) * lateralMovement,
                -Mathf.Sin(waveOffset) * _amplitude,
                Mathf.Sin(time * _zMovingFactor) * lateralMovement
            );
            
            transform.Rotate(Vector3.up * (_rotationSpeed * Time.deltaTime));
        }
    }
}

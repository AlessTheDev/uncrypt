using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PlayerLoading : MonoBehaviour
    {
        [SerializeField] private Image playerImage;
        [SerializeField] private TextMeshProUGUI loadingText;
        
        [SerializeField] private float updateSpeed;

        private float _percentage;
        
        private float _lerpedTextPercentage;
        private float _lastLerpedPercentage;

        private void Start()
        {
            _lastLerpedPercentage = Mathf.NegativeInfinity;
            _percentage = 0;
            playerImage.fillAmount = 0;
        }

        private void Update()
        {
            if (Mathf.Approximately(_lastLerpedPercentage, _percentage)) // Avoid unnecessary calculations 
            {
                return;
            }
                
            _lerpedTextPercentage = Mathf.Lerp(_lerpedTextPercentage, _percentage, updateSpeed * Time.deltaTime);
            
            float normalizedPercentage = _lerpedTextPercentage / 100f;
            
            playerImage.fillAmount = normalizedPercentage;
            loadingText.text = Bar(normalizedPercentage) + _lerpedTextPercentage.ToString("0.00") + "%";
            
            _lastLerpedPercentage = _lerpedTextPercentage;
        }

        public void UpdatePercentage(int percentage)
        {
            _percentage = percentage;            
        }

        private static string Bar(float percentage)
        {
            // [##############--------] 
            const int length = 22;
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("[");
            int fullCharCount = Mathf.RoundToInt(22f * percentage);
            for (int i = 0; i < fullCharCount; i++)
            {
                stringBuilder.Append("#");
            }
            for (int i = 0; i < length - fullCharCount; i++)
            {
                stringBuilder.Append("-");
            }
            stringBuilder.Append("]");
            
            return stringBuilder.ToString();
        }
    }
}

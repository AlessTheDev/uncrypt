using System;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.UI;
using Utils;

namespace UI
{
    public class PlayerUIManager : SceneSingleton<PlayerUIManager>
    {
        [SerializeField] private TextMeshProUGUI hpText;
        [SerializeField] private Canvas canvas;

        [Header("Slider Settings")] [SerializeField]
        private Slider mainHpSlider;

        [SerializeField] private Image mainHpSliderImage;
        [SerializeField] private float mainSliderSpeed;

        [SerializeField] private Slider backgroundHpSlider;
        [SerializeField] private Image backgroundHpSliderImage;
        [SerializeField] private float backgroundSliderSpeed;


        [Header("Color Settings")] [SerializeField]
        private Color lowHpColor;

        [SerializeField] private Color highHpColor;
        [SerializeField] private Color secondaryColorHigh;

        private PlayerController _playerController;
        private int _maxHp;
        private Color _colorDiff;

        private void Start()
        {
            _maxHp = PlayerStats.BaseHp;
            mainHpSlider.maxValue = _maxHp;
            backgroundHpSlider.maxValue = _maxHp;

            _colorDiff = highHpColor - secondaryColorHigh;
        }

        private void FixedUpdate()
        {
            if (!_playerController)
            {
                if (GameManager.Instance != null)
                {
                    _playerController = GameManager.Instance.Player;
                    canvas.enabled = true;
                }
                else
                {
                    canvas.enabled = false;
                }
            }
        }

        private void Update()
        {
            if (!_playerController) return;

            mainHpSlider.value = Utilities.ExpLerp(mainHpSlider.value, _playerController.Hp,
                mainSliderSpeed * Time.unscaledDeltaTime);
            backgroundHpSlider.value = Utilities.ExpLerp(backgroundHpSlider.value, _playerController.Hp,
                backgroundSliderSpeed * Time.unscaledDeltaTime);

            Color color = Color.Lerp(lowHpColor, highHpColor, mainHpSlider.value / _maxHp);

            mainHpSliderImage.color = color;
            color -= _colorDiff;
            backgroundHpSliderImage.color = color;

            hpText.text = "HP: " + (int)_playerController.Hp;
        }
    }
}
using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class PauseMenu : PersistentSingleton<PauseMenu>
    {
        [SerializeField] private GameObject container;
        [SerializeField] private RectTransform rect;
        [SerializeField] private float animationDuration = 1f;
        [SerializeField] private Ease animationEase = Ease.InOutQuad;
        [SerializeField] private Slider masterSlider;
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider sfxSlider;
        [SerializeField] private GameObject confirmExit;
        [SerializeField] private AudioMixer audioMixer;

        [SerializeField] private Button[] lockedSettings;
        [SerializeField] private GameObject lockedSettingsNotice;

        private Tween _activeTween;
        private float _initialScaleX;

        private bool _active;
        private bool _hasInitializedUI;

        private void Start()
        {
            container.SetActive(false);
            confirmExit.SetActive(false);

            _initialScaleX = rect.localScale.x;

            InitializeUI();

            SceneManager.activeSceneChanged += (_, _) => { InitializeUI(); };

            _active = false;

            masterSlider.onValueChanged.AddListener((value) =>
            {
                SetVolume("Master", value);
                masterSlider.value = value;
            });

            musicSlider.onValueChanged.AddListener((value) =>
            {
                SetVolume("Music", value);
                musicSlider.value = value;
            });

            sfxSlider.onValueChanged.AddListener((value) =>
            {
                SetVolume("Sfx", value);
                sfxSlider.value = value;
            });

            InputManager.Instance.InputActions.Player.Pause.performed += ctx =>
            {
                if (!GameManager.Instance || _active) return;

                Show();
            };

            if (SaveSystem.GetData() == null)
            {
                LockOptions(); // Some Options need a save file to be effective
            }
        }

        /// <summary>
        /// Makes sure to initialize data as soon as a save file is available
        /// </summary>
        private void InitializeUI()
        {
            if (_hasInitializedUI) return;

            SaveSystem.GameData data = SaveSystem.GetData();

            if (data == null) return;
            
            UnlockOptions();

            musicSlider.value = data.MusicVolume;
            sfxSlider.value = data.SfxVolume;
            masterSlider.value = data.MasterVolume;

            SetVolume("Master", data.MasterVolume);
            SetVolume("Music", data.MusicVolume);
            SetVolume("Sfx", data.SfxVolume);

            _hasInitializedUI = true;
        }

        private void LockOptions()
        {
            lockedSettingsNotice.SetActive(true);
            foreach (Button setting in lockedSettings)
            {
                setting.interactable = false;
            }
        }
        
        private void UnlockOptions()
        {
            lockedSettingsNotice.SetActive(false);
            foreach (Button setting in lockedSettings)
            {
                setting.interactable = true;
            }
        }

        private void SetVolume(string property, float value)
        {
            audioMixer.SetFloat(property, Mathf.Log10(Mathf.Max(0.001f, value)) * 20);
        }

        public void Show()
        {
            container.SetActive(true);
            _activeTween?.Kill();

            GameManager.Instance?.OnPlayerEntersSafeZone?.Invoke();

            rect.localScale = new Vector2(.1f, _initialScaleX);
            _activeTween = rect.DOScaleX(_initialScaleX, animationDuration).SetEase(animationEase)
                .OnComplete(() => _active = true);
        }

        private void Update()
        {
            if (_active && Input.GetKeyDown(KeyCode.Escape))
            {
                Close();
            }
        }

        public void Close()
        {
            _activeTween?.Kill();
            _activeTween = rect.DOScaleX(0, animationDuration).SetEase(animationEase)
                .OnComplete(() => container.SetActive(false));

            SaveSystem.GameData data = SaveSystem.GetData();

            if (data != null)
            {
                data.MasterVolume = masterSlider.value;
                data.MusicVolume = musicSlider.value;
                data.SfxVolume = sfxSlider.value;
                SaveSystem.Save(data);
            }

            GameManager.Instance?.OnPlayerExitsSafeZone?.Invoke();

            _active = false;
        }

        public void ExitButton()
        {
            confirmExit.gameObject.SetActive(true);
        }

        public void ConfirmExit()
        {
            Application.Quit();
        }

        public void UndoExit()
        {
            confirmExit.gameObject.SetActive(false);
        }
    }
}
using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class AimSettingsButton : MonoBehaviour
    {
        private static readonly int Enabled = Animator.StringToHash("Enabled");
        private const SaveSystem.AimType DefaultAim = SaveSystem.AimType.Keyboard;

        [SerializeField] private SaveSystem.AimType aimType;
        [SerializeField] private AimSettingsButton connectedButton; // NOTE: Create a different system if Radio buttons add up
        [SerializeField] private Animator animator;

        private void Start()
        {
            if (SaveSystem.GetData() == null)
            {
                animator.SetBool(Enabled, aimType == DefaultAim);
            }
            else
            {
                UpdateUI();
            }
            
            GetComponent<Button>().onClick.AddListener(() =>
            {
                SaveSystem.GameData data = SaveSystem.GetData();
                data.AimType = aimType;
                SaveSystem.Save(data);
                
                connectedButton.UpdateUI();
                UpdateUI();
                GameManager gm = GameManager.Instance;
                if (gm != null)
                {
                    gm.Player?.UpdateAimType();
                }
            });
        }

        public void UpdateUI()
        {
            SaveSystem.GameData data = SaveSystem.GetData();
            animator.SetBool(Enabled, aimType == data.AimType);
        }
    }
}
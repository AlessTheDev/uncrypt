using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FileSystemLevel
{
    public class AdministratorPermissionsUI : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private RectTransform container;
        [SerializeField] private RectTransform containerOutPos;
        [SerializeField] private TextMeshProUGUI playerName;
        
        [Header("Cursor")]
        [SerializeField] private Image cursor;
        [SerializeField] private Sprite normalCursor;
        [SerializeField] private Sprite clickCursor;
        [SerializeField] private RectTransform cursorStartPos;
        [SerializeField] private RectTransform cursorOutPos;
        
        [Header("Buttons")]
        [SerializeField] private Image confirmButton;
        [SerializeField] private Image denyButton;

        [Header("Animation Settings")]
        [SerializeField] private Ease entranceEase = Ease.OutBack;
        [SerializeField] private Ease exitEase = Ease.InBack;
        [SerializeField] private Ease cursorToButtonEase = Ease.Linear;
        
        [SerializeField] private float containerEntranceDuration = 0.5f;
        [SerializeField] private float cursorEntranceDuration = 0.7f;
        [SerializeField] private float cursorWaitTime = 1.0f;
        [SerializeField] private float cursorMoveToButtonTime = 1.5f;
        [SerializeField] private float cursorMoveDuration = 0.5f;
        [SerializeField] private float cursorClickDuration = 0.3f;

        private Vector3 _containerStartPos;

        private void Awake()
        {
            _containerStartPos = container.position;
        }

        private void Start()
        {
            container.position = containerOutPos.position;
            cursor.transform.position = cursorOutPos.position;

            playerName.text = SaveSystem.GetData().PlayerName;
        }

        public IEnumerator ShowCoroutine(bool clickYes = false)
        {
            container.position = containerOutPos.position;
            cursor.transform.position = cursorOutPos.position;
            
            // Animate container and cursor to starting positions
            container.DOMove(_containerStartPos, containerEntranceDuration).SetEase(entranceEase);
            cursor.transform.DOMove(cursorStartPos.position, cursorEntranceDuration).SetEase(entranceEase);
            
            // Wait for entrance animation plus wait time
            yield return new WaitForSeconds(cursorEntranceDuration + cursorWaitTime);
            
            Image btn = clickYes ? confirmButton : denyButton;
            cursor.transform.DOMove(btn.transform.position, cursorMoveToButtonTime).SetEase(entranceEase);
            
            yield return new WaitForSeconds(cursorMoveToButtonTime);

            // Animate cursor click
            cursor.sprite = clickCursor;
            yield return new WaitForSeconds(cursorClickDuration);
            cursor.sprite = normalCursor;
            
            // Animate exit
            cursor.transform.DOMove(cursorOutPos.position, cursorMoveDuration).SetEase(exitEase);
            container.DOMove(containerOutPos.position, containerEntranceDuration).SetEase(exitEase);
        }
    }
}
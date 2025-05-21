using System;
using TMPro;
using UnityEngine;

namespace UI.FileSystemPuzzle
{
    public class PuzzleTextBox : MonoBehaviour
    {
        public Vector2 targetPos;
        public float speed;
        public TextMeshProUGUI text;
        
        private RectTransform _rectTransform;

        private void Start()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        private Vector2 _vel;
        private void Update()
        {
            _rectTransform.position = Vector2.SmoothDamp(_rectTransform.position, targetPos, ref _vel, 1 / speed);
        }
    }
}
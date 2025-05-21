using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;

public class Indicator : MonoBehaviour
{
    [SerializeField] private Transform indicatorTransform;
    [SerializeField] private SpriteRenderer indicatorRenderer;
    
    [SerializeField] private float duration;
    [SerializeField] private Vector3 highlightPosition;
    [SerializeField] private Vector3 restPosition;

    private bool _isVisible;
    private Vector3 _initialScale;

    private void Awake()
    {
        indicatorTransform.localPosition = restPosition;
        gameObject.SetActive(false);
        _isVisible = false;
        _initialScale = indicatorTransform.localScale;
        indicatorTransform.localScale = Vector3.zero;
    }

    public void ShowForSeconds(float seconds)
    {
        gameObject.SetActive(true);

        StartCoroutine(ShowForSecondsCoroutine(seconds));
    }
    
    private IEnumerator ShowForSecondsCoroutine(float seconds)
    {
        if(_isVisible) yield break;

        Show();
        yield return new WaitForSeconds(seconds);
        Hide();
    }

    public void Show()
    {
        gameObject.SetActive(true);
        _isVisible = true;
        indicatorTransform.DOLocalMove(highlightPosition, duration).SetEase(Ease.OutQuad);
        indicatorTransform.DOScale(_initialScale, duration);
        indicatorRenderer.DOFade(1, duration / 2).SetEase(Ease.InQuad);
    }

    public void Hide()
    {
        indicatorTransform.DOLocalMove(restPosition, duration).SetEase(Ease.OutQuad);
        indicatorTransform.DOScale(Vector3.zero, duration);
        indicatorRenderer.DOFade(0, duration / 2f);
        _isVisible = false;
    }

    private void OnDestroy()
    {
        indicatorTransform.DOKill();
        indicatorRenderer.DOKill();
    }
}
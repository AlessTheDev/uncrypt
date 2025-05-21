using System;
using DG.Tweening;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitializeGame : MonoBehaviour
{
    [SerializeField] private RectTransform creditsPanel;
    [SerializeField] private float creditsPanelSpeed = .5f;
    [SerializeField] private Ease creditsPanelEase = Ease.InOutCubic;

    private float _initialPanelScaleX;

    private void Start()
    {
        _initialPanelScaleX = creditsPanel.localScale.x;   
        creditsPanel.localScale = new Vector3(0, creditsPanel.localScale.y);
    }

    public void Play()
    {
        if (SaveSystem.GetData() == null)
        {
            SceneManager.LoadScene(Scenes.InitialCutscene);
        }
        else
        {
            SceneManager.LoadScene(Scenes.Hub);
        }
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void OpenCreditsPanel()
    {
        creditsPanel.gameObject.SetActive(true);
        creditsPanel.DOScaleX(_initialPanelScaleX, creditsPanelSpeed).SetEase(creditsPanelEase);
    }

    public void CloseCreditsPanel()
    {
        creditsPanel.DOScaleX(0, creditsPanelSpeed).SetEase(creditsPanelEase).OnComplete(() => creditsPanel.gameObject.SetActive(false));
    }

    public void ShowOptions()
    {
        PauseMenu.Instance.Show();
    }

    public void OpenURL(string url)
    {
        Application.OpenURL(url);
    }
}
using System;
using DefaultNamespace;
using DG.Tweening;
using UnityEngine;

public class ThanksForPlayingMenu : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;

    private void Start()
    {   
        canvasGroup.alpha = 0;
        canvasGroup.DOFade(1, 5);
    }

    public void GoBackToMainMenu()
    {
        SaveSystem.DeleteData();
        _ = SceneFader.Instance.SwitchScene(Scenes.InitializeGame);
    }
}

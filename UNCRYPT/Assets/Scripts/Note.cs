using System;
using Terminal;
using UnityEngine;
using DG.Tweening;
using Player.StateMachine.States;
using TMPro;

public class Note : Collectable
{
    [Header("Note Info")] 
    [SerializeField] private string fileName;
    [SerializeField] private string content;
    [SerializeField] private string dir = "notes";

    private void Awake()
    {
        fileNameVisual.text = fileName;
    }

    public void Show()
    {
        ShowPanel(dir, fileName, content,
            "You’ll be able to read the notes again next time you access the terminal in the 'notes' folder.");
    }
}
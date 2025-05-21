using System;
using UnityEngine;

public class EnableForCheckpoint : MonoBehaviour
{
    [SerializeField] private SaveSystem.StoryCheckpoint checkpoint;
    private void Awake()
    {
        gameObject.SetActive(SaveSystem.GetData().StoryCheckpoint == checkpoint);
    }
}
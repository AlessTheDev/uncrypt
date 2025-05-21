using System;
using UnityEngine;

namespace Utils
{
    public class CheckpointModifier : MonoBehaviour
    {
        [SerializeField] private SaveSystem.StoryCheckpoint checkpoint;
        
        [ContextMenu("Set Checkpoint")]
        private void EditCheckpoint()
        {
            var data = SaveSystem.GetData();
            data.StoryCheckpoint = checkpoint;
            SaveSystem.Save(data);
        }
    }
}
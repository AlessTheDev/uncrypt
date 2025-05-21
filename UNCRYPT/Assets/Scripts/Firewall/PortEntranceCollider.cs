using System;
using DefaultNamespace;
using Player;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Firewall
{
    public class PortEntranceCollider : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == PlayerController.Layer)
            {
                if (transform.GetComponentInParent<Port>().IsOpen)
                {
                    _ = SceneFader.Instance.SwitchScene(Scenes.FileSystem);
                }
                else
                {
                    TopUIDialogue.Instance.ShowDialogue(new []
                    {
                        new TopUIDialogue.DialogueSentence("It seems like this port is closed."),
                        new TopUIDialogue.DialogueSentence("Let's go look elsewhere."),
                        new TopUIDialogue.DialogueSentence("I recommend using the nmap command from the terminal to quickly find the open port!"),
                    }, new WaitForSeconds(1));
                }
            }
        }
    }
}
using UnityEngine;

namespace Terminal
{
    public class TerminalAccess : MonoBehaviour
    {
        [SerializeField] private string terminalName;

        public void OpenTerminal()
        {
            TerminalManager.Instance.Open($"echo 'Remote Access Terminal [{terminalName}] <color=#f5426c>Most used commands: nmap or help</color>'");
        }
    }
}

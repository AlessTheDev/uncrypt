using System.Collections;
using UnityEngine;

namespace Terminal
{
    public abstract class TerminalCommand
    {
        public abstract string GetName();

        protected TerminalCommand()
        {
            TerminalManager.Instance.RegisterCommand(this);
        }

        public abstract IEnumerator Execute(string[] args);

        protected void Print(string text, string color = null)
        {
            TerminalManager.Instance.Writer.Print(
                color != null ? $"<color={color}>{text}</color>" : text);
        }

        protected void Clear()
        {
            TerminalManager.Instance.Writer.Clear();
        }

        protected void Clear(int nLines)
        {
            TerminalManager.Instance.Writer.Clear(nLines);
        }

        protected void UpdateLastLine(string line)
        {
            Clear(1);
            Print(line);
        }
        
        protected FileSystem FS => TerminalManager.Instance.FileSystem;
    }
}
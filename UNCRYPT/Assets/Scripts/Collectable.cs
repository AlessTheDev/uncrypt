using Terminal;
using TMPro;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    [Header("Settings")] 
    [SerializeField] protected Transform visual;
    [SerializeField] protected TextMeshProUGUI fileNameVisual;
    [SerializeField] protected float rotationSpeed = 100;
        
    [Header("Up And Down Animation")] 
    [SerializeField] protected float animationSpeed = 1.5f;
    [SerializeField] protected float animationRange = 0.4f;
    
    private float _t;

    public void ShowPanel(string dir, string fileName, string content, string message)
    {
        FileSystem fs = TerminalManager.Instance.FileSystem;
        if (fs.Root.GetDirWithName(dir) == null)
        {
            fs.Root.AddDirectory(new FileSystem.Directory(dir));
        }

        FileSystem.Directory keysDir = fs.Root.GetDirWithName(dir);
        if (keysDir.GetFileWithName(fileName) == null)
        {
            keysDir.AddFile(new FileSystem.File(fileName, content));
        }
        
        TerminalManager.Instance.Writer.Clear();
        string command = $"cat /{dir}/{fileName}";
        TerminalManager.Instance.Writer.Print(command);
        TerminalManager.Instance.Open(command, true);
        
        TerminalManager.Instance.Writer.Print("<color=grey>---");
        TerminalManager.Instance.Writer.Print(message);
        TerminalManager.Instance.Writer.Print("Press any button to close the terminal.</color>");
        
        fs.Save();
    }
    
    private void Update()
    {
        _t += Time.deltaTime * animationSpeed;
        visual.localPosition = new Vector3(0, Mathf.Sin(_t) * animationRange, 0);
        visual.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }
}
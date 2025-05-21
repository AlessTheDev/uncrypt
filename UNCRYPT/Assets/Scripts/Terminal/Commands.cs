using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DialogueSystem;
using Firewall;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Terminal
{
    public class HelpCommand : TerminalCommand
    {
        public override string GetName() => "help";

        public override IEnumerator Execute(string[] args)
        {
            Print("AVAILABLE COMMANDS");
            Print("------------------");
            Print("cd <folder> \t Change directory");
            Print("exit \t Closes the terminal");
            Print("cat <file-name> \t Displays the contents of a file");
            Print("clear \t Clears the terminal screen");
            Print("curl \t Transfers data to or from a server");
            Print("hack <address> \t Does nothing, but makes you look like a hacker");
            Print("ls \t Lists files and folders");
            Print("nmap \t Scans the network for open ports");
            Print("scp \t Copies files between computers");
            Print("tree \t Displays folders in a structured tree view");
            Print("\n**HOW TO USE THE TERMINAL**");
            Print("The terminal is a text-based tool that allows you to interact with the system.");
            Print("You can type the commands listed above and press ENTER to execute them.");
            Print("Examples:");
            Print("- Type 'ls' to see the files in the current folder.");
            Print("- Use 'cd <folder>' to move into another folder.");
            Print("- Type 'cat <file-name>' to read the contents of a file.");
            Print("If you need help with a command, type its name followed by '--help'.");

            yield return null;
        }
    }

    // This is a test command
    public class HackCommand : TerminalCommand
    {
        public override string GetName() => "hack";

        public override IEnumerator Execute(string[] args)
        {
            if (args.Length != 2)
            {
                Print("Usage: hack <target>");
                yield break;
            }

            string target = args[1];

            Print($"[INFO] Initiating attack on {target}...");
            yield return new WaitForSeconds(1);

            Print("[INFO] Bypassing main firewall...");
            yield return new WaitForSeconds(2);

            Print("[INFO] Injecting payload...");
            yield return new WaitForSeconds(2);
            Print("[WARNING] Countermeasures detected!", "#CE2E2E");
            string evading = "Evading";
            Print(evading);
            for (int i = 0; i < 3; i++)
            {
                yield return new WaitForSeconds(1);
                evading += ".";
                UpdateLastLine(evading);
            }

            Print("[INFO] Creating secure backdoor...");
            yield return new WaitForSeconds(3);

            Print("[INFO] Uploading trojans...");
            yield return new WaitForSeconds(2);

            Print("[SUCCESS] Attack completed!", "#5bd489");
            yield return null;
        }
    }

    // Clears the terminal screen
    public class ClearCommand : TerminalCommand
    {
        public override string GetName() => "clear";

        public override IEnumerator Execute(string[] args)
        {
            if (args.Length == 2 && args[1] == "--help")
            {
                Print("Usage: clear");
                Print("Description: Clears the terminal's screen.");
                yield break;
            }

            Clear();
            yield return null;
        }
    }

    public class SudoRmCommand : TerminalCommand
    {
        public override string GetName() => "sudo";

        public override IEnumerator Execute(string[] args)
        {
            if (string.Concat(args).StartsWith("sudo rm -rf /".Replace(" ", "")))
            {
                Print(
                    "\u2588 \u2591 \u2592 \u2593 \u25a0 \u25a1 \u25b2 \u25bc \u2620 \u26a0 \u2716 \u2605 \u2606 \u2751 \u2752 \u2302 \u237e \u2364 ᚠ ᛒ Ӝ Ж Ѭ Ψ Ω ᓚ ᖉ ᔑ \u20a7 \u20a9 \u26a1 \u2622 \u2623 \u231b \u23f3 \u2302 \u2694 \u2692\nẅ ẃ ẁ ỻ ỽ ὅ ὕ ὗ ἷ ἥ ἐ ἔ ḁ ḅ ḉ ṡ ṭ ṕ ẗ Ɀ ᴥ ᴓ \u221e \u2261 \u2260 \u2297 \u2295 \u2299 \u225b \u238b \u21e7 \u21e9 \u2630 \u29c8 \u2af7 \u2af8\n｢ ｣ \ud835\udd04 \ud835\udd0d ℑ ℵ \u2207 \u2202 \u2211 \u220f \u221e \u21ad \u21cc \u27bf \u2399 \u239b \u239c \u239d \u24b6 \u24b7 \u24cd\n");
                Print("Can't find root directory");
                yield return new WaitForSeconds(3);
                Application.Quit();
            }
            else if (args.Length > 1)
            {
                Print(args[1] == "rm" ? "Error. You're not supposed to delete anything..." : "Unnecessary sudo");
            }
            else
            {
                Print("Incorrect syntax");
            }
        }
    }

    public class RmCommand : TerminalCommand
    {
        public override string GetName() => "rm";

        public override IEnumerator Execute(string[] args)
        {
            Print("Permission denied. Use sudo.");
            yield return null;
        }
    }

    public class ExitCommand : TerminalCommand
    {
        public override string GetName() => "exit";

        public override IEnumerator Execute(string[] args)
        {
            if (args.Length == 2 && args[1] == "--help")
            {
                Print("Usage: exit");
                Print("Description: Closes the terminal.");
                yield break;
            }

            TerminalManager.Instance.Close();
            yield return null;
        }
    }

    public class NMapCommand : TerminalCommand
    {
        public override string GetName() => "nmap";

        public override IEnumerator Execute(string[] args)
        {
            if (PortsManager.Instance == null || TerminalManager.Instance.IsInTutorialMode)
            {
                Print("This command can't be executed right now.");
                yield break;
            }

            if (args.Length == 2 && args[1] == "--help")
            {
                Print("Usage: nmap -p 20-30 192.168.1.100");
                Print("Description: Scans for open ports in the specified range at the given IP address.");
                yield break;
            }

            if (args.Length != 3 || (args.Length == 3 && (args[1] != "-p") || args[2] != "20-30" ||
                                     args[2] != "192.168.1.100"))
            {
                Print("[ASSISTANT] Auto-correcting command", "#FFFFFF");
            }

            float startingTime = Time.time;
            Print($"Starting Nmap 7.93 ( https://nmap.org ) at {DateTime.Now}");
            Print("Nmap scan report for 192.168.1.100");
            Print("Host is up (0.0020s latency).");

            Print("PORT\tSTATE\tSERVICE");
            Port[] ports = PortsManager.Instance.Ports;

            foreach (var port in ports)
            {
                string line = "";
                line += $"{port.PortNumber}/tcp" + "\t";
                line += (port.IsOpen ? "open" : "closed") + "\t";
                line += port.PortService;
                Print(line);
                yield return new WaitForSeconds(1.5f);
            }

            Print($"Nmap done: 1 IP address (1 host up) scanned in {Time.time - startingTime} seconds");

            Print("Press any key to continue...");
            yield return new WaitUntil(() => Input.anyKeyDown);
            TerminalManager.Instance.Close(false);
            InputManager.Instance.InputActions.Player.Disable();
            yield return new WaitForSeconds(2);
            yield return PortsManager.Instance.OpenPort.Highlight(8);
            yield return new WaitForSeconds(4);
            yield return new WaitWhile(() => TerminalManager.Instance.Writer.isActiveAndEnabled); // Security check
            InputManager.Instance.InputActions.Player.Enable();
            GameManager.Instance.Player.EnableMovement();
            DialogueManager.Instance.StartCoroutine(DialogueManager.Instance.ParseDialogue("port_found_1"));
        }
    }

    public class SecretCommand : TerminalCommand
    {
        public override string GetName() => "secret";

        public override IEnumerator Execute(string[] args)
        {
            Print(":3 WOO SECRET");
            yield return null;
        }
    }

    public class TreeCommand : TerminalCommand
    {
        public override string GetName() => "tree";

        public override IEnumerator Execute(string[] args)
        {
            ListChildDirs(FS.GetDirectory("/"));
            yield return new WaitForSeconds(0.1f);
        }

        private void
            ListChildDirs(FileSystem.Directory dir, int lvl = 0) // lvl tells us how deep we are into the directory tree
        {
            if (dir.Children.Length == 0)
            {
                return;
            }

            string folderLine = "";

            for (int i = 0; i < lvl; i++)
            {
                folderLine += "  ";
            }

            folderLine += dir.Name;

            Print(folderLine);

            // List Dirs
            foreach (var d in dir.Children)
            {
                string subFoldersLine = "";
                for (int i = 0; i < lvl + 1; i++)
                {
                    subFoldersLine += "  ";
                }

                subFoldersLine += "|--";

                subFoldersLine += d.Name;

                Print(subFoldersLine);
                ListChildDirs(d, lvl + 1);
            }
        }
    }

    public class CurlCommand : TerminalCommand
    {
        public override string GetName() => "curl";

        public override IEnumerator Execute(string[] args)
        {
            if (args.Length != 2)
            {
                Print("Usage: curl <url>");
                Print("Description: Downloads content from the specified URL.");
                yield break;
            }

            Print("[INFO] Executing curl command");
            yield return new WaitForSeconds(2);
            Print($"Connecting to {args[1]}");
            Print("Status Code 200\nNo further information\n[INFO] Closing connection");
            yield return null;
        }
    }

    public class SCPCommand : TerminalCommand
    {
        public override string GetName() => "scp";

        public override IEnumerator Execute(string[] args)
        {
            if (args.Length != 3)
            {
                Print("Usage: scp <destination-file> <destination-host>");
                Print(
                    "[INFO] <destination-host> specifies where to copy the selected file.\nUse 'homehost' to send it to your own PC.",
                    "#26dcc1");
                yield break;
            }

            Print("[INFO] Starting scp command");
            Print($"[INFO] Copying file to {args[2]}");
            yield return new WaitForSeconds(2);
            if (args[2] == "homehost")
            {
                // TODO: Add the file to the file system
                Print($"[SUCCESS] File {args[1]} successfully copied to {args[2]}", "#5bd489");
            }
            else
            {
                Print("[WARNING] Unrecognized host, try using a valid name", "#CE2E2E");
            }

            yield return null;
        }
    }


    public class ChangeDirectoryCommand : TerminalCommand
    {
        public override string GetName() => "cd";

        public override IEnumerator Execute(string[] args)
        {
            if (args.Length == 2 && args[1] == "--help")
            {
                Print("Usage: cd <folder>");
                Print("Description: Changes the current directory to the specified folder.");
                yield break;
            }

            if (args.Length != 2)
            {
                Print("Usage: cd <folder>");
                yield break;
            }

            try
            {
                TerminalManager.Instance.FileSystem.ChangeDirectory(args[1]);
            }
            catch (Exception e)
            {
                Print(e.Message);
            }

            yield return null;
        }
    }

    public class CatCommand : TerminalCommand
    {
        public override string GetName() => "cat";

        public override IEnumerator Execute(string[] args)
        {
            if (args.Length != 2 || args.Length == 2 && args[1] == "--help")
            {
                Print("Usage: cat <file-name>");
                Print("Description: Shows the content of a specified file.");
                yield break;
            }

            string filePath = args[1].Replace("\\", "/");
            int lastSlashIndex = filePath.LastIndexOf("/", StringComparison.Ordinal);
            bool isSubDir = lastSlashIndex != -1;

            FileSystem.Directory dir = isSubDir ? FS.GetDirectory(filePath[..(lastSlashIndex + 1)]) : FS.CurrentDir;
            FileSystem.File f = dir.GetFileWithName(isSubDir ? filePath[(lastSlashIndex + 1)..] : filePath);

            if (f == null)
            {
                Print("File non found.");
                yield break;
            }


            Print(f.Content);
        }
    }


    public class EchoCommand : TerminalCommand
    {
        public override string GetName() => "echo";

        public override IEnumerator Execute(string[] args)
        {
            StringBuilder content = new StringBuilder();
            for (int i = 1; i < args.Length; i++)
            {
                content.Append(args[i]);
                content.Append(" ");
            }
            Print(content.ToString().Replace("\"", ""));
            yield return null;
        }
    }

    public class ListCommand : TerminalCommand
    {
        public override string GetName() => "ls";

        public override IEnumerator Execute(string[] args)
        {
            if (args.Length == 2 && args[1] == "--help")
            {
                Print("Usage: ls");
                Print("Description: Lists files and folders in the current directory.\n\n");
                yield break;
            }

            FileSystem.Directory dir = TerminalManager.Instance.FileSystem.CurrentDir;
            string output = "";
            foreach (var d in dir.Children)
            {
                output += $"<color=#1daecf>{d.Name}</color>\t";
            }

            foreach (var f in dir.Files)
            {
                output += $"{f.Name} ";
            }

            Print(output);
            yield return null;
        }
    }
}
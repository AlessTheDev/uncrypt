using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Terminal;
using UnityEngine;
using UnityEngine.Serialization;

public static class SaveSystem
{
    public enum StoryCheckpoint
    {
        None,
        TutorialCompleted,
        FileTaken,
        FirstKeyTaken,
        SecondKeyTaken,
    }

    public enum AimType
    {
        Keyboard,
        Mouse
    }
    
    [System.Serializable]
    public class GameData
    {
        public FileSystem.Directory FileSystemRoot;
        public StoryCheckpoint StoryCheckpoint;
        public AimType AimType;
        public bool HasOpenedTerminalBefore;
        public bool NeedsNMapTutorial;
        public string PlayerName;
        public float MasterVolume;
        public float MusicVolume;
        public float SfxVolume;

        public GameData(FileSystem.Directory fileSystemRoot)
        {
            FileSystemRoot = fileSystemRoot;
            PlayerName = "Player";
            StoryCheckpoint = StoryCheckpoint.None;
            HasOpenedTerminalBefore = false;
            NeedsNMapTutorial = true;
            AimType = AimType.Keyboard;

            MasterVolume = .7f;
            MusicVolume = .5f;
            SfxVolume = .8f;
        }
    }
    public static readonly string FILE_PATH = Application.persistentDataPath + "/data.cupflow";

    // Cached Player Name
    public static string PlayerName;

    public static void Save(GameData data)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        FileStream stream = new FileStream(FILE_PATH, FileMode.Create);

        formatter.Serialize(stream, data);

        stream.Close();
    }
    
    // NOTE: It would be better to cache game data instead of reading from files everytime 
    public static GameData GetData()
    {
        if (!File.Exists(FILE_PATH)) return null;
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(FILE_PATH, FileMode.Open);

        GameData data = formatter.Deserialize(stream) as GameData;

        stream.Close();
        
        PlayerName = data?.PlayerName;

        return data;
    }

    public static void DeleteData()
    {
        if (File.Exists(FILE_PATH)) File.Delete(FILE_PATH);
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public class GameSaveManager : SingletonDontDestroy<GameSaveManager>
{
    [Tooltip("Always insert a '/' at the beginning of this string")]
    public string allSaveDataPath;
    [Tooltip("Always insert a '.' at the beginning of this string")]
    public string saveFilesExtension;

    public bool useEncryption = false;
    private readonly string encryptionCodeWord = "hippopotomonstrosesquilipedian";
    public List<SaveData> saveData = new List<SaveData>();
    public List<SaveData> defaultData = new List<SaveData>();


    public static event Action OnResetGame;
    public override void Awake()
    {
        base.Awake();

        SaveDefaultData();
        LoadAll();

    }
    private void Start()
    {
        GameManager.OnGameEnd += SaveAll;
        //// Initialize the debugging buttons
        //saveButton.onClick.AddListener(() => SaveAll());
        //loadButton.onClick.AddListener(() => LoadAll());
        //resetButton.onClick.AddListener(() => ResetGame());
        // Save the default data before overwriting with the saved data
       
        // Overwrite the default data with the saved data
        
    }
    private string EncryptDecrypt(string data)
    {
        string modifiedData = "";

        for (int i = 0; i < data.Length; i++)
        {
            modifiedData += (char)(data[i] ^ encryptionCodeWord[i % encryptionCodeWord.Length]);
        }
        return modifiedData;
    }

    public bool IsSaveFile()
    {
        // does the save directory exist?
        return Directory.Exists(Path.Combine(Application.persistentDataPath, allSaveDataPath));
    }

    public void SaveGame(SaveData saveData)
    {
        // If the save directory doesn't exist, then create it
        if (!IsSaveFile())
        {
            Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, allSaveDataPath));
        }

        // If the specific save directory doesn't exist, then create it
        if (!Directory.Exists(Path.Combine(Application.persistentDataPath, allSaveDataPath, saveData.savePath)))
        {
            Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, allSaveDataPath, saveData.savePath));
        }

        /*----------------SERIALIZATION----------------*/

        // Initialize the binary formatter object
        BinaryFormatter bf = new BinaryFormatter();
        // Create the file
        string fullPath = Path.Combine(Application.persistentDataPath, allSaveDataPath, saveData.savePath, saveData.fileName + saveFilesExtension);

        // initialize the serializer
        var json = JsonUtility.ToJson(saveData.scriptableData);

        if (useEncryption)
        {
            json = EncryptDecrypt(json);
        }
        
        using (FileStream stream = new FileStream(fullPath, FileMode.Create))
        {
            using(StreamWriter writer = new StreamWriter(stream))
            {
                bf.Serialize(stream, json);
                writer.Write(json);
            }
        }
    }
    public void LoadGame(SaveData saveData)
    {
        // if the save directory doesn't exist, return out of the function... it means that the file has never been saved so no need for loading
        if (!Directory.Exists(Path.Combine(Application.persistentDataPath, allSaveDataPath, saveData.savePath)))
        {
            return;//TODO: Make it return instead of create a new directory
        }

        /*--------------DESERIALIZATION----------------*/

        // Initialize the binary formatter object
        BinaryFormatter bf = new BinaryFormatter();
        // if the file exists, then we deserialize and overwrite the data on the scriptable object
        if (File.Exists(Path.Combine(Application.persistentDataPath, allSaveDataPath, saveData.savePath, saveData.fileName + saveFilesExtension)))
        {
            // open the file 

            string fullPath = Path.Combine(Application.persistentDataPath, allSaveDataPath, saveData.savePath, saveData.fileName + saveFilesExtension);
            string jsonToLoad = "";

        //    FileStream file = File.Open(Path.Combine(Application.persistentDataPath, allSaveDataPath, saveData.savePath, saveData.fileName + saveFilesExtension), FileMode.Open);
            // derialize the data with the serializer
            
            using(FileStream stream = new FileStream(fullPath, FileMode.Open))
            {
                using(StreamReader reader = new StreamReader(stream))
                {
                    bf.Deserialize(stream);
                    jsonToLoad = reader.ReadToEnd();
                }
            }
            if (useEncryption)
            {
                jsonToLoad = EncryptDecrypt(jsonToLoad);
            }
         //   saveData.scriptableData = JsonUtility.FromJson<ScriptableObject>(jsonToLoad);
            JsonUtility.FromJsonOverwrite(jsonToLoad, saveData.scriptableData);
            // close the file 
        }
    }
    // FUNCTION: Used to delete an existing save data
    public void DeleteSaveGame(SaveData saveData)
    {
        // if the directory doesn't exist, return out of the function... it means that the file has never been saved so no need for loading
        if (!Directory.Exists(Path.Combine(Application.persistentDataPath, allSaveDataPath, saveData.savePath)))
        {
            return;//TODO: Make it return instead of create a new directory
        }
        // if the file exists, then delete it
        if (File.Exists(Path.Combine(Application.persistentDataPath, allSaveDataPath, saveData.savePath, saveData.fileName + saveFilesExtension)))
        {
            File.Delete(Path.Combine(Application.persistentDataPath, allSaveDataPath, saveData.savePath, saveData.fileName + saveFilesExtension));
        }
    }
    // FUNCTION: runs the "SaveGame" function on all save data in the "saveData" list
    public void SaveAll()
    {
        foreach (var saveDatum in saveData)
        {
            SaveGame(saveDatum);
        }
    }
    // FUNCTION: runs the "LoadGame" function on all save data in the "saveData" list
    public void LoadAll()
    {
        foreach (var saveDatum in saveData)
        {
            LoadGame(saveDatum);
        }
    }
    // FUNCTION: runs the "DeleteSaveGame" function on all save data in the "saveData" list
    public void DeleteAllSaves()
    {
        foreach (var saveDatum in saveData)
        {
            DeleteSaveGame(saveDatum);
        }
    }
    // FUNCTION: runs the "SaveGame" function on all save data in the "defaultData" list
    public void SaveDefaultData()
    {
        //    if(PlayerPrefs.GetInt(PlayerPrefKeys.firstTimeSave) == 1) { return; }
        //    if (!PlayerPrefs.HasKey(PlayerPrefKeys.firstTimeSave))
        //  {
        foreach (var saveDatum in defaultData)
        {
            SaveGame(saveDatum);
        }
        //   }
        //   PlayerPrefs.SetInt(PlayerPrefKeys.firstTimeSave, 1);
    }
    // FUNCTION: runs the "LoadGame" function on all save data in the "defaultData" list
    public void LoadDefaultData()
    {
        foreach (var saveDatum in defaultData)
        {
            LoadGame(saveDatum);
        }
    }
    // FUNCTION: makes game reseting possible 
    public void ResetGame()
    {
        // first of all, delete all existing game save files, so as to not conflict with the new save data to come
        DeleteAllSaves();
        // then overwrite the scriptable objects with the data residing in the default saves... this is so the game data in the scriptable objects get affected by the reset
        LoadDefaultData();
        // then with the new data loaded, save it the directories
        SaveAll();
        // lastly, tell all listeners subscribed to the "OnResetGame" event that the game has been reset
        OnResetGame?.Invoke();
    }
    //UNITY_CALLBACK_FUNCTION, FUNCTION: saves the game data before the app closes
    private void OnApplicationQuit()
    {
        // Save the game before the game closes
        SaveAll();
    }
    
}

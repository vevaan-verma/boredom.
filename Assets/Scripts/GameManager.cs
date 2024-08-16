using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameManager : MonoBehaviour {

    [Header("Levels")]
    [SerializeField] private Level[] levels;
    [SerializeField] private string levelStatusesFilePath;
    public LevelStatusRoot levelStatusData;

    private void Awake() {

        DontDestroyOnLoad(gameObject);

        string fullPath = GetFullPath(levelStatusesFilePath);
        print(fullPath);

        if (!File.Exists(fullPath))
            File.Create(fullPath).Close();

        levelStatusData = new LevelStatusRoot();

        using (StreamReader sr = new StreamReader(fullPath)) {

            if (Application.isEditor)
                Debug.Log("File successfully opened at " + fullPath);

            if (sr.EndOfStream) {

                if (Application.isEditor)
                    Debug.Log("There is no level status data! Autofilling list.");

                foreach (Level level in levels)
                    levelStatusData.levels.Add(new LevelStatus(level.levelNum, false));

            } else {

                levelStatusData = JsonConvert.DeserializeObject<LevelStatusRoot>(sr.ReadToEnd());

            }
        }
    }

    public bool IsLevelCompleted(Level level) {

        foreach (LevelStatus status in levelStatusData.levels)
            if (status.levelNum == level.levelNum)
                return status.completed;

        return false;

    }

    public void SetLevelCompleted(Level level) {

        foreach (LevelStatus status in levelStatusData.levels) {

            if (status.levelNum == level.levelNum) {

                status.completed = true;
                return;

            }
        }
    }

    public void SaveLevelData() {

        string fullPath = GetFullPath(levelStatusesFilePath);

        using (StreamWriter sw = new StreamWriter(fullPath)) {

            if (Application.isEditor)
                Debug.Log("Serializing level data to file!");

            sw.Write(JsonConvert.SerializeObject(levelStatusData, Formatting.Indented, new JsonSerializerSettings {

                ReferenceLoopHandling = ReferenceLoopHandling.Ignore

            }));
        }
    }

    public string GetFullPath(string path) => Application.persistentDataPath + Path.DirectorySeparatorChar + path;

}

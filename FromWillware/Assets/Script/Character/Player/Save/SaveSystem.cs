using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Collections;

public class SaveSystem : MonoBehaviour
{
    public string fileName = "save.json";

    private bool canSave = false;
    
    // 把路径获取改为公开的静态方法，方便主菜单检查存档是否存在
    public static string GetSavePath(string fileName) 
    {
        return System.IO.Path.Combine(Application.persistentDataPath, fileName);
    }

    // 跨场景信号：决定场景加载后是否要读取本地存档
    public static bool shouldLoadSaveGame = false;

    string Path => GetSavePath(fileName);

    IEnumerator Start()
    {
        if (shouldLoadSaveGame)
        {
            yield return null; // 等一帧

            Load();

            shouldLoadSaveGame = false;
        }
    }

    void Update()
    {
        if (canSave && Input.GetKeyDown(KeyCode.E)) Save();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SavePoint"))
        {
            canSave = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("SavePoint"))
        {
            canSave = false;
        }
    }

    public void Save()
    {
        SaveFile file = new SaveFile();

        var saveables = FindObjectsOfType<MonoBehaviour>(true);

        foreach (var s in saveables)
        {
            if (s is ISaveable saveable)
            {
                SaveEntry entry = new SaveEntry
                {
                    id = saveable.GetUniqueID(),
                    json = saveable.CaptureState()
                };

                file.entries.Add(entry);
            }
        }

        string json = JsonUtility.ToJson(file, true);
        File.WriteAllText(Path, json);

        // 如果你希望存档时也保存当前场景索引，可以加上这句
        PlayerPrefs.SetInt("SavedSceneIndex", UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        PlayerPrefs.Save();

        Debug.Log("Saved");
    }

    public void Load()
    {
        if (!File.Exists(Path)) 
        {
            Debug.LogWarning("未找到存档文件！");
            return;
        }

        string json = File.ReadAllText(Path);
        SaveFile file = JsonUtility.FromJson<SaveFile>(json);

        var saveables = FindObjectsOfType<MonoBehaviour>(true);

        foreach (var s in saveables)
        {
            if (s is ISaveable saveable)
            {
                foreach (var entry in file.entries)
                {
                    if (entry.id == saveable.GetUniqueID())
                    {
                        saveable.RestoreState(entry.json);
                        break;
                    }
                }
            }
        }

        Debug.Log("Loaded");
    }
}
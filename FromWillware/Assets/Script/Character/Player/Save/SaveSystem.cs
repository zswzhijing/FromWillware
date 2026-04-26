using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    public string fileName = "save.json";

    private bool canSave = false;
    string Path => System.IO.Path.Combine(Application.persistentDataPath, fileName);

    void Update()
    {
        if (canSave&&Input.GetKeyDown(KeyCode.E)) Save();
        if (Input.GetKeyDown(KeyCode.I)) Load();
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

        Debug.Log("Saved");
    }

    public void Load()
    {
        if (!File.Exists(Path)) return;

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
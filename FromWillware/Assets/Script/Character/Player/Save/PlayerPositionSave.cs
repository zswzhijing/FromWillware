using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPositionSave : MonoBehaviour,ISaveable
{
    public string UniqueID = "PlayerPosition";

    public string CaptureState()
    {
        TransformData data = new TransformData(transform.position, transform.rotation);
        return JsonUtility.ToJson(data);
    }

    public void RestoreState(string json)
    {
        TransformData data = JsonUtility.FromJson<TransformData>(json);
        StartCoroutine(ApplyPositionNextFrame(data));
    }

    IEnumerator ApplyPositionNextFrame(TransformData data)
    {
        yield return null; // ⭐ 等一帧

        transform.position = data.GetPosition();
        transform.rotation = data.GetRotation();
    }

    public string GetUniqueID()
    {
        return UniqueID;
    }
}

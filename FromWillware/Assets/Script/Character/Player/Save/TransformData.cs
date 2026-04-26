using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TransformData
{
    public float x, y, z;
    public float rotY; // 只存水平朝向就够了（第三人称常用）

    public TransformData(Vector3 pos, Quaternion rot)
    {
        x = pos.x;
        y = pos.y;
        z = pos.z;

        rotY = rot.eulerAngles.y;
    }

    public Vector3 GetPosition()
    {
        return new Vector3(x, y, z);
    }

    public Quaternion GetRotation()
    {
        return Quaternion.Euler(0, rotY, 0);
    }
}
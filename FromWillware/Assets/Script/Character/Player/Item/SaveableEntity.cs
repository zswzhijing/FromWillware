using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SaveableEntity : MonoBehaviour
{
    [SerializeField]
    private string uniqueID;

    public string UniqueID => uniqueID;

#if UNITY_EDITOR
    private void OnValidate()
    {
        // 自动生成唯一ID
        if (string.IsNullOrEmpty(uniqueID))
        {
            uniqueID = System.Guid.NewGuid().ToString();

            EditorUtility.SetDirty(this);
        }
    }
#endif
}
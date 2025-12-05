using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AssetSO))]
public class AssetSOButton : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        AssetSO assetSO = (AssetSO)target;

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Validate All GUID",GUILayout.Width(200),GUILayout.Height(30)))
        {
            assetSO.Check();
        }
        
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }
}

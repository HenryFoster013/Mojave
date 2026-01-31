using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerFactionSO))]
public class PlayerFactionEditor : Editor{
    public override void OnInspectorGUI(){

        DrawDefaultInspector();
        PlayerFactionSO faction = (PlayerFactionSO)target;

        if (faction.Flag != null){
            GUILayout.Space(10);
            Rect rect = GUILayoutUtility.GetRect(267, 150, GUILayout.ExpandWidth(false));
            EditorGUI.DrawPreviewTexture(rect, faction.Flag);
        }
    }
}
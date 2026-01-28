using UnityEngine;
using UnityEditor;
using System.IO;

public class SoundEffectGenerator{
    
    [MenuItem("Assets/Create/Custom/Sound/Generate SoundEffectSO from Selection", false, 1)]
    public static void GenerateSO(){

        Object[] selectedObjects = Selection.GetFiltered(typeof(AudioClip), SelectionMode.Assets);

        if (selectedObjects.Length == 0){
            Debug.LogWarning("No AudioClips selected! Please select the .ogg files you want to convert.");
            return;
        }

        int count = 0;

        foreach (Object obj in selectedObjects){
            AudioClip clip = (AudioClip)obj;
            SoundEffectSO newSO = ScriptableObject.CreateInstance<SoundEffectSO>();
            SerializedObject serializedSO = new SerializedObject(newSO);
            serializedSO.FindProperty("BaseClip").objectReferenceValue = clip;
            serializedSO.FindProperty("Name").stringValue = clip.name;
            serializedSO.ApplyModifiedProperties();

            string clipPath = AssetDatabase.GetAssetPath(clip);
            string folderPath = Path.GetDirectoryName(clipPath);
            string newAssetPath = Path.Combine(folderPath, clip.name + ".asset");
            newAssetPath = AssetDatabase.GenerateUniqueAssetPath(newAssetPath);

            AssetDatabase.CreateAsset(newSO, newAssetPath);
            count++;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"Successfully created {count} SoundEffectSO assets!");
    }

    [MenuItem("Assets/Create/Custom/Sound/Generate SoundEffectSO from Selection", true)]
    public static bool ValidateGenerateSO(){
        return Selection.GetFiltered(typeof(AudioClip), SelectionMode.Assets).Length > 0;
    }
}
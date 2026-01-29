using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "New Territory", menuName = "World Map/Territory")]
public class TerritorySO : ScriptableObject
{
    [Header("Main")]
    public string Name;
    public Texture2D Source;
    public RegionSO Region;

    [Header("Generated")] // Information taken from the coloured territory sheet, do not edit!!
    public Color32 Colour;
    public float MedianX;
    public float MedianY;

    public void GenerateValues(){
        SetColour();
        SetMedian();
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }

    void SetColour(){
        Color32[] pixels = Source.GetPixels32();
        bool found = false;
        for (int i = 0; i < pixels.Length && !found; i++){
            if(pixels[i].a != 0){
                Colour = pixels[i];
                found = true;
            }
        }
    }

    void SetMedian(){

        int matches = 0;
        int sum_x = 0;
        int sum_y = 0;
        Color32[] pixels = Source.GetPixels32();
        
        for(int i = 0; i < pixels.Length; i++){
            if(pixels[i].a != 0){
                matches++;
                int y = (i / Source.width);
                int x = i - (y * Source.width);
                sum_x += x;
                sum_y += y;
            }
        }

        MedianX = ((float)sum_x / (float)matches) / Source.width;
        MedianY = ((float)sum_y / (float)matches) / Source.height;
    }

}

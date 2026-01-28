using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Sound Lookup", menuName = "Custom/Sound/Lookup")]
public class SoundEffectLookupSO : ScriptableObject
{
    public SoundEffectSO[] Sounds;

    public SoundEffectSO GetSFX(string name){
        SoundEffectSO result = null;
        for(int i = 0; i < Sounds.Length && result == null; i++){
            if(Sounds[i].GetName().ToUpper() == name.ToUpper())
                result = Sounds[i];
        }
        return result;
    }

    public SoundEffectSO GetSFX(int id){
        return Sounds[id];
    }
}

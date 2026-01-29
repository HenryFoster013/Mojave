using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RISK_Utils
{
    public class TerritoryInstance{

        public TerritorySO definition {private set; get;}
        public PlayerFactionSO owner;

        public bool dirty {private set; get;}
        public int shader_mask_index {private set; get;} = -1;

        public TerritoryInstance(TerritorySO passed_definition){
            definition = passed_definition;
            Dirty();
        }

        public void SetOwner(PlayerFactionSO new_faction){
            if(owner == new_faction)
                return;
            owner = new_faction;
            Dirty();
        }

        public Color GetRegionColour(){
            return definition.Region.Colour;
        }

        public Color GetFactionColour(){
            if(owner == null)
                return Color.white;
            return owner.Colour;
        }

        public void Dirty(){dirty = true;}
        public void Clean(){dirty = false;}
    }
}
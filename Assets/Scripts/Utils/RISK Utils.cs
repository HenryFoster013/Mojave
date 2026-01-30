using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RISK_Utils
{
    public class TerritoryInstance{

        public TerritorySO definition {private set; get;}
        public PlayerFactionSO owner;

        public bool selected {private set; get;}
        public bool dirty {private set; get;}
        public int shader_mask_index {private set; get;} = -1;

        public TerritoryInstance(TerritorySO passed_definition){
            definition = passed_definition;
            Dirty();
        }

        // Setters //

        public void SetOwner(PlayerFactionSO new_faction){
            if(owner == new_faction)
                return;
            owner = new_faction;
            Dirty();
        }

        public void Select(){
            selected = true;
            Dirty();
        }

        public void Deselect(){
            selected = false;
            Dirty();
        }

        public void Dirty(){dirty = true;}
        public void Clean(){dirty = false;}

        // Getters //

        public Color RegionColour(){return definition.Region.Colour;}
        public Color FactionColour(){
            if(owner == null)
                return Color.white;
            return owner.Colour;
        }
        public string Name(){return definition.name;}
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RISK_Utils
{
    public class RegionInstance{
        
        public RegionSO definition {private set; get;}
        public List<TerritoryInstance> territories {private set; get;}
        public bool complete; 

        public RegionInstance(RegionSO data){
            definition = data;
            territories = new List<TerritoryInstance>();
            complete = false;
        }

        public void AddTerritory(TerritoryInstance territory){
            territories.Add(territory);
            territory.region = this;
        }

        public List<TerritoryInstance> GetList(){
            return territories;
        }

        public void UpdateCompletion(){

            bool complete_buffer = complete;
            complete = true;
            PlayerFactionSO faction = territories[0].owner;

            for(int i = 1; i < territories.Count && complete; i++){
                if(territories[i].owner != faction){
                    complete = false;
                }
            }

            if(complete != complete_buffer){
                foreach(TerritoryInstance territory in territories)
                    territory.Dirty();
            }
        }
    }

    public class TerritoryInstance{

        public TerritorySO definition {private set; get;}
        public PlayerFactionSO owner;
        public RegionInstance region;

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
            region.UpdateCompletion();
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
                return new Color(0.5f, 0.5f, 0.5f);
            return owner.Colour;
        }
        public string Name(){return definition.name;}
    }
}
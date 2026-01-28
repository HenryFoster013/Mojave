using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RISK_Utils
{
    public class TerritoryInstance{

        public TerritorySO definition {private set; get;}

        public bool dirty {private set; get;}
        public int shader_mask_index {private set; get;} = -1;

        public TerritoryInstance(TerritorySO passed_definition){
            definition = passed_definition;
            dirty = true;
        }

        public void Clean(){
            dirty = false;
        }
    }
}
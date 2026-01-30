using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RISK_Utils;
using UnityEngine.EventSystems;

public class ClickManager : MonoBehaviour{
    
    [Header("Primary References")]
    [SerializeField] PlayerController _PlayerController;
    [SerializeField] MapManager _MapManager;
    [SerializeField] Camera _Camera;

    [Header("Misc")]
    [SerializeField] LayerMask ClickableLayers;

    const int UILayer = 5;

    public bool IsPointerOverUIElementBuffer {private set; get;}
    
    float mouse_down_time = 0;
    Vector3 mouse_pos, mouse_start_pos;
    bool block_worldy_clicks_buffer, click_buffer;
    
    void Start(){
        
    }

    void Update(){
        ClickingLogic();
        SelectionLogic();
    }

    void ClickingLogic(){
        if(Input.GetMouseButtonDown(0))
            mouse_start_pos = Input.mousePosition;
        mouse_pos = Input.mousePosition;

        if(Input.GetMouseButton(0))
            mouse_down_time += Time.deltaTime;
        else{
            if(!click_buffer)
                click_buffer = true;
            else{
                mouse_down_time = 0;
                click_buffer = false;
            }
        }
    }

    void SelectionLogic(){

        if(block_worldy_clicks_buffer){
            block_worldy_clicks_buffer = false;
            return;
        }

        if (ShortClick()){ 
            RaycastHit hit;
            Ray ray = _Camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100f, ClickableLayers)){
                if(hit.transform.tag == "Board"){
                   
                    _PlayerController.Select(_MapManager.GetTerritoryAtPoint(hit.point));

                }
            }
            else{
                _PlayerController.Deselect();
            }
        }

        
        if(LongClick()){
            
            // Feed information to the camera controller to manage dragging the screen around

        }

        IsPointerOverUIElementBuffer = IsPointerOverUIElement();
    }

    // CLICK TYPES //

    bool ShortClick(){
        return !IsPointerOverUIElementBuffer && !IsPointerOverUIElement() && (Input.GetMouseButtonUp(0) && mouse_down_time < 0.2f && ((mouse_pos - mouse_start_pos).sqrMagnitude < 10f));
    }

    bool LongClick(){
        return !IsPointerOverUIElement() && (Input.GetMouseButton(0) && (mouse_down_time > 0.2f || ((mouse_pos - mouse_start_pos).sqrMagnitude > 10f)));
    }

    public bool IsPointerOverUIElement(){
        return IsPointerOverUIElement(GetEventSystemRaycastResults());
    }

    private bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults){
        for (int index = 0; index < eventSystemRaysastResults.Count; index++){
            RaycastResult curRaysastResult = eventSystemRaysastResults[index];
            if (curRaysastResult.gameObject.layer == UILayer){

                // Here we can add a hoverable display for more information

                return true;
            }
        }
        return false;
    }
    
    static List<RaycastResult> GetEventSystemRaycastResults(){
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);
        return raycastResults;
    }
}

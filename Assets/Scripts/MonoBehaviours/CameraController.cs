using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SoundUtils;

public class CameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] MapManager _MapManager;
    [SerializeField] SoundEffectLookupSO SFX_Lookup;
    
    [Header("Main")]
    [SerializeField] Transform MovementPivot;
    [SerializeField] Transform ZoomPivot;
    [SerializeField] Transform RotationPivot;

    // CONSTANTS //
    
    // Movement Modifiers
    const float base_speed = 15f;
    const float sprint_speed = 25f;
    const float zoom_speed = 15f;
    const float scroll_zoom_speed = 10f;

    // Height Bounds
    const float minimum_height = 1f;
    const float median_bound = 12f;
    const float maximal_height = 30f;

    // Camera Rotations
    const float bottom_rotation = 20f;
    const float standard_rotation = 70f;
    const float flattened_rotation = 90f;
    const float rotation_speed = 70f;

    // VARIABLES //

    // Fixed Values
    float normalised_median, normalised_maximum, median_maximum;

    // Targets
    Vector3 target_position;
    float target_zoom;
    float target_rotation;
    float normalised_zoom;
    float target_pivot;
    int rotation_snap = 0;

    void Start(){
        SetDefaults();
    }

    void SetDefaults(){
        target_zoom = ZoomPivot.localPosition.y;
        normalised_median = median_bound - minimum_height;
        normalised_maximum = maximal_height - minimum_height;
        median_maximum = maximal_height - median_bound;
        target_pivot = 0f;
    }

    public void RunLogic(bool typing){
        if(!typing){
            SetTargetPositions();
            SetTargetRotations();
        }
        LerpTowards();
    }

    void SetTargetRotations(){

        normalised_zoom = target_zoom - minimum_height;
        target_rotation = standard_rotation;

        if(Input.GetKey(KeyCode.LeftShift))
            target_pivot += Input.GetAxisRaw("Rotation Axis") * rotation_speed * Time.deltaTime;
        else{
            if(Input.GetKeyDown(KeyCode.DownArrow)){
                target_pivot = 0f;
                rotation_snap = 0;
                PlaySFX("pipboy_tab_2", SFX_Lookup);
            }
            if(Input.GetKeyDown(KeyCode.LeftArrow)){
                rotation_snap--;
                target_pivot = 90f * rotation_snap;
                PlaySFX("pipboy_tab_2", SFX_Lookup);
            }
            if(Input.GetKeyDown(KeyCode.RightArrow)){
                rotation_snap++;
                target_pivot = 90f * rotation_snap;
                PlaySFX("pipboy_tab_2", SFX_Lookup);
            }
        }
        
        if(target_zoom < median_bound)
            target_rotation = Mathf.Lerp(bottom_rotation, standard_rotation, normalised_zoom / normalised_median);
        else
            target_rotation = Mathf.Lerp(standard_rotation, flattened_rotation, (target_zoom - median_bound) / median_maximum);
    }

    void SetTargetPositions(){

        float speed_mod = base_speed;
        if(Input.GetKey(KeyCode.LeftShift)){
            speed_mod = sprint_speed;
        }
        
        speed_mod = speed_mod * ((normalised_zoom / normalised_maximum) + 0.15f); 
        target_position += Vector3.Normalize((MovementPivot.forward * Input.GetAxisRaw("Vertical")) + (MovementPivot.right * Input.GetAxisRaw("Horizontal"))) * speed_mod * Time.deltaTime;
        
        target_zoom += (Input.GetAxisRaw("Zoom Axis") + Input.GetAxisRaw("Mouse ScrollWheel") * 275f) * zoom_speed * Time.deltaTime;
        target_zoom = Mathf.Clamp(target_zoom, minimum_height, maximal_height);
    }

    void LerpTowards(){
        MovementPivot.localPosition = Vector3.Lerp(MovementPivot.localPosition, target_position, Time.deltaTime * 15f);
        ZoomPivot.localPosition = Vector3.Lerp(ZoomPivot.localPosition, Vector3.up * target_zoom, Time.deltaTime * 10f);
        RotationPivot.localEulerAngles = new Vector3(Mathf.Lerp(RotationPivot.eulerAngles.x, target_rotation, Time.deltaTime * 3f), 0f, 0f);
        MovementPivot.localEulerAngles = new Vector3(0f, Mathf.LerpAngle(MovementPivot.eulerAngles.y, target_pivot, Time.deltaTime * 6f), 0f);
        _MapManager.UpdateNametagRotation(MovementPivot.eulerAngles.y);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
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

    // VARIABLES //

    // Fixed Values
    float normalised_median;
    float normalised_maximum;

    // Targets
    Vector3 target_position;
    public float target_zoom;
    float target_rotation;
    float normalised_zoom;

    void Start(){
        SetDefaults();
    }

    void SetDefaults(){
        target_zoom = ZoomPivot.localPosition.y;
        normalised_median = median_bound - minimum_height;
        normalised_maximum = maximal_height - minimum_height;
    }

    void Update(){
        SetTargetPositions();
        SetTargetRotations();
        LerpTowards();
    }

    void SetTargetRotations(){
        normalised_zoom = target_zoom - minimum_height;
        target_rotation = standard_rotation;
        
        if(target_zoom < median_bound)
            target_rotation = Mathf.Lerp(bottom_rotation, standard_rotation, normalised_zoom / normalised_median);
        else
            target_rotation = Mathf.Lerp(standard_rotation, flattened_rotation, (target_zoom - median_bound) / normalised_maximum);
    }

    void SetTargetPositions(){

        float speed_mod = base_speed;
        if(Input.GetKey(KeyCode.LeftShift))
            speed_mod = sprint_speed;
        
        speed_mod = speed_mod * ((normalised_zoom / normalised_maximum) + 0.15f); 
        target_position += (new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"))) * speed_mod * Time.deltaTime;
        
        target_zoom += Input.GetAxis("Zoom Axis") * zoom_speed * Time.deltaTime;
        target_zoom = Mathf.Clamp(target_zoom, minimum_height, maximal_height);
    }

    void LerpTowards(){
        MovementPivot.localPosition = Vector3.Lerp(MovementPivot.localPosition, target_position, Time.deltaTime * 15f);
        ZoomPivot.localPosition = Vector3.Lerp(ZoomPivot.localPosition, Vector3.up * target_zoom, Time.deltaTime * 10f);
        RotationPivot.eulerAngles = new Vector3(Mathf.Lerp(RotationPivot.eulerAngles.x, target_rotation, Time.deltaTime * 3f), 0f, 0f);
    }
}

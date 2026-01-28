using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Main")]
    [SerializeField] Transform MovementPivot;
    [SerializeField] Transform ZoomPivot;
    [SerializeField] Transform RotationPivot;
    
    [Header("Modifiers")]
    [SerializeField] float BaseSpeed = 5f;
    [SerializeField] float SprintSpeed = 5f;
    [SerializeField] float ZoomSpeed = 15f;

    [Header("Bounds")]
    [SerializeField] float MinimumHeight = 1f;
    [SerializeField] float MedianBound = 12f;
    [SerializeField] float MaximalHeight = 30f;

    [Header("Rotations")]
    [SerializeField] float BottomRotation;
    [SerializeField] float StandardRotation;
    [SerializeField] float FlattenedRotation;

    Vector3 target_position;
    public float target_zoom;
    float target_rotation;

    void Start(){
        SetDefaults();
    }

    void SetDefaults(){
        target_zoom = ZoomPivot.localPosition.y;
    }

    void Update(){
        SetTargetPositions();
        SetTargetRotations();
        LerpTowards();
    }

    void SetTargetRotations(){
        target_rotation = StandardRotation;
        float prospect = 0f;

        if(target_zoom < MedianBound){
            prospect = (target_zoom - MinimumHeight) / (MedianBound - MinimumHeight);
            target_rotation = Mathf.Lerp(BottomRotation, StandardRotation, prospect);
        }
        else{
            prospect = (target_zoom - MedianBound) / (MaximalHeight - MedianBound);
            target_rotation = Mathf.Lerp(StandardRotation, FlattenedRotation, prospect);
        }
    }

    void SetTargetPositions(){
        float speed_mod = BaseSpeed;
        if(Input.GetKey(KeyCode.LeftShift))
            speed_mod = SprintSpeed;
        
        float zoom_speed_mod = ((target_zoom - MinimumHeight) / (MaximalHeight - MinimumHeight)) + 0.15f;
        speed_mod = speed_mod * zoom_speed_mod;

        float keyboard_zoom_mod = 0f;
        if(Input.GetKey("e"))
            keyboard_zoom_mod = 1f;
        else if(Input.GetKey("q"))
            keyboard_zoom_mod = -1f;
        keyboard_zoom_mod = keyboard_zoom_mod * ZoomSpeed;
        
        target_zoom += (keyboard_zoom_mod) * Time.deltaTime;
        target_zoom = Mathf.Clamp(target_zoom, MinimumHeight, MaximalHeight);

        target_position += (new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"))) * speed_mod * Time.deltaTime;
    }

    void LerpTowards(){
        MovementPivot.localPosition = Vector3.Lerp(MovementPivot.localPosition, target_position, Time.deltaTime * 15f);
        ZoomPivot.localPosition = Vector3.Lerp(ZoomPivot.localPosition, Vector3.up * target_zoom, Time.deltaTime * 10f);
        RotationPivot.eulerAngles = new Vector3(Mathf.Lerp(RotationPivot.eulerAngles.x, target_rotation, Time.deltaTime * 3f), 0f, 0f);
    }
}

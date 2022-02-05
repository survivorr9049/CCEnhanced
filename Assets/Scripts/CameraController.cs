using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [HideInInspector] public Vector3 rotation;
    [HideInInspector] public Vector3 planarForward{
        get{
            return Vector3.Scale(plane, playerCamera.transform.forward).normalized;
        }
    }
    [HideInInspector] public Vector3 planarRight{
        get{
            return -Vector3.Cross(planarForward, Vector3.up).normalized;
        }
    }
    Vector3 plane = new Vector3(1, 0, 1);
    public Camera playerCamera;
    public float mouseSensitivity;
    ///<summary> maximum and minimum x angle of the camera </summary>
    public float clampAngle;
    [SerializeField] bool lockCursor;
    [SerializeField] float targetFOV;
    public float FOVChangeSmoothness;
    void Start()
    {
        if(lockCursor) Cursor.lockState = CursorLockMode.Locked;
    }
    void Update()
    {
        rotation += new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0);
        rotation.x = Mathf.Clamp(rotation.x, -clampAngle, clampAngle);
        if(targetFOV != playerCamera.fieldOfView){
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFOV, FOVChangeSmoothness * Time.deltaTime);
            playerCamera.fieldOfView = Mathf.Round(playerCamera.fieldOfView*100)/100;
        }
        playerCamera.transform.eulerAngles = rotation;

    }
    public void SetFOV(float newFOV){
        targetFOV = newFOV;
    }
}

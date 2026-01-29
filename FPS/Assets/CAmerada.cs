using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class CAmerada : MonoBehaviour
{
    public Transform Player;
    public float sensitivity = 5f;
    float CameraVerticalRotation = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float inputX = Input.GetAxis("Mouse X") * sensitivity;
        float inputY = Input.GetAxis("Mouse Y") * sensitivity;

        CameraVerticalRotation -= inputY;
        CameraVerticalRotation = Mathf.Clamp(CameraVerticalRotation, -90f, 90f);
        transform.localEulerAngles = Vector3.right * CameraVerticalRotation;


        Player.Rotate(Vector3.up * inputX);

       

    }
}

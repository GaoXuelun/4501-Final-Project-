using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private int mouseMoveEdge = 20;
    [SerializeField] private float moveSpeed = 20;
    [SerializeField] private float zoom = 50;
    [SerializeField] private float zoomMax = 50;
    [SerializeField] private float zoomMin = 10;
    [SerializeField] private float zoomSpeed = 10;
    [SerializeField] private float screenHorEdge = 60;
    [SerializeField] private float screenVerEdge = 75;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CameraMove();
        CameraZoom();
    }

    private void CameraMove()
    {
        Vector3 inputDir = new Vector3(0, 0, 0);
        if (Input.mousePosition.x < mouseMoveEdge)  //move left
            inputDir.x = -1f;
        if (Input.mousePosition.y < mouseMoveEdge)  //move down
            inputDir.z = -1f;
        if (Input.mousePosition.x > Screen.width - mouseMoveEdge)  //move right
            inputDir.x = 1f;
        if (Input.mousePosition.y > Screen.height - mouseMoveEdge)  //move up
            inputDir.z = 1f;
        // calculation    
        Vector3 moveDir = transform.forward * inputDir.z + transform.right * inputDir.x;
        transform.position += moveDir * moveSpeed * Time.deltaTime;
        // camera move (screen) edge
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -screenHorEdge, screenHorEdge), transform.position.y, Mathf.Clamp(transform.position.z, -screenVerEdge, screenVerEdge));
    }
    
    private void CameraZoom()
    {
        // zoom in and out
        if (Input.mouseScrollDelta.y < 0)
            zoom += 5;
        if (Input.mouseScrollDelta.y > 0)
            zoom -= 5;
        zoom = Mathf.Clamp(zoom, zoomMin, zoomMax);
        virtualCamera.m_Lens.FieldOfView = Mathf.Lerp(virtualCamera.m_Lens.FieldOfView, zoom, zoomSpeed * Time.deltaTime);
    }
}

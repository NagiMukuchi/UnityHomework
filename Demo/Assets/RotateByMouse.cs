using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateByMouse : MonoBehaviour
{
    public float anglePerSecond = 150f;
    public Transform playerCamera; // Gắn Camera (child của player) ở đây

    private float pitch = 0f; // góc nhìn dọc

    private void Start()
    {
        // Khóa chuột vào giữa màn hình
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // Xử lý xoay ngang (yaw) -> xoay player
        float yaw = mouseX * anglePerSecond * Time.deltaTime;
        transform.Rotate(0, yaw, 0);

        // Xử lý xoay dọc (pitch) -> xoay camera
        pitch -= mouseY * anglePerSecond * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, -90f, 90f); // Giới hạn góc nhìn lên/xuống

        playerCamera.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }
}

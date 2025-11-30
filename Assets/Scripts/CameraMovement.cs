using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float rotationSpeed = 2f;
    public float fastMultiplier = 2f;

    float yaw = 0f;
    float pitch = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        yaw += Input.GetAxis("Mouse X") * rotationSpeed;
        pitch -= Input.GetAxis("Mouse Y") * rotationSpeed;
        pitch = Mathf.Clamp(pitch, -80f, 80f);

        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);

        float speed = moveSpeed;

        if (Input.GetKey(KeyCode.LeftShift))
            speed *= fastMultiplier;

        Vector3 movement = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) movement += transform.forward;
        if (Input.GetKey(KeyCode.S)) movement -= transform.forward;
        if (Input.GetKey(KeyCode.A)) movement -= transform.right;
        if (Input.GetKey(KeyCode.D)) movement += transform.right;

        transform.position += movement.normalized * speed * Time.deltaTime;
    }
}

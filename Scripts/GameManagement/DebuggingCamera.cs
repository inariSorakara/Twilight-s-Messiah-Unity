using UnityEngine;

public class DebuggingCamera : MonoBehaviour
{
    public float moveSpeed = 10f;      // Speed for WASD movement
    public float rotationSpeed = 20f; // Speed for arrow key rotation

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    void Start()
    {
        // Store the initial position and rotation so we can reset later
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    void Update()
    {
        // 1) Move camera with WASD (in local space)
        float moveX = Input.GetAxis("Horizontal"); // A/D
        float moveZ = Input.GetAxis("Vertical");   // W/S
        Vector3 movement = new Vector3(moveX, 0, moveZ) * (moveSpeed * Time.deltaTime);
        transform.Translate(movement, Space.Self);

        // 2) Rotate camera with arrow keys (in local space)
        float rotateHorizontal = 0f;
        float rotateVertical   = 0f;

        if (Input.GetKey(KeyCode.UpArrow))    rotateVertical = 1f;
        if (Input.GetKey(KeyCode.DownArrow))  rotateVertical = -1f;
        if (Input.GetKey(KeyCode.LeftArrow))  rotateHorizontal = -1f;
        if (Input.GetKey(KeyCode.RightArrow)) rotateHorizontal = 1f;

        // Rotate around the local y-axis for horizontal, and local x-axis for vertical
        transform.Rotate(Vector3.up,   rotateHorizontal * rotationSpeed * Time.deltaTime, Space.Self);
        transform.Rotate(Vector3.right, -rotateVertical * rotationSpeed * Time.deltaTime, Space.Self);

        // 3) Reset camera with the Enter key
        if (Input.GetKeyDown(KeyCode.Return))
        {
            transform.position = initialPosition;
            transform.rotation = initialRotation;
        }
    }
}

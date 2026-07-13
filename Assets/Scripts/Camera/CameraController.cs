using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 10f;

    [Header("Zoom")]
    [SerializeField] private float zoomSpeed = 10f;
    [SerializeField] private float minHeight = 5f;
    [SerializeField] private float maxHeight = 20f;


   
    private void Update()
    {
        MoveCamera();
        ZoomCamera();
    }

    private void MoveCamera()
    {
        Vector2 movement = InputManager.Instance.CameraMovement;

        Vector3 direction = new Vector3(
            movement.x,
            0,
            movement.y
        );

        transform.position += direction * moveSpeed * Time.deltaTime;
    }
private void ZoomCamera()
{
    float scroll = InputManager.Instance.MouseScroll.y;

    Vector3 position = transform.position;

    position.y -= scroll * zoomSpeed * Time.deltaTime;
    position.y = Mathf.Clamp(position.y, minHeight, maxHeight);

    transform.position = position;
}
}
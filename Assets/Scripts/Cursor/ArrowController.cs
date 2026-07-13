using UnityEngine;

public class ArrowController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 20f;

    [Header("Height")]
    [SerializeField] private float defaultHeight = 1.0f;
    [SerializeField] private float unitOffset = 0.25f;

    [Header("Float")]
    [SerializeField] private float floatAmplitude = 0.15f;
    [SerializeField] private float floatSpeed = 3f;

    private Vector3 targetPosition;

    private void Awake()
    {
        targetPosition = transform.position;
    }

    private void Update()
    {
        Move();
    }

    public void SetTarget(GridTile tile)
    {
        targetPosition = tile.WorldPosition;

        float y = defaultHeight;

        if (tile.Occupant != null)
        {
            Renderer renderer = tile.Occupant.GetComponentInChildren<Renderer>();

            if (renderer != null)
            {
                y = renderer.bounds.max.y + unitOffset;
            }
        }

        targetPosition.y = y;
    }

    private void Move()
    {
        Vector3 pos = Vector3.Lerp(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime);

        pos.y += Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;

        transform.position = pos;
    }
}
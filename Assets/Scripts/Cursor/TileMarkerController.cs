using UnityEngine;

public class TileMarkerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 20f;
    [SerializeField] private float heightOffset = 0.02f;

    [Header("Pulse")]
    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private float pulseAmount = 0.02f;

    private Vector3 targetPosition;
    private Vector3 baseScale;

    private void Awake()
    {
        baseScale = transform.localScale;
        targetPosition = transform.position;
    }

    private void Update()
    {
        Move();
        AnimatePulse();
    }

    public void SetTarget(GridTile tile)
    {
        targetPosition = tile.WorldPosition + Vector3.up * heightOffset;
    }

    private void Move()
    {
        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime);
    }

    private void AnimatePulse()
    {
        float scale =
            1f +
            Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;

        transform.localScale = baseScale * scale;
    }
}
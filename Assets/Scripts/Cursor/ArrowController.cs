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

    public static ArrowController Instance { get; private set; }

    private Unit currentTargetUnit;

    private void Awake()
    {
        Instance = this;
        targetPosition = transform.position;
    }

    private void Update()
    {
        // Se não tiver um alvo forçado, tentar seguir a unidade do turno
        Unit unitToFollow = currentTargetUnit;
        if (unitToFollow == null && TurnManager.Instance != null)
        {
            unitToFollow = TurnManager.Instance.CurrentUnit;
        }

        if (unitToFollow != null)
        {
            UpdateTargetPosition(unitToFollow);
        }

        Move();
    }

    public void ForceTarget(Unit unit)
    {
        currentTargetUnit = unit;
    }

    public void ClearForcedTarget()
    {
        currentTargetUnit = null;
    }

    private void UpdateTargetPosition(Unit unit)
    {
        // Se a unidade está fazendo preview de movimento, pegamos a posição de onde ela vai parar
        GridTile tile = unit.EffectiveTile;
        if (tile == null)
            return;

        targetPosition = tile.WorldPosition;

        float y = defaultHeight;

        Renderer renderer = unit.GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            y = renderer.bounds.max.y + unitOffset;
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
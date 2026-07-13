using UnityEngine;

public class UnitVisual : MonoBehaviour
{
    [SerializeField] private Renderer unitRenderer;

    private Color defaultColor;


    private void Awake()
    {
        if (unitRenderer == null)
            unitRenderer = GetComponentInChildren<Renderer>();


        defaultColor = unitRenderer.material.color;
    }


    public void Select()
    {
        unitRenderer.material.color = Color.yellow;
    }


    public void Deselect()
    {
        unitRenderer.material.color = defaultColor;
    }
}
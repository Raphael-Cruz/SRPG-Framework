using UnityEngine;

public class SelectionIndicator : MonoBehaviour
{
    private void Awake()
    {
        gameObject.SetActive(false);
    }


    public void Show(Vector3 position)
    {
        transform.position = position + Vector3.up * 0.1f;

        gameObject.SetActive(true);
    }


    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
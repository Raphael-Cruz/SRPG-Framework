using UnityEngine;

public class PersistentObject : MonoBehaviour
{
    private void Awake()
    {
        // Garante que o objeto não seja destruído ao carregar uma nova cena
        DontDestroyOnLoad(gameObject);
    }
}
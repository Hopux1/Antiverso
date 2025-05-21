using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy")) 
        {
            Destroy(other.gameObject);
            Debug.Log("Enemigo eliminado por caer al vacío");
        }
    }
}

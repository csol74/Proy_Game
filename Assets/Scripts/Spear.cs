using UnityEngine;

public class Spear : MonoBehaviour
{
    // Opcional: un efecto de destrucción o sonido puedes agregar aquí

    private void OnTriggerEnter(Collider other)
    {
        // Aquí detectamos si el objeto que tocó la lanza es el jugador (u otro)
        if (other.CompareTag("Player"))
        {
            // Se "rompe" la lanza destruyéndola
            Destroy(gameObject);
        }
    }
}

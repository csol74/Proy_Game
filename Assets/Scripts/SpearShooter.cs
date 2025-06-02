using UnityEngine;

public class SpearShooter : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Arrastra aquí tu prefab de la lanza (con Rigidbody y Collider)")]
    public GameObject spearPrefab;

    [Tooltip("Transform desde donde sale la lanza; normalmente será este mismo GameObject o un hijo vacío")]
    public Transform spawnPoint;

    [Header("Configuración de disparo")]
    [Tooltip("Segundos entre cada disparo")]
    public float shootInterval = 3f;

    [Tooltip("Velocidad con la que sale la lanza hacia adelante")]
    public float spearSpeed = 10f;

    private void Start()
    {
        if (spearPrefab == null || spawnPoint == null)
        {
            Debug.LogError("SpearShooter: te faltó asignar spearPrefab o spawnPoint en el Inspector.");
            enabled = false;
            return;
        }

        // Lanza lanza repetidamente
        InvokeRepeating(nameof(LaunchSpear), 0f, shootInterval);
    }

    private void LaunchSpear()
    {
        // Instanciar la lanza
        GameObject nuevaSpear = Instantiate(spearPrefab, spawnPoint.position, spawnPoint.rotation);

        // 🔁 Rota la lanza acostada (ajusta según cómo esté tu modelo)
        nuevaSpear.transform.Rotate(90f, 0f, 0f); // O prueba con -90f si apunta mal

        // Empujar con Rigidbody
        Rigidbody rb = nuevaSpear.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = spawnPoint.forward * spearSpeed;
        }

        // Autodestruir después de unos segundos
        Destroy(nuevaSpear, 5f);
    }
}

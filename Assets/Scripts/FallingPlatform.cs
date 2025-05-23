using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    public float delayBeforeFall = 2f;
    public float respawnDelay = 5f;

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Collider platformCollider;
    private Renderer platformRenderer;

    private bool isFalling = false;

    void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;

        platformCollider = GetComponent<Collider>();
        platformRenderer = GetComponent<Renderer>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isFalling && collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(FallAndRespawn());
        }
    }

    private System.Collections.IEnumerator FallAndRespawn()
    {
        isFalling = true;

        yield return new WaitForSeconds(delayBeforeFall);

        // Desactivar plataforma
        platformCollider.enabled = false;
        platformRenderer.enabled = false;

        yield return new WaitForSeconds(respawnDelay);

        // Restaurar plataforma
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        platformCollider.enabled = true;
        platformRenderer.enabled = true;

        isFalling = false;
    }
}


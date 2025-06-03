using UnityEngine;

public class Portal : MonoBehaviour
{
    public Portal linkedPortal;

    private void OnTriggerEnter(Collider other)
    {
        if (linkedPortal == null) return;

        if (other.CompareTag("Player"))
        {
            CharacterController controller = other.GetComponent<CharacterController>();
            if (controller != null)
            {
                // Evita el doble teleport con un pequeño offset
                controller.enabled = false;
                other.transform.position = linkedPortal.transform.position + Vector3.up * 1.5f;
                controller.enabled = true;
            }
        }
    }
}
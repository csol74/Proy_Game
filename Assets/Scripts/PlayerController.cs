using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 1f;
    public float sprintSpeed = 2f;
    public float jumpHeight = 1f;
    public float gravity = -20f;
    public float rotationSpeed = 8f;
    public float mouseSensitivity = 1f;

    [Header("References")]
    public Transform cameraTransform;
    public Animator animator;
    public Transform spawnPoint;

    [Header("Shield Effect")]
    public GameObject shieldVisual;

    [Header("Keys")]
    public int keysCollected = 0;
    public int totalKeysNeeded = 2;

    private CharacterController controller;
    private Vector3 velocity;
    private float currentSpeed;
    private float yaw;
    private bool isGrounded;
    public bool IsMoving { get; private set; }
    public float CurrentYaw => yaw;

    private int lives = 3;
    private const int maxLives = 3;
    private bool isGameOver = false;

    private bool isShielded = false;
    public float shieldDuration = 8f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;

        if (spawnPoint != null)
            transform.position = spawnPoint.position;

        if (shieldVisual != null)
            shieldVisual.SetActive(false);

        // Inicializar UI de vidas y llaves
        UIManager.Instance.UpdateHearts(lives);
        UIManager.Instance.UpdateKeys(keysCollected, totalKeysNeeded);
    }

    void Update()
    {
        if (isGameOver) return;

        HandleMovement();
        HandleRotation();

        if (transform.position.y < -50f)
        {
            Die();
        }
    }

    void HandleMovement()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 inputDirection = new Vector3(horizontal, 0f, vertical).normalized;

        IsMoving = inputDirection.magnitude > 0.1f;

        if (IsMoving)
        {
            Vector3 moveDirection = Quaternion.Euler(0f, cameraTransform.eulerAngles.y, 0f) * inputDirection;
            bool isSprinting = Input.GetKey(KeyCode.LeftShift);
            currentSpeed = isSprinting ? sprintSpeed : walkSpeed;

            controller.Move(moveDirection * currentSpeed * Time.deltaTime);
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // ACTIVA O DESACTIVA LA ANIMACIÓN DE CAMINAR
        animator.SetBool("Run", IsMoving);
    }

    void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") / mouseSensitivity;
        yaw += mouseX;

        if (IsMoving)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.Euler(0f, yaw, 0f),
                rotationSpeed * Time.deltaTime
            );
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;

        if (body != null && !body.isKinematic && hit.gameObject.CompareTag("Pushable"))
        {
            Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
            body.velocity = pushDir * currentSpeed;
        }

        if (hit.gameObject.CompareTag("Spike") ||
            hit.gameObject.CompareTag("DeadZone") ||
            hit.gameObject.CompareTag("Spear"))
        {
            if (!isShielded)
            {
                Die();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GoldenCarrot"))
        {
            StartCoroutine(ActivateShield());
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Carrot"))
        {
            if (lives < maxLives)
            {
                lives++;
                UIManager.Instance.UpdateHearts(lives);
            }
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Key"))
        {
            keysCollected++;
            Destroy(other.gameObject);
            Debug.Log($"Llaves recolectadas: {keysCollected} / {totalKeysNeeded}");

            UIManager.Instance.UpdateKeys(keysCollected, totalKeysNeeded);
        }
        else if (other.CompareTag("FinalDoor"))
        {
            if (keysCollected >= totalKeysNeeded)
            {
                UIManager.Instance.ShowVictory();
            }
            else
            {
                Debug.Log("Necesitas más llaves para abrir la puerta.");
            }
        }
    }

    private IEnumerator ActivateShield()
    {
        if (isShielded)
            yield break;

        isShielded = true;

        if (shieldVisual != null)
            shieldVisual.SetActive(true);

        yield return new WaitForSeconds(shieldDuration);

        isShielded = false;

        if (shieldVisual != null)
            shieldVisual.SetActive(false);
    }

    void Die()
    {
        lives--;
        UIManager.Instance.UpdateHearts(lives);

        if (lives <= 0)
        {
            GameOver();
            return;
        }

        velocity = Vector3.zero;
        if (spawnPoint != null)
        {
            controller.enabled = false;
            transform.position = spawnPoint.position;
            controller.enabled = true;
        }
    }

    void GameOver()
    {
        isGameOver = true;
        UIManager.Instance.ShowGameOver();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement_Speed")]
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

    [Header("UI")]
    public Image[] heartImages; 
    public GameObject gameOverPanel;

    private CharacterController controller;
    private Vector3 velocity;
    private float currentSpeed;
    private float yaw;
    private bool isGrounded;
    public bool IsMoving { get; private set; }
    public float CurrentYaw => yaw;

    private int lives = 3;
    private bool isGameOver = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;

        if (spawnPoint != null)
            transform.position = spawnPoint.position;

        UpdateHeartsUI();
        gameOverPanel.SetActive(false);
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
    }

    void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") / mouseSensitivity;
        yaw += mouseX;

        if (IsMoving)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0f, yaw, 0f), rotationSpeed * Time.deltaTime);
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Spike") || hit.gameObject.CompareTag("DeadZone"))
        {
            Die();
        }
    }

    void Die()
    {
        lives--;
        UpdateHeartsUI();

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

    void UpdateHeartsUI()
    {
        for (int i = 0; i < heartImages.Length; i++)
        {
            heartImages[i].enabled = i < lives;
        }
    }

    void GameOver()
    {
        isGameOver = true;
        gameOverPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
    }

    
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Paneles UI")]
    public GameObject gameOverPanel;
    public GameObject victoryPanel;

    [Header("Main Menu UI")]
    public GameObject mainMenuPanel;

    [Header("Música de Fondo")]
    // Arrastra aquí tu AudioSource (con el clip ya asignado y Loop = true)
    public AudioSource bgMusicSource;

    [Header("Vidas")]
    public Image[] heartImages;

    [Header("Keys UI")]
    public Image keyIcon;
    public TextMeshProUGUI keyCounterTMP_Text;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        // Al inicio, mostrar el menú principal y detener la música
        ShowMainMenu();

        // Nos aseguramos de ocultar otros paneles
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        if (victoryPanel != null)
            victoryPanel.SetActive(false);
    }

    // ---------------------- Main Menu ----------------------

    public void ShowMainMenu()
    {
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
        }

        // Cuando estemos en el Main Menu, NO suena la música de juego
        if (bgMusicSource != null && bgMusicSource.isPlaying)
            bgMusicSource.Stop();
    }

    public void HideMainMenu()
    {
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
        }

        // Al ocultar el menú principal (iniciar el juego), arrancamos la música en bucle
        if (bgMusicSource != null && !bgMusicSource.isPlaying)
            bgMusicSource.Play();
    }

    public void ExitGame()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }

    // ---------------------- Game Over / Victory ----------------------

    public void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
        }

        // En game over, detenemos la música
        if (bgMusicSource != null && bgMusicSource.isPlaying)
            bgMusicSource.Stop();
    }

    public void ShowVictory()
    {
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
        }

        // En victoria, detenemos la música
        if (bgMusicSource != null && bgMusicSource.isPlaying)
            bgMusicSource.Stop();
    }

    // ---------------------- HUD (Vidas y Llaves) ----------------------

    public void UpdateHearts(int lives)
    {
        for (int i = 0; i < heartImages.Length; i++)
        {
            heartImages[i].enabled = (i < lives);
        }
    }

    public void UpdateKeys(int keysCollected, int totalKeysNeeded)
    {
        if (keyIcon != null)
            keyIcon.enabled = true;

        if (keyCounterTMP_Text != null)
            keyCounterTMP_Text.text = keysCollected + " / " + totalKeysNeeded;
    }
}

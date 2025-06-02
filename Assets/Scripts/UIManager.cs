using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Paneles UI")]
    public GameObject gameOverPanel;
    public GameObject victoryPanel;

    [Header("Vidas")]
    public Image[] heartImages;

    [Header("Keys UI")]
    public Image keyIcon;
    public TextMeshProUGUI keyCounterTMP_Text;
             

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
    }

    public void ShowVictory()
    {
        victoryPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
    }

    public void UpdateHearts(int lives)
    {
        for (int i = 0; i < heartImages.Length; i++)
        {
            heartImages[i].enabled = i < lives;
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

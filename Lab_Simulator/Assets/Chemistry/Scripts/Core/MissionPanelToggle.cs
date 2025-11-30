using UnityEngine;
using UnityEngine.UI;

public class MissionPanelToggle : MonoBehaviour
{
    [Header("Panel ve Butonlar")]
    public GameObject missionPanel;   // MissionPanel GameObject
    public Button missionButton;      // HUD'deki "Görevler" butonu
    public Button closeButton;        // Panel içindeki X butonu (istemiyorsan boş bırak)

    [Header("Klavye kısayolu")]
    public KeyCode toggleKey = KeyCode.G;

    [Header("Panel açıkken kapanacak oyun scriptleri")]
    public MonoBehaviour[] gameplayScriptsToDisable;
    // Örn: PlayerMovement, MouseLook, HandController

    bool isOpen = false;

    void Start()
    {
        // Başlangıçta panel kapalı dursun
        if (missionPanel != null)
            missionPanel.SetActive(false);

        // Görevler butonu
        if (missionButton != null)
            missionButton.onClick.AddListener(TogglePanel);

        // X butonu
        if (closeButton != null)
            closeButton.onClick.AddListener(ClosePanel);
    }

    void OnDestroy()
    {
        if (missionButton != null)
            missionButton.onClick.RemoveListener(TogglePanel);

        if (closeButton != null)
            closeButton.onClick.RemoveListener(ClosePanel);
    }

    void Update()
    {
        // Klavyeden G tuşu ile aç/kapa
        if (Input.GetKeyDown(toggleKey))
        {
            TogglePanel();
        }

        // Panel açıkken ESC ile kapat
        if (isOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            ClosePanel();
        }
    }

    public void TogglePanel()
    {
        if (!isOpen)
            OpenPanel();
        else
            ClosePanel();
    }

    void OpenPanel()
    {
        if (missionPanel == null) return;

        missionPanel.SetActive(true);
        isOpen = true;

        SetGameplayEnabled(false);

        // Cursor'u UI için serbest bırak
        if (MouseManager.Instance != null)
        {
            MouseManager.Instance.UnlockCursor();
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void ClosePanel()
    {
        if (missionPanel == null) return;

        missionPanel.SetActive(false);
        isOpen = false;

        SetGameplayEnabled(true);

        // FPS için cursor'u tekrar kilitle
        if (MouseManager.Instance != null)
        {
            MouseManager.Instance.LockCursor();
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void SetGameplayEnabled(bool enabled)
    {
        foreach (var s in gameplayScriptsToDisable)
        {
            if (s != null)
                s.enabled = enabled;
        }
    }
}

using UnityEngine;
using TMPro;

public class InfoPanelUI : MonoBehaviour
{
    public static InfoPanelUI Instance;

    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descText;
    public TextMeshProUGUI safetyText;

    void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    public void Show(InfoData data)
    {
        Debug.Log("INFO PANEL AÇILDI: " + gameObject.name);

        titleText.text = data.title;
        descText.text = data.description;
        safetyText.text = data.safetyNotes;

        gameObject.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}

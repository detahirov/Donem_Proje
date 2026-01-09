using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MissionListUI : MonoBehaviour
{
    public Transform contentRoot;
    public GameObject missionRowPrefab;

    public Color doneColor = Color.gray;
    public Color currentColor = Color.yellow;
    public Color futureColor = Color.white;

    List<GameObject> spawnedRows = new List<GameObject>();

    void OnEnable()
    {
        Debug.Log("[MissionListUI] OnEnable çaðrýldý");
        RebuildList();

        if (MissionManager.Instance != null)
            MissionManager.Instance.OnMissionChanged += OnMissionChanged;
        else
            Debug.LogWarning("[MissionListUI] MissionManager.Instance == null");
    }

    void OnDisable()
    {
        if (MissionManager.Instance != null)
            MissionManager.Instance.OnMissionChanged -= OnMissionChanged;
    }

    void OnMissionChanged(MissionSO newMission)
    {
        Debug.Log("[MissionListUI] OnMissionChanged, liste yenileniyor");
        RebuildList();
    }


    public void RebuildList()
    {
        if (MissionManager.Instance == null) return;

        var missions = MissionManager.Instance.missions;
        int currentIndex = MissionManager.Instance.currentMissionIndex;

        // Eski satýrlarý temizle
        foreach (Transform child in contentRoot)
        {
            Destroy(child.gameObject);
        }
        spawnedRows.Clear();

        for (int i = 0; i < missions.Count; i++)
        {
            var m = missions[i];
            GameObject row = Instantiate(missionRowPrefab, contentRoot);
            spawnedRows.Add(row);

            // TextMeshPro bileþenlerini bul (Garanti yöntem)
            var texts = row.GetComponentsInChildren<TextMeshProUGUI>();
            TextMeshProUGUI titleText = null;
            TextMeshProUGUI descText = null;

            // Ýsim kontrolü yerine sýraya göre atama (Prefab yapýna göre)
            // Genellikle 1. baþlýk, 2. açýklamadýr.
            if (texts.Length > 0) titleText = texts[0];
            if (texts.Length > 1) descText = texts[1];

            if (titleText != null) titleText.text = m.title;
            if (descText != null) descText.text = m.description;

            bool isDone = (i < currentIndex);
            bool isCurrent = (i == currentIndex);

            var img = row.GetComponent<Image>();

            if (isDone)
            {
                // Tamamlanmýþ Görev
                if (titleText != null)
                {
                    titleText.color = doneColor;
                    titleText.fontStyle |= FontStyles.Strikethrough;
                }
                if (descText != null) descText.color = doneColor;
                if (img != null) img.color = new Color(0.2f, 0.2f, 0.2f, 0.6f);
            }
            else if (isCurrent)
            {
                // Aktif Görev
                if (titleText != null)
                {
                    titleText.color = currentColor;
                    titleText.fontStyle &= ~FontStyles.Strikethrough; // Çizgiyi kaldýr
                }
                if (descText != null) descText.color = currentColor;
                if (img != null) img.color = new Color(0.5f, 0.5f, 0.0f, 0.8f); // Biraz daha belirgin
            }
            else
            {
                // Gelecek Görev
                if (titleText != null)
                {
                    titleText.color = futureColor;
                    titleText.fontStyle &= ~FontStyles.Strikethrough;
                }
                if (descText != null) descText.color = futureColor;
                if (img != null) img.color = new Color(0.1f, 0.1f, 0.1f, 0.5f);
            }
        }
    }
}

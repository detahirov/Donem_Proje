using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MissionListUI : MonoBehaviour
{
    [Header("Baðlantýlar")]
    public Transform contentRoot;          // ScrollView/Viewport/Content
    public GameObject missionRowPrefab;    // MissionRowPrefab
    public Color doneColor = Color.gray;
    public Color currentColor = Color.yellow;
    public Color futureColor = Color.white;

    List<GameObject> spawnedRows = new List<GameObject>();

    void OnEnable()
    {
        // Panel açýldýðýnda listeyi yenile
        RebuildList();

        if (MissionManager.Instance != null)
            MissionManager.Instance.OnMissionChanged += OnMissionChanged;
    }

    void OnDisable()
    {
        if (MissionManager.Instance != null)
            MissionManager.Instance.OnMissionChanged -= OnMissionChanged;
    }

    void OnMissionChanged(MissionSO newMission)
    {
        RebuildList();
    }

    void RebuildList()
    {
        if (MissionManager.Instance == null) return;

        // Önce eski satýrlarý temizle
        foreach (var go in spawnedRows)
        {
            if (go != null) Destroy(go);
        }
        spawnedRows.Clear();

        var missions = MissionManager.Instance.missions;
        int currentIndex = MissionManager.Instance.currentMissionIndex;

        for (int i = 0; i < missions.Count; i++)
        {
            var m = missions[i];
            GameObject row = Instantiate(missionRowPrefab, contentRoot);
            spawnedRows.Add(row);

            // Çocuklarý bul
            var texts = row.GetComponentsInChildren<TextMeshProUGUI>();
            TextMeshProUGUI titleText = null;
            TextMeshProUGUI descText = null;

            foreach (var t in texts)
            {
                if (t.gameObject.name.Contains("Title"))
                    titleText = t;
                else if (t.gameObject.name.Contains("Desc"))
                    descText = t;
            }

            if (titleText != null)
                titleText.text = m.title;

            if (descText != null)
                descText.text = m.description;

            bool isDone = (i < currentIndex);
            bool isCurrent = (i == currentIndex);

            // Renk / stil ayarý
            var img = row.GetComponent<Image>();

            if (isDone)
            {
                if (titleText != null) titleText.color = doneColor;
                if (descText != null) descText.color = doneColor;
                if (img != null) img.color = new Color(0.2f, 0.2f, 0.2f, 0.6f);

                // Üstü çizili yapmak istiyorsan:
                if (titleText != null) titleText.fontStyle |= FontStyles.Strikethrough;
            }
            else if (isCurrent)
            {
                if (titleText != null) titleText.color = currentColor;
                if (descText != null) descText.color = currentColor;
                if (img != null) img.color = new Color(0.3f, 0.3f, 0.0f, 0.7f);
            }
            else
            {
                if (titleText != null) titleText.color = futureColor;
                if (descText != null) descText.color = futureColor;
                if (img != null) img.color = new Color(0.1f, 0.1f, 0.1f, 0.5f);
            }
        }
    }
}

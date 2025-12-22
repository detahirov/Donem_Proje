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
        if (MissionManager.Instance == null)
        {
            Debug.LogWarning("[MissionListUI] RebuildList: MissionManager.Instance yok!");
            return;
        }

        var missions = MissionManager.Instance.missions;
        int currentIndex = MissionManager.Instance.currentMissionIndex;

        Debug.Log($"[MissionListUI] RebuildList: {missions.Count} görev bulundu, currentIndex={currentIndex}");

        // Eski satýrlarý sil
        foreach (var go in spawnedRows)
        {
            if (go != null) Destroy(go);
        }
        spawnedRows.Clear();

        for (int i = 0; i < missions.Count; i++)
        {
            var m = missions[i];
            GameObject row = Instantiate(missionRowPrefab, contentRoot);
            spawnedRows.Add(row);

            // Prefab içindeki textleri bul
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

            // Ýsimleri farklý koyduysan direkt children[0]/[1] da yapabilirsin:
            if (titleText == null || descText == null)
            {
                var tmps = row.GetComponentsInChildren<TextMeshProUGUI>();
                if (tmps.Length > 0) titleText = tmps[0];
                if (tmps.Length > 1) descText = tmps[1];
            }

            if (titleText != null)
                titleText.text = m.title;

            if (descText != null)
                descText.text = m.description;

            bool isDone = (i < currentIndex);
            bool isCurrent = (i == currentIndex);

            var img = row.GetComponent<Image>();

            if (isDone)
            {
                if (titleText != null) titleText.color = doneColor;
                if (descText != null) descText.color = doneColor;
                if (img != null) img.color = new Color(0.2f, 0.2f, 0.2f, 0.6f);
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

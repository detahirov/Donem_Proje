using UnityEngine;
using UnityEngine.UI;

public class MissionUI : MonoBehaviour
{
    public Text titleText;
    public Text descriptionText;

    void Start()
    {
        if (MissionManager.Instance != null)
        {
            MissionManager.Instance.OnMissionChanged += UpdateUI;
            UpdateUI(MissionManager.Instance.CurrentMission);
        }
    }

    void OnDestroy()
    {
        if (MissionManager.Instance != null)
            MissionManager.Instance.OnMissionChanged -= UpdateUI;
    }

    void UpdateUI(MissionSO mission)
    {
        if (mission == null)
        {
            titleText.text = "Görev Yok";
            descriptionText.text = "";
        }
        else
        {
            titleText.text = mission.title;
            descriptionText.text = mission.description;
        }
    }
}

using UnityEngine;

[CreateAssetMenu(menuName = "Chemistry/Info Data")] 
public class InfoData : ScriptableObject
{
    [Header("Genel")]
    public string title;

    [TextArea(4, 10)]
    public string description;

    [Header("Kategori")]
    public InfoCategory category;

    [Header("Etkileþimler")]
    public InfoData[] relatedObjects;

    [Header("Güvenlik")]
    [TextArea(2, 6)]
    public string safetyNotes;
}

public enum InfoCategory
{
    Chemical,
    Tool,
    Equipment,
    Reaction
}

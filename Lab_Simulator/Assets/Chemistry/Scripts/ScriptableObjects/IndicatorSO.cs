// Assets/Chemistry/Scripts/ScriptableObjects/IndicatorSO.cs
using UnityEngine;

[CreateAssetMenu(menuName = "Chemistry/Indicator")]
public class IndicatorSO : ScriptableObject
{
    public SubstanceSO indicator; // fenolftalein SO'su
    [System.Serializable]
    public struct PHColorBand
    {
        public float minPH, maxPH;
        public Color color;
    }
    public PHColorBand[] bands;
    public Color GetColorForPH(float ph)
    {
        foreach (var b in bands)
        {
            if (ph >= b.minPH && ph <= b.maxPH) return b.color;
        }
        return Color.white;
    }
}

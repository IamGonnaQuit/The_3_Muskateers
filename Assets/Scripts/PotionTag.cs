using UnityEngine;

[CreateAssetMenu(fileName = "NewPotionTag", menuName = "Potions/Potion Tag")]
public class PotionTag : ScriptableObject
{
    public string tagName;

    [TextArea(2, 4)]
    public string tagDescription;
}

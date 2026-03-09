using UnityEngine;

[CreateAssetMenu(fileName = "New Fish", menuName = "Fishing Game/Fish")]
public class FishData : ScriptableObject
{
    public string fishName;
    public Sprite icon;
    public int sellValue;
    public int maxStack = 20;
}
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;

    [Header("Equipped Prefab")]
    public GameObject equippedPrefab; // prefab positioned for FPS hands/camera
}
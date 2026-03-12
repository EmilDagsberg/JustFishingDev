using UnityEngine;

[CreateAssetMenu(fileName = "New Store Item", menuName = "Shop/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public int price;

    [Header("Spawn On Buy")]
    public GameObject prefabToSpawn;
    public Vector3 spawnPosition;
    public Vector3 spawnRotation;

    [Header("Purchase Rules")]
    public bool canOnlyBuyOnce;
}

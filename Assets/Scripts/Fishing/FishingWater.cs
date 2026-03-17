using UnityEngine;

public class FishingWater : MonoBehaviour
{
    [Header("Fish available in this water")]
    [SerializeField] private FishData[] availableFish;

    public FishData[] AvailableFish => availableFish;
}
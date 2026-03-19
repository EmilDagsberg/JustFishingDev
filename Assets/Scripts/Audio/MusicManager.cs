using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    private MusicZone currentZone;
    private List<MusicZone> activeZones = new List<MusicZone>();

    private void Awake()
    {
        Instance = this;
        Debug.Log("MusicManager initialized");
    }

    public void RequestPlay(MusicZone zone)
    {
        Debug.Log("RequestPlay called from: " + zone.gameObject.name + " | currentZone: " + (currentZone != null ? currentZone.gameObject.name : "null"));

        if (!activeZones.Contains(zone))
            activeZones.Add(zone);

        if (currentZone == zone) return;

        if (currentZone != null)
        {
            Debug.Log("Fading out: " + currentZone.gameObject.name);
            currentZone.FadeTo(0f);
        }

        currentZone = zone;
        currentZone.FadeTo(currentZone.chosenVolume);
    }

    public void RequestStop(MusicZone zone)
    {
        Debug.Log("RequestStop called from: " + zone.gameObject.name);

        activeZones.Remove(zone);

        if (currentZone != zone) return;

        currentZone.FadeTo(0f);
        currentZone = null;

        // After leaving purple forest go back to priority '0' main background music
        MusicZone goBack = null;
        int highestPrio = int.MinValue;
        foreach (var m in activeZones)
        {
            if (m.priority > highestPrio)
            {
                highestPrio = m.priority;
                goBack = m;
            }
        }

        if (goBack != null)
        {
            currentZone = goBack;
            currentZone.FadeTo(currentZone.chosenVolume);
        }
    }
}
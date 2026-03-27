using UnityEngine;
using System.Collections.Generic;

public class WaypointManager : MonoBehaviour
{
    public static WaypointManager Instance { get; private set; }

    [Header("Waypoints")]
    public List<Transform> alleWegpunkte = new List<Transform>();

    private void Awake()
    {
        // Singleton-Pattern: Damit wir von überall darauf zugreifen können
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); return; }

        // Automatisch alle Kinder des Objekts als Wegpunkte registrieren
        foreach (Transform child in transform)
        {
            alleWegpunkte.Add(child);
        }
    }

    public Transform GetRandomWaypoint()
    {
        if (alleWegpunkte.Count == 0) return null;
        return alleWegpunkte[Random.Range(0, alleWegpunkte.Count)];
    }
}
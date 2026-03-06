using UnityEngine;
using Barmetler.RoadSystem;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class DynamischeRoadVolumes : MonoBehaviour
{
    [Header("Straßen Einstellungen")]
    public GameObject volumePrefab;
    public float abstand = 2f;

    [Header("Gehweg Einstellungen")]
    public GameObject sidewalkPrefab;   // Dein neues Gehweg-Prefab
    public float sidewalkOffset = 5.5f; // Wie weit links/rechts vom Zentrum?

    private GameObject volumeContainer;

    public void GeneriereEntlangSpline()
    {
        LoescheAlteVolumes();

        if (volumePrefab == null)
        {
            Debug.LogWarning("Bitte Volume Prefab zuweisen!");
            return;
        }

        Road road = GetComponent<Road>();
        if (road == null)
        {
            Debug.LogError("Kein Road-Skript auf diesem Objekt gefunden!");
            return;
        }

        volumeContainer = new GameObject("Generierte_Volumes");
        volumeContainer.transform.SetParent(this.transform);
        volumeContainer.transform.localPosition = Vector3.zero;
        volumeContainer.transform.localRotation = Quaternion.identity;

        var points = road.GetEvenlySpacedPoints(abstand);

        for (int i = 0; i < points.Length; i++)
        {
            Vector3 worldPos = transform.TransformPoint(points[i].position);
            Quaternion worldRot = Quaternion.identity;
            Vector3 richtung = Vector3.forward;

            // Richtung berechnen
            if (i < points.Length - 1)
            {
                Vector3 nextWorldPos = transform.TransformPoint(points[i + 1].position);
                richtung = (nextWorldPos - worldPos).normalized;
            }
            else if (i > 0)
            {
                Vector3 prevWorldPos = transform.TransformPoint(points[i - 1].position);
                richtung = (worldPos - prevWorldPos).normalized;
            }

            if (richtung != Vector3.zero) 
                worldRot = Quaternion.LookRotation(richtung);

            // --- NEU: GEHWEG LOGIK ---
            // Wir berechnen "Rechts" relativ zur Fahrtrichtung (Kreuzprodukt)
            Vector3 rightVector = Vector3.Cross(Vector3.up, richtung).normalized;

            // 1. Die Straße (Mitte)
            Instantiate(volumePrefab, worldPos, worldRot, volumeContainer.transform);

            // 2. Gehweg Links und Rechts (nur wenn Prefab zugewiesen)
            if (sidewalkPrefab != null)
            {
                Vector3 linksPos = worldPos - (rightVector * sidewalkOffset);
                Vector3 rechtsPos = worldPos + (rightVector * sidewalkOffset);

                Instantiate(sidewalkPrefab, linksPos, worldRot, volumeContainer.transform);
                Instantiate(sidewalkPrefab, rechtsPos, worldRot, volumeContainer.transform);
            }
        }

        Debug.Log($"Check: {points.Length} Straßenabschnitte inkl. Gehwege erstellt.");
    }

    public void LoescheAlteVolumes()
    {
        Transform alterContainer = transform.Find("Generierte_Volumes");
        if (alterContainer != null)
        {
            // Im Editor-Modus nutzen wir DestroyImmediate
            DestroyImmediate(alterContainer.gameObject);
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(DynamischeRoadVolumes))]
public class DynamischeRoadVolumesEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        DynamischeRoadVolumes script = (DynamischeRoadVolumes)target;

        GUILayout.Space(15);
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("VOLUMES GENERIEREN", GUILayout.Height(40)))
        {
            script.GeneriereEntlangSpline();
        }

        GUI.backgroundColor = Color.white;
        if (GUILayout.Button("Alles Löschen", GUILayout.Height(25)))
        {
            script.LoescheAlteVolumes();
        }
    }
}
#endif
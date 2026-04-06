using UnityEngine;

public class StorageCabinetInteract : MonoBehaviour
{
    [Header("Spawner Settings")]
    [SerializeField] private Item itemPrefab;
    [SerializeField] private Transform spawnPoint;
    public bool HasItemPrefab => itemPrefab != null;

    public bool TrySpawnItem(PlayerInteraction player)
    {
        if (player == null)
        {
            return false;
        }

        // Cabinet interaction is consumed even when the player already holds something.
        if (player.CurrentItem != null)
        {
            return true;
        }

        if (itemPrefab == null)
        {
            Debug.LogWarning($"Storage cabinet '{name}' has no itemPrefab assigned.");
            return true;
        }

        Vector3 spawnPosition = spawnPoint != null ? spawnPoint.position : transform.position;
        Quaternion spawnRotation = spawnPoint != null ? spawnPoint.rotation : transform.rotation;

        Item spawnedItem = Instantiate(itemPrefab, spawnPosition, spawnRotation);
        player.ForceEquipItem(spawnedItem);

        return true;
    }
}

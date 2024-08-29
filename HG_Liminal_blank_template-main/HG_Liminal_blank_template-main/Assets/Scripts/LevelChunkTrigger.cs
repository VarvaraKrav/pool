using UnityEngine;

public class LevelChunkTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider belongs to a LevelChunk
        if (other.CompareTag("LevelChunk"))
        {
            // Destroy the LevelChunk
            Destroy(other.gameObject);
        }
    }
}

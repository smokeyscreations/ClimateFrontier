using UnityEngine;
using System.Collections;

public class EnemyFlashEffect3D : MonoBehaviour
{
    [Header("Flash Settings")]
    public SkinnedMeshRenderer enemySkinnedMeshRenderer; // Assign via Inspector
    public int flashMaterialIndex = 1; // Assuming the second material is the flash material
    public float flashDuration = 0.2f;
    public Color flashColor = new Color(1f, 1f, 1f, 0.7f); // Semi-transparent white

    private Coroutine currentFlashCoroutine;
    private MaterialPropertyBlock propBlock;

    private void Awake()
    {
        // If not assigned in Inspector, attempt to find the SkinnedMeshRenderer
        if (enemySkinnedMeshRenderer == null)
        {
            enemySkinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
            if (enemySkinnedMeshRenderer == null)
            {
                Debug.LogError($"{gameObject.name}: No SkinnedMeshRenderer found.");
            }
        }

        // Initialize the Material Property Block
        propBlock = new MaterialPropertyBlock();
    }

    // Method to trigger the flash
    public void TriggerFlash()
    {
        if (gameObject.activeInHierarchy)
        {
            if (currentFlashCoroutine != null)
            {
                StopCoroutine(currentFlashCoroutine);
            }
            currentFlashCoroutine = StartCoroutine(FlashCoroutine());
        }
        else
        {
            Debug.LogWarning($"{gameObject.name}: Attempted to trigger flash on an inactive GameObject.");
        }
    }

    private IEnumerator FlashCoroutine()
    {
        if (enemySkinnedMeshRenderer == null || enemySkinnedMeshRenderer.sharedMaterials.Length <= flashMaterialIndex)
        {
            Debug.LogError($"{gameObject.name}: Flash material not assigned or index out of range.");
            yield break;
        }

        // Ensure propBlock is initialized
        if (propBlock == null)
        {
            propBlock = new MaterialPropertyBlock();
        }

        // Get the current property block
        enemySkinnedMeshRenderer.GetPropertyBlock(propBlock, flashMaterialIndex);

        // Set the flash color (Albedo)
        propBlock.SetColor("_BaseColor", flashColor); 
        enemySkinnedMeshRenderer.SetPropertyBlock(propBlock, flashMaterialIndex);

        // Wait for the duration
        yield return new WaitForSeconds(flashDuration);

        // Reset the flash color to transparent
        propBlock.SetColor("_BaseColor", new Color(1f, 1f, 1f, 0f)); // Transparent
        enemySkinnedMeshRenderer.SetPropertyBlock(propBlock, flashMaterialIndex);

        currentFlashCoroutine = null;
    }

    public void ResetFlash()
    {
        if (enemySkinnedMeshRenderer == null || enemySkinnedMeshRenderer.sharedMaterials.Length <= flashMaterialIndex)
        {
            Debug.LogError($"{gameObject.name}: Flash material not assigned or index out of range.");
            return;
        }

        // Ensure propBlock is initialized
        if (propBlock == null)
        {
            propBlock = new MaterialPropertyBlock();
        }

        // Get the current property block
        enemySkinnedMeshRenderer.GetPropertyBlock(propBlock, flashMaterialIndex);

        // Set the flash color to transparent
        propBlock.SetColor("_BaseColor", new Color(1f, 1f, 1f, 0f));
        enemySkinnedMeshRenderer.SetPropertyBlock(propBlock, flashMaterialIndex);
    }
}

using UnityEngine;
using System.Collections;

public class WolfAttackTarget : MonoBehaviour
{
    [Header("Targeting")]
    [Tooltip("If not assigned, will automatically find the nearest enemy within searchRadius.")]
    public GameObject Target;
    public LayerMask enemyLayer;        // The layer(s) on which enemies reside
    public float searchRadius = 100f;   // How far the projectile searches for an enemy if Target is not assigned
    public bool pickNearestEnemy = true; // If true, picks the nearest enemy from the enemyLayer

    private GameObject currentTarget;
    private RFX1_TransformMotion transformMotion;

    void OnEnable()
    {
        // If the projectile is pooled, OnEnable can act like 'spawn-time' logic.
        transformMotion = GetComponentInChildren<RFX1_TransformMotion>();
        if (transformMotion == null)
        {
            Debug.LogError("You must attach RFX1_TransformMotion script to your projectile prefab!");
            return;
        }

        // If no explicit target was assigned externally, try auto-find
        if (Target == null && pickNearestEnemy)
        {
            Target = FindNearestEnemy();
        }

        UpdateTarget();
    }

    void Update()
    {
        // If we allow dynamic updates to the target
        UpdateTarget();
    }

    private void UpdateTarget()
    {
        if (transformMotion == null) return; // We need RFX1_TransformMotion

        if (Target != currentTarget)
        {
            currentTarget = Target;
            transformMotion.Target = currentTarget;
        }
    }

    /// <summary>
    /// Finds the nearest enemy within searchRadius on enemyLayer.
    /// Returns null if none found.
    /// </summary>
    private GameObject FindNearestEnemy()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, searchRadius, enemyLayer);
        if (colliders.Length == 0)
        {
            return null;
        }

        float minDist = Mathf.Infinity;
        Collider closest = null;
        foreach (var col in colliders)
        {
            float dist = Vector3.Distance(transform.position, col.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = col;
            }
        }

        return closest != null ? closest.gameObject : null;
    }
}

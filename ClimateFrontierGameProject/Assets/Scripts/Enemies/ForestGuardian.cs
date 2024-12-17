using UnityEngine;
using EnemyStates;
using TMPro;

public class ForestGuardian : BaseEnemy
{
    private Color floatingColor = Color.red;
    protected override void Awake()
    {
        base.Awake();
        goldReward = 13;
        InitializeAttributes();
    }

    protected override void ShowFloatingDamage(float damage)
    {
        if (floatingTextPrefab == null)
        {
            Debug.LogError($"{gameObject.name}: FloatingTextPrefab is not assigned.");
            return;
        }

        // Define spawn position slightly above the enemy to prevent overlap
        Vector3 spawnPosition = transform.position + new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f), 1.5f, UnityEngine.Random.Range(-0.5f, 0.5f));

        // Spawn a FloatingText instance from the pool
        GameObject floatingTextGO = ObjectPooler.Instance.SpawnFromPool("FloatingText", spawnPosition, Quaternion.identity);
        textMesh = floatingTextGO.GetComponent<TextMeshPro>();

        if (floatingTextGO != null)
        {
            FloatingText floatingText = floatingTextGO.GetComponent<FloatingText>();
            if (floatingText != null)
            {
                // Set the damage number
                floatingText.SetText(Mathf.RoundToInt(damage).ToString());
                textMesh.color = floatingColor;
            }
            else
            {
                Debug.LogError($"{gameObject.name}: FloatingText component not found on the prefab.");
            }
        }
    }
    private void InitializeAttributes()
    {
        AttackRange = 1f;
    }

    //public override void PerformAttack()
    //{
        

    //    if (Target != null && Vector3.Distance(transform.position, Target.position) <= AttackRange)
    //    {
    //        Debug.Log("Forest Guardian performs a powerful attack!");
    //    }
    //}
}

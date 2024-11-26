using UnityEngine;
using EnemyStates;
using TMPro;

public class DarkElf : BaseEnemy
{
    private Color floatingColor = Color.red;
    private float attackRange = 6f;
    public float projectileSpeed = 2f;
    public int projectileDamage = 10;
    public string projectileTag = "MagicProjectile";
    protected override void Awake()
    {
        base.Awake();
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
        AttackRange = attackRange;
    }

    public override void PerformAttack()
    {
        if (Target != null && Vector3.Distance(transform.position, Target.position) <= attackRange)
        {
            // Define a vertical offset of 1 unit
            Vector3 spawnOffset = Vector3.up * 1f; // 1 unit upwards

            // Calculate spawn position with forward direction and vertical offset
            Vector3 spawnPosition = transform.position + transform.forward + spawnOffset;

            // Spawn a projectile from the pool at the calculated spawn position
            GameObject projectile = ObjectPooler.Instance.SpawnFromPool(projectileTag, spawnPosition, Quaternion.identity);
            if (projectile != null)
            {
                // Initialize the projectile
                MagicProjectile magicProjectile = projectile.GetComponent<MagicProjectile>();
                if (magicProjectile != null)
                {
                    magicProjectile.Initialize(Target, projectileSpeed, projectileDamage);
                }

                Debug.Log($"{gameObject.name} casts a magic spell towards the player.");
            }
            else
            {
                Debug.LogError($"Failed to spawn projectile with tag '{projectileTag}'.");
            }
        }
    }


    //public override void PerformAttack()
    //{


    //    if (Target != null && Vector3.Distance(transform.position, Target.position) <= AttackRange)
    //    {
    //        Debug.Log("Forest Guardian performs a powerful attack!");
    //    }
    //}
}

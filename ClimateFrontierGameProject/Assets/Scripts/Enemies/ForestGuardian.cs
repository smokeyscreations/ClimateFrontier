using UnityEngine;
using EnemyStates;

public class ForestGuardian : BaseEnemy
{
    protected override void Awake()
    {
        base.Awake();
        InitializeAttributes();
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

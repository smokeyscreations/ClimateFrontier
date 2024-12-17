using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyPoolData
{
    public BaseEnemy prefab;      // The enemy prefab reference
    public int maxCount;      // Maximum active enemies of this type at once

    [HideInInspector] public Queue<BaseEnemy> queue; // The internal pool queue, set at runtime
    [HideInInspector] public int activeCount = 0;    // Number of active enemies currently in the scene
}

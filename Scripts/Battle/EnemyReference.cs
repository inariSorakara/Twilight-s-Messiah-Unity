using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component used to keep track of an enemy's source SO and its index in encounter pools
/// </summary>
public class EnemyReference : MonoBehaviour
{
    // The EnemySO that created this enemy
    public EnemySO SourceEnemySO { get; set; }
    
    // The index of this enemy in its source pool
    public int EnemyIndex { get; set; } = -1;
    
    // Whether this enemy is a challenger
    public bool IsChallenger { get; set; } = false;
}

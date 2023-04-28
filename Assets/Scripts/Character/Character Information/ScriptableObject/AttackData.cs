using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// store attack data
[CreateAssetMenu(fileName = "Attack Data", menuName = "Character Information/Attack")]

public class AttackData : ScriptableObject
{
    [Header("Attack Info")]
    public float attackRange;
    public float skillRange;
    public float coolDown;
    public float minDamage;
    public float maxDamage;
    public float criticalMultiplier;
    public float criticalChance;
}

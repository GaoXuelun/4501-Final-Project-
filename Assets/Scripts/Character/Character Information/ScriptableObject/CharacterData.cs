using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// store character data
[CreateAssetMenu(fileName = "Character Data", menuName = "Character Information/Data")]

public class CharacterData : ScriptableObject
{
    [Header("Stats Info")]
    public float maxHP;
    public float currentHP;
    public float armor;
    public float currentArmor;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CharacterStats : MonoBehaviour
{
    public event Action<float, float> ActionUpdateHPBarUI;
    public CharacterData sampleData;
    public CharacterData characterData;
    public AttackData attackData;

    [HideInInspector]
    public bool isCritical;

    void Awake()
    {
        if (sampleData != null)
            characterData = Instantiate(sampleData);    // copy sample data to create a new one, so that data of each obj is independent
    }

    #region read from characterData
    public float MaxHP
    {
        get => characterData != null ? characterData.maxHP : 0;
        set => characterData.maxHP = value;
    }
    public float CurrentHP
    {
        get => characterData != null ? characterData.currentHP : 0;
        set => characterData.currentHP = value;
    }
    public float Armor
    {
        get => characterData != null ? characterData.armor : 0;
        set => characterData.armor = value;
    }
    public float CurrentArmor
    {
        get => characterData != null ? characterData.currentArmor : 0;
        set => characterData.currentArmor = value;
    }
    #endregion

    #region combat calculation
    public void TakeDamage(CharacterStats attacker, CharacterStats target)
    {
        float damage = Mathf.Max(attacker.CurrentDamage() - target.CurrentArmor, 1);    // calculate final damage and set a min damage to avoid 0 or negative damage
        CurrentHP = Mathf.Max(CurrentHP - damage, 0);   // calculate current hp and set 0 as minimum boundary
        if (isCritical) // if attack is critical, play hit animation
            target.GetComponent<Animator>().SetTrigger("Hit");
        ActionUpdateHPBarUI?.Invoke(CurrentHP, MaxHP);
    }

    public void TakeDamage(int damage, CharacterStats target)
    {
        float fixDamage = Mathf.Max(damage - target.CurrentArmor, 1);     // calculate final damage and set a min damage to avoid 0 or negative damage
        CurrentHP = Mathf.Max(CurrentHP - fixDamage, 0);   // calculate current hp and set 0 as minimum boundary
    }

    private float CurrentDamage()
    {
        float damage = UnityEngine.Random.Range(attackData.minDamage, attackData.maxDamage);    // attack damage is random in range
        if (isCritical)
            damage *= attackData.criticalMultiplier;
        return damage;
    }
    #endregion
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Character : BattleableEntity {

    private CharacterInfo characterInfo;

    public EquipmentInventory EquipmentInventory { get { return equipmentInventory; } }
    private EquipmentInventory equipmentInventory;
   
    public override Transform WorldInfo { get { return PartyManager.Instance.PartyController.GetCorrespondingPartyMember(this).transform; } }

    protected override float Physical_Damage {
        get {
            float damage = base.GetStat<Stat>(StatName.Attack).MaxValue;

            if (IsCriticalHit()) // ability/skill/spell damage/not taken into consideration yet
                damage *= 1 + base.GetStat<Stat>(StatName.Critical_Damage).MaxValue;

            damage *= (1 + GetStat<ElementalStat>(StatName.Elemental, characterInfo.ElementType, ElementalStatType.Damage).MaxValue); // elemental damage bonus

            return damage; 
        }
    }

    protected override float Magic_Damage {
        get {
            float magic_damage = base.GetStat<Stat>(StatName.Magic_Attack).MaxValue;

            magic_damage *= 1 + base.GetStat<DepletableStat>(StatName.Mana).MaxValue / 100.0f; // scaling magic damage based on total mana

            if (IsCriticalHit()) 
                magic_damage *= 1 + base.GetStat<Stat>(StatName.Critical_Damage).MaxValue;

            return magic_damage;
        }
    }

    protected override float Get_Outgoing_Damage(bool isPhysical) {
        return base.Get_Outgoing_Damage(isPhysical);
    }

    protected override float Get_Incoming_Damage(float rawDamage, bool isPhysical) { // magic defense
        float defense = base.GetStat<Stat>(StatName.Defense).MaxValue;
        float magic_defense = base.GetStat<Stat>(StatName.Magic_Defense).MaxValue;

        float def_damage_reduction = defense / (defense + 5.0f * level.RoundedCurrentLevel + 500);
        float magic_def_damage_reduction = magic_defense / (magic_defense + 5.0f * level.RoundedCurrentLevel + 500);

        float res_damage_reduction = base.GetStat<Stat>(StatName.Resistance).MaxValue;
        float elemental_damage_reduction = GetStat<ElementalStat>(StatName.Elemental, characterInfo.ElementType, ElementalStatType.Resistance).MaxValue;

        return rawDamage * (1 - (isPhysical ? def_damage_reduction : magic_def_damage_reduction)) * (1 - res_damage_reduction) * (1 - elemental_damage_reduction);
    }

    public Skill GetSkill(SkillType skillType) {
        return characterInfo.Skills.Where(skill => skill.SkillType == skillType).FirstOrDefault(); // support for mutiple skills add later
    }

    public override LivingStatus TakeDamage(float rawDamage, bool isPhysical) {
        return base.TakeDamage(rawDamage, isPhysical); 
    }

    public override void Die() {
        // doesn't actually die for now, just sets the sprite to invis
        foreach (SpriteRenderer spriteRenderer in WorldInfo.GetComponentsInChildren<SpriteRenderer>())
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0);
    }

    public void Revive() {
        foreach (SpriteRenderer spriteRenderer in WorldInfo.GetComponentsInChildren<SpriteRenderer>())
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1);

        base.GetStat<DepletableStat>(StatName.HP).AddValue(StatModType.Flat, float.MaxValue);

        OnHealthChange?.Invoke();
    }

    private bool IsCriticalHit() {
        return UnityEngine.Random.Range(0.0f, 1.0f) < base.GetStat<Stat>(StatName.Critical_Rate).MaxValue * (1 + base.GetStat<Stat>(StatName.Luck).MaxValue);
    }

    public override void SetEntityInfo(LivingEntityInfo livingEntityInfo) {
        characterInfo = (CharacterInfo)livingEntityInfo;

        InitCharacter();
    }

    public override T GetEntityInfo<T>() {
        return (T)(LivingEntityInfo)characterInfo;
    }

    // this is an exponential equation that gives us a lot of control over the balance of the exp from level to level (yoinked from oldschool runescape) // https://oldschool.runescape.wiki/w/Experience#Formula kinda hard to implement tho
    public override Func<float, float> levelScalingFunction => (x) => 1.0f / 8.0f * (x * x - x + 600 * ( (Mathf.Pow(2, x / 7.0f) - Mathf.Pow(2, 1.0f / 7.0f)) / (Mathf.Pow(2, 1.0f / 7.0f) - 1) ));

    protected override void Awake() {
        base.Awake();

        equipmentInventory = new EquipmentInventory();
    }

    public void InitCharacter() {
        this.SetUpSkills();
        base.SetUpStats();
    }

    public void SetUpSkills() {
        if (characterInfo.Skills == null || characterInfo.Skills.Count <= 0)
            return;

        foreach (Skill skill in characterInfo.Skills)
            if (skill != null)
                skill.InitSkill();
    }
}

public enum CharacterArchetype {
    All_Rounder,
    Berserker,
    Archer,
    Tank,
    Rogue,
    Mage
}

using UnityEngine;

[CreateAssetMenu(fileName = "New Effect Skill", menuName = "Entity/Skill/Effect Skill")]
public class EffectSkill : Skill {
    
    public override SkillEffect[] SkillEffects { get { return _StatEffects; } }
	[SerializeField] private StatEffect[] _StatEffects;

    public override void Deactivate() {
        foreach (LivingEntity affectedTarget in AffectedTargets) {
            foreach (StatEffect statEffect in _StatEffects)
                statEffect.RemoveEffect(affectedTarget);
        }

        AffectedTargets.Clear();
    }

    [System.Serializable]
    public class StatEffect : SkillEffect { // assuming all stat effects from skills are tempoary and follow the Skill ActiveDuration timer

        public StatName StatName;
	    public StatModType StatModType;
        public float value;

        public override void TriggerEffect(LivingEntity target) {
            Stat targetStat = target.GetStat<Stat>(StatName);

            if (targetStat is DepletableStat depletableStat)
                depletableStat.AddValue(StatModType, value);
            else
                targetStat.AddModifier(new StatModifier(value, StatModType, this));

            if (StatName == StatName.HP)
                if (target.OnHealthChange != null)
                    target.OnHealthChange();
        }

        public void RemoveEffect(LivingEntity target) {
            Stat targetStat = target.GetStat<Stat>(StatName);

            if (targetStat is DepletableStat)
                Debug.LogWarning("Can not remove a instant effect");
            else
                targetStat.RemoveAllModifiersFromSource(this);
        }
    }
}

using UnityEngine;

[CreateAssetMenu(fileName = "New Offensive Skill", menuName = "Entity/Skill/Offensive Skill")]
public class DamageSkill : Skill {

    public override SkillEffect[] SkillEffects { get { return _DamageEffects; } }
	[SerializeField] private DamageEffect[] _DamageEffects;

    public override float Activate(LivingEntity[] userTeam, LivingEntity[] enemyTeam) {
        return base.Activate(userTeam, enemyTeam);
    }

    [System.Serializable]
    public class DamageEffect : SkillEffect { // very similar to the ItemBonus class lol
	    
        public float value;
        public bool valueScalesOffBase; // not implemented yet

        public override void TriggerEffect(LivingEntity target) {
            target.TakeDamage(value, false);
        }
    }

}

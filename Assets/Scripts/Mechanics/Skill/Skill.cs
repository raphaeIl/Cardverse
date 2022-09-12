using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Skill", menuName = "Entity/Skill/Generic Skill")]
public class Skill : ScriptableObject {

    public string Name;
    public string Description;
    public Sprite DisplayIcon;
    // unlockable
    public SkillType SkillType;
    public SkillEffectType SkillEffectType;
    // mana/stamina cost
    [ConditionalField("_isMutiTarget")] public int MaxTargets;
    [ConditionalField("_isAOE")] public float EffectRadius;

	[HideInInspector] public bool _isMutiTarget;
	[HideInInspector] public bool _isAOE;

    public int CooldownTime; // measured in turns unless theres a special case where a skill can be used outside of combat
    public int ActiveDuration; // also measured in turns

    public ActivationLocation ActivationLocation;

    public ParticleSystem SkillAnimation;
    public float AnimationDuration; // particleSystem's duration is not accurate since some effects can have sub effects with different lifetime/duration
    public float EffectDelay; // since some effects need to be triggers during the animation, a delay is needed

    public virtual SkillEffect[] SkillEffects { get; set; }

    private SkillState skillState;
    private int cooldownTimer;
    private int activeTimer;

    protected List<LivingEntity> AffectedTargets = new List<LivingEntity>();

    public void InitSkill() { // todo: Instantiate(scriptableObject) to create a new one instead of refrencing the original template
        skillState = SkillState.Ready;
        cooldownTimer = 0;
        activeTimer = 0;

        GameMaster.Instance.StartCoroutine(TryInitBattleEvent());

        IEnumerator TryInitBattleEvent() {
        while (BattleManager.Instance == null)
            yield return null;

        BattleManager.Instance.OnNextTurn += (isPlayerTurn) => {
            if (!isPlayerTurn)
                return;

            switch (skillState) {
                case SkillState.Ready:
                    break;

                case SkillState.Active:
                    activeTimer--;

                    if (activeTimer <= 0) {
                        activeTimer = 0;
                        Deactivate();
                        skillState = SkillState.Cooldown;
                        cooldownTimer = CooldownTime;
                    }
                    break;

                case SkillState.Cooldown:
                    cooldownTimer--;

                    if (cooldownTimer <= 0) {
                        cooldownTimer = 0;
                        skillState = SkillState.Ready;
                    }
                    break;
            }
        };
    }
    }

    void OnValidate() {
        _isAOE = SkillEffectType == SkillEffectType.AOE;
        _isMutiTarget = SkillEffectType == SkillEffectType.Muti_target;
    }

    public virtual float Activate(LivingEntity[] userTeam, LivingEntity[] enemyTeam) { // [0] for both teams are the primary user/target, with the rest being in the default party/enemytriggerteam order
        if (skillState == SkillState.Ready) {
            activeTimer = ActiveDuration;
            skillState = SkillState.Active;
        } else {
            Toast.DisplayError("Skill is already active or in cooldown");
            return 0;
        }

        LivingEntity[] primaryTargets; // entity at [0] is primary user/target, others are kinda random atm

        if (ActivationLocation == ActivationLocation.Self)
            primaryTargets = userTeam;
        else if (ActivationLocation == ActivationLocation.Enemy)
            primaryTargets = enemyTeam;
        else
            throw new System.NotImplementedException("skills that activates on both self and enemy are not implemented yet");

        Destroy(Instantiate(SkillAnimation, primaryTargets[0].WorldInfo.position, SkillAnimation.transform.rotation).gameObject, AnimationDuration);

        userTeam[0].StartCoroutine(TriggerEffectsWithDelay());

        IEnumerator TriggerEffectsWithDelay() {
            yield return new WaitForSeconds(EffectDelay);

            foreach (SkillEffect skillEffect in SkillEffects) {
                switch(SkillEffectType) {
                    case SkillEffectType.Single_Target:
                        skillEffect.TriggerEffect(primaryTargets[0]);
                        AffectedTargets.Add(primaryTargets[0]);
                        break;

                    case SkillEffectType.Muti_target:
                        for (int i = 0; i < MaxTargets && i < primaryTargets.Length; i++) {
                            skillEffect.TriggerEffect(primaryTargets[i]);
                            AffectedTargets.Add(primaryTargets[i]);
                        }
                        break;

                    case SkillEffectType.AOE:
                        RaycastHit2D[] entitiesInRadius = Physics2D.CircleCastAll(enemyTeam[0].transform.position, EffectRadius, Vector2.right);

                        foreach (RaycastHit2D raycastHit in entitiesInRadius) {
                            LivingEntity targetHit = raycastHit.transform.GetComponent<LivingEntity>();
                            if (ActivationLocation == ActivationLocation.Self)
                                targetHit = targetHit as Character;
                            else if (ActivationLocation == ActivationLocation.Enemy)
                                targetHit = targetHit as Enemy;

                            if (targetHit != null) {
                                skillEffect.TriggerEffect(targetHit);
                                AffectedTargets.Add(targetHit);
                            }
                        }
                        break;
                }
            }
        }
        
        return EffectDelay;
    }

    public virtual void Deactivate() {

    }

    [System.Serializable]
    public abstract class SkillEffect {
        public abstract void TriggerEffect(LivingEntity target);
    }
}

public enum SkillType {
    Normal,
    Elemental,
    Ultimate,
    Special
}

public enum SkillEffectType {
    Single_Target,
    Muti_target,
    AOE
}

public enum SkillState {
    Ready,
    Active,
    Cooldown
}

public enum ActivationLocation {
    Self,
    Enemy,
    Both
}

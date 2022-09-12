using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class BattleableEntity : LivingInteractableEntity, ICanAttack {

    public SpriteRenderer selectionCircle;

    public float stoppingDistance;

    protected virtual float Physical_Damage { get { return base.GetStat<Stat>(StatName.Attack).MaxValue; } }
    protected virtual float Magic_Damage { get { return 0; } }

    protected virtual float Get_Outgoing_Damage(bool isPhysical) { return isPhysical ? Physical_Damage : Magic_Damage; }
    protected virtual float Get_Incoming_Damage(float rawDamage, bool isPhysical) { return rawDamage; }

    public bool IsMouseOver { get; set; }
    public event System.Action<LivingEntity> OnMouseClickEvent;

    private Rigidbody2D rb2d;
    private bool isAttacking;

    protected override void OnValidate() {
        base.OnValidate();
    }

    protected override void Awake() {
        base.Awake();

        rb2d = GetComponent<Rigidbody2D>();
    }

    protected override void Update() {
        base.Update();

        if (IsMouseOver && Input.GetMouseButtonDown(0))
            OnMouseClick();
    }

    void OnMouseEnter() {
        IsMouseOver = true;
    }

    void OnMouseExit() {
        IsMouseOver = false;
    }

    public override LivingStatus TakeDamage(float rawAmount, bool isPhysical) {

        float incoming_damage = Get_Incoming_Damage(rawAmount, isPhysical); // real incoming damage calculations will be calculated here

        LivingStatus livingStatus = base.TakeDamage(incoming_damage, isPhysical);
        // is critical
        DamagePopUp.Create(WorldInfo.position, (int)incoming_damage, 1.5f, isPhysical ? Color.yellow : Color.cyan, false);

        return livingStatus;
    }

    public void OnMouseClick() {
        if (this is Enemy)
            if (OnMouseClickEvent != null)
                OnMouseClickEvent(this);
    }

    public void Attack(BattleableEntity victim, AttackType attackType, Action onAttackFinish) {
        if (isAttacking)
            return;

        StartCoroutine(AttackMovement(victim, attackType, onAttackFinish, 0.5f));
    }

    IEnumerator AttackMovement(BattleableEntity victim,  AttackType attackType, Action onAttackFinish, float attackDuration) {
        isAttacking = true;

        yield return new WaitForSeconds(1.0f);
        Vector3 originalPosition = WorldInfo.position;

        Vector3 moveDir = (victim.transform.position - WorldInfo.position).normalized;
        yield return GameMaster.Instance.Utils.MoveGameObjectTo(WorldInfo, WorldInfo.position + moveDir, 0.2f);

        yield return new WaitForSeconds(1.0f);

        switch (attackType) {
            case AttackType.Physical:
                ParticleSystem attackAnimation = ((this is Character) ? GameMaster.Instance.GameAssets.playerNormalAttackAnimation : GameMaster.Instance.GameAssets.enemyNormalAttackAnimation).GetComponentInChildren<ParticleSystem>();
                moveDir.y = 0;
                Destroy(Instantiate(attackAnimation, victim.WorldInfo.position - moveDir * 1.3f, attackAnimation.transform.rotation), attackAnimation.main.duration);

                GetStat<DepletableStat>(StatName.Stamina).RemoveValue(StatModType.Flat, LivingEntityInfo.Physical_Attack_Stamina_Cost); // check if not enough

                yield return new WaitForSeconds(0.2f);

                victim.TakeDamage(Get_Outgoing_Damage(attackType == AttackType.Physical), attackType == AttackType.Physical);
                break;

            case AttackType.Magic:
                GameObject attackAnimations = Instantiate(GameMaster.Instance.GameAssets.playerMagicAttackAnimation, victim.WorldInfo.position, GameMaster.Instance.GameAssets.playerMagicAttackAnimation.transform.rotation);

                ParticleSystem child1 = attackAnimations.transform.GetChild(0).GetComponent<ParticleSystem>();
                child1.Play();

                yield return new WaitForSeconds(1f);

                ParticleSystem child2 = attackAnimations.transform.GetChild(1).GetComponent<ParticleSystem>();
                child2.Play();

                yield return new WaitForSeconds(0.8f);

                Destroy(attackAnimations);

                GetStat<DepletableStat>(StatName.Mana).RemoveValue(StatModType.Flat, LivingEntityInfo.Magic_Attack_Mana_Cost);

                victim.TakeDamage(Get_Outgoing_Damage(attackType == AttackType.Magic), attackType == AttackType.Magic);
                break;

            case AttackType.Elemental:
                
                Character attacker = this as Character;
                Skill elementalSkill;

                if (attacker != null)
                    elementalSkill = attacker.GetSkill(SkillType.Elemental);
                else
                    throw new InvalidOperationException("Enemies can not have elmental skills"); // for now along with everything that uses this

                LivingEntity[] orderedAttackingTeam = new LivingEntity[] { this }.Concat(BattleManager.Instance.BattlingPlayerTeam.Where(player => player != this && !player.IsDead)).ToArray();
                LivingEntity[] orderedVictimTeam = new LivingEntity[] { victim }.Concat(BattleManager.Instance.BattlingEnemyTeam.Where(enemy => enemy != victim && !victim.IsDead)).ToArray();
                

                yield return new WaitForSeconds(elementalSkill.Activate(orderedAttackingTeam, orderedVictimTeam));

                break;
        }

        // GetComponent<CharacterController2D>().PlayAttackAnimation(); player attack animation goes here
        
        Flash.Create(victim.WorldInfo.position, 0.3f, 200, Color.white);
        CameraShaker.Instance.Shake(0.3f, 0.05f);

        GameMaster.Instance.Utils.MoveGameObjectTo(WorldInfo, originalPosition, 0.2f);
        yield return new WaitForSeconds(1f);

        onAttackFinish();
        isAttacking = false;
    }

    public void ConstrainAllMovement() {
        if (rb2d == null)
            rb2d = WorldInfo.GetComponent<Rigidbody2D>();

       rb2d.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    public void UnConstrainAllMovement() {
        if (rb2d == null)
            rb2d = WorldInfo.GetComponent<Rigidbody2D>();
        
        rb2d.constraints = RigidbodyConstraints2D.None;
        rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    public void ShowSelectionCircle() {
        if (selectionCircle == null && this is Character)
            PartyManager.Instance.PartyController.GetCorrespondingPartyMember((Character)this).ShowSelectionCircle();
        else
            selectionCircle.gameObject.SetActive(true);
    }

    public void HideSelectionCircle() {
        if (selectionCircle == null && this is Character)
            PartyManager.Instance.PartyController.GetCorrespondingPartyMember((Character)this).HideSelectionCircle();
        else
            selectionCircle.gameObject.SetActive(false);
    }
}

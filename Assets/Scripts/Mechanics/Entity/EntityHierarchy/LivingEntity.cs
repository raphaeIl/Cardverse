using System.Collections;
using UnityEngine;
using System.Linq;

public class LivingEntity : MonoBehaviour, IDamageable {

    public new string name;
    public Level level;

    public bool IsDead { get { return this.GetStat<DepletableStat>(StatName.HP).CurrentValue <= 0; } }

    public virtual System.Func<float, float> levelScalingFunction { get { return null; } }

    public System.Action OnHealthChange;
    public System.Action<ItemBonus.ModifiedStatData[]> OnStatChange; // all stats changed, previous value, current value, a dictionary can be used as an alterative

    private LivingEntityInfo livingEntityInfo; // instead of permentally hiding this, an Editor script could be made to hide this if a subclass of this is created and show the subclass's variable instead
    //protected LivingEntityInfo EntityInfo; // todo: rework entity info

    public virtual Transform WorldInfo { get { return transform; } } // transform

    private Stat[] stats;

    protected virtual void OnValidate() {

    }

    protected virtual void Awake() {
        level = new Level(levelScalingFunction);
    }

    protected virtual void Start() {
        OnStatChange += UpdateStats;

        GameMaster.Instance.Utils.InvokeLater(() => {
            foreach (Stat stat in GetStats())
                level.OnLevelChanged += stat.SetBaseLevel;
        }, 0.1f);
    }

    protected virtual void Update() {
        foreach (DepletableStat depletableStat in GetDepletableStats())
            depletableStat.Replenish();
    }

    public virtual T GetEntityInfo<T>() where T : LivingEntityInfo {
        return (T)livingEntityInfo;
    }

    public virtual void SetEntityInfo(LivingEntityInfo livingEntityInfo) {
        this.livingEntityInfo = (InteractableEntityInfo)livingEntityInfo;
    }

    public T GetStat<T>(StatName statName, ElementType elementType = ElementType.None, ElementalStatType elementalStatType = ElementalStatType.Unknown) where T : Stat {
        Stat statMatch;

        if (typeof(T) == typeof(ElementalStat))
            statMatch = stats.Where(stat => stat is ElementalStat elementalStat && elementalStat.ElementType == elementType && elementalStat.ElementalStatType == elementalStatType).FirstOrDefault() ?? ElementalStat.DEFAULT;
        else
            statMatch = stats.Where(stat => stat.StatName == statName).FirstOrDefault();
        
        return (T)statMatch;
    }

    public Stat[] GetStats() {
        return stats.Where(stat => !(stat is ElementalStat)).ToArray(); // most references of this probably doesn't need the passive, elemental stat
    }

    public DepletableStat[] GetDepletableStats() {
        return stats.Where(stat => stat is DepletableStat).Select(stat => (DepletableStat)stat).ToArray();
    }

    public void MoveTo(Vector2 location, float duration = 1f) {
        StartCoroutine(MoveAnimation(location, duration));
    }

    public virtual LivingStatus TakeDamage(float amount, bool isPhysical) {
        this.GetStat<DepletableStat>(StatName.HP).RemoveValue(StatModType.Flat, amount);

        print($"{name} takes {amount} damage!");

        if (OnHealthChange != null)
            OnHealthChange();

        if (this.GetStat<DepletableStat>(StatName.HP).CurrentValue <= 0) {
            Die();
            return LivingStatus.Dead;
        } else
            return LivingStatus.Alive;

    }

    public virtual void Die() {
        //if (!IsDead)
        //    return;

        Destroy(gameObject);
    }

    protected IEnumerator MoveAnimation(Vector2 location, float duration = 0.1f) {
        Vector2 originalLocation = transform.position;

        float speed = 1 / duration;
        float percent = 0;

        while (percent < 1) {
            
            percent += Time.deltaTime * speed;
            transform.position = Vector2.Lerp(originalLocation, location, percent);

            yield return null;
        }

    }

    public void SetUpStats() {
        LivingEntityInfo livingEntityInfo = null;

        if (this is Character)
            livingEntityInfo = GetEntityInfo<CharacterInfo>();
        else if (this is Enemy)
            livingEntityInfo = GetEntityInfo<EnemyInfo>();
        else
            GetEntityInfo<LivingEntityInfo>();

        if (livingEntityInfo == null) {
            print("SetUpStats:: livingEntityInfo is null");
            return;
        }

        stats = new Stat[livingEntityInfo.PossibleStats.Count + livingEntityInfo.PassiveStats.Count];

        int i = 0;
        for (; i < livingEntityInfo.PossibleStats.Count; i++) {
            if (livingEntityInfo.PossibleStats[i].IsDepletable)
                stats[i] = new DepletableStat(livingEntityInfo.PossibleStats[i].BaseValue, livingEntityInfo.PossibleStats[i].StatName, livingEntityInfo.PossibleStats[i].StatType, livingEntityInfo.PossibleStats[i].BaseValueScalingType, livingEntityInfo.PossibleStats[i].ReplenishRate);
            else
                stats[i] = new Stat(livingEntityInfo.PossibleStats[i].BaseValue, livingEntityInfo.PossibleStats[i].StatName, livingEntityInfo.PossibleStats[i].StatType, livingEntityInfo.PossibleStats[i].BaseValueScalingType);
        }

        for (int j = 0; j < livingEntityInfo.PassiveStats.Count; j++)
            stats[i++] = new ElementalStat(livingEntityInfo.PassiveStats[j].BaseValue, livingEntityInfo.PassiveStats[j].StatName, livingEntityInfo.PassiveStats[j].StatType, livingEntityInfo.PassiveStats[j].BaseValueScalingType, livingEntityInfo.PassiveStats[j].ElementType, livingEntityInfo.PassiveStats[j].ElementalStatType);
        
    }

    private void UpdateStats(ItemBonus.ModifiedStatData[] modifiedStatData) {
        print(name);
        print(Utils.ToString(GetStats()));
    }
}

public enum LivingStatus {
    Alive,
    Dead,
}

public enum ElementType {
    None,
    Earth,
    Water,
    Ice,
    Lightning,
    Fire
}
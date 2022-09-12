using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : BattleableEntity {

    private EnemyInfo enemyInfo;

    public override void SetEntityInfo(LivingEntityInfo livingEntityInfo) {
        enemyInfo = (EnemyInfo)livingEntityInfo;

        base.SetUpStats();
    }

    public override T GetEntityInfo<T>() {
        return (T)(LivingEntityInfo)enemyInfo;
    }

}

public enum EnemyArchetype {
    Common,
    Elite,
    Boss,
    Special
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface ICanAttack { // this name is so bad lmao
                                // parameter can also be ICanAttack, this wouldn't make sense in a normal game but in this game (all turn-based) you can only damage entities by turned based combat
    public void Attack(BattleableEntity victim, AttackType attackType, Action onAttackComplete);

}


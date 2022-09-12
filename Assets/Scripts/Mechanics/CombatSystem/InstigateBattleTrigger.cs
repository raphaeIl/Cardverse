using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstigateBattleTrigger : MonoBehaviour {

    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private EnemyInfo[] enemiesToSummon;

    private bool hasTriggered;

    void OnTriggerEnter2D(Collider2D collision) {
        if (hasTriggered)
            return;

        PartyMember player = collision.transform.GetComponent<PartyMember>();

        if (player != null && !BattleManager.Instance.InBattle) {

            List<Enemy> enemiesSummon = new List<Enemy>();

            foreach (EnemyInfo enemyInfo in enemiesToSummon) {
                Enemy enemySummon = Instantiate(enemyPrefab, transform).GetComponent<Enemy>();
                enemySummon.SetEntityInfo(Instantiate(enemyInfo));

                enemiesSummon.Add(enemySummon);
            }

            BattleManager.Instance.StartBattle(enemiesSummon.ToArray());
            hasTriggered = true;
        }
    }

}

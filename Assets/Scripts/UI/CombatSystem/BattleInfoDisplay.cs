using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleInfoDisplay : MonoBehaviour {

    public Text entityName;
    public BattleStatDisplay[] statDisplays;

    public float paddingOffset;

    void Start() {
        CustomPaddingOffset();
    }

    public void UpdateBattleInfoDisplay(BattleableEntity entityToDisplay) { // should be BattleableEntity[]
        entityName.text = entityToDisplay.GetEntityInfo<LivingEntityInfo>().Name;

        GetComponent<CanvasGroup>().alpha = entityToDisplay.IsDead ? 0.3f : 1f;

        statDisplays[0].UpdateStatDisplay(entityToDisplay.GetStat<DepletableStat>(StatName.HP)); // manutally setting this here because the battlehud will only display health, mana and stamina
        statDisplays[1].UpdateStatDisplay(entityToDisplay.GetStat<DepletableStat>(StatName.Mana));
        statDisplays[2].UpdateStatDisplay(entityToDisplay.GetStat<DepletableStat>(StatName.Stamina));
    }

    private void CustomPaddingOffset() {
        for (int i = 0; i < statDisplays.Length; i++) {
            RectTransform rectTransform = statDisplays[i].GetComponent<RectTransform>();

            rectTransform.localPosition = new Vector2(paddingOffset * i, rectTransform.localPosition.y);

        }
    }
}

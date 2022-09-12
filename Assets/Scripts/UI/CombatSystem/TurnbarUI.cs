using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnbarUI : MonoBehaviour {

    public int turnLimit;

    public Color playerBarColor;
    public Color playerBarTextColor;
    public Color enemyBarColor;
    public Color enemyBarTextColor;

    public TextMeshProUGUI currentTurnDisplay;
    public TextMeshProUGUI nextTurnDisplay;

    public Image turnBar;

    private Animator animator;

    private bool isStarted;
    private float timeCounter;

    // Start is called before the first frame update
    void Awake() {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update() {
        
        if (isStarted) {
            if (timeCounter >= turnLimit) {
                isStarted = false;
                BattleManager.Instance.NextTurn();
                return;
            }

            timeCounter += Time.deltaTime;
            turnBar.fillAmount = timeCounter / turnLimit;
        }

    }

    public void StartTurnbar(bool isPlayer) {
        timeCounter = 0;

        turnBar.color = isPlayer ? playerBarColor : enemyBarColor;
        nextTurnDisplay.color = isPlayer ? playerBarTextColor : enemyBarTextColor;
        nextTurnDisplay.SetText(isPlayer ? "Player Turn" : "Enemy Turn");

        animator.SetTrigger("turnbar");

        isStarted = true;

    }
}

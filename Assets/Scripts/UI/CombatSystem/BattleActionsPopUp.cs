using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleActionsPopUp : MonoBehaviour {

    #region Singleton

    public static BattleActionsPopUp Instance;

    void Awake() {

        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

        animator = transform.GetChild(0).GetComponent<Animator>();
    }

    #endregion

    [HideInInspector] public bool isOverPopUp;

    private bool isShowing;

    private Animator animator;

    void Update() {
        
        if (!BattleManager.Instance.InBattle || BattleManager.Instance.battleState != BattleState.Idle)
            return;
        
        isOverPopUp = RectTransformUtility.RectangleContainsScreenPoint(transform.GetChild(0).GetComponent<RectTransform>(), Input.mousePosition);

        if (isOverPopUp || BattleManager.Instance.MouseOverPartyMember()) {
            Show();
            RectTransform rt = GetComponent<RectTransform>();

            if (!isOverPopUp)
                rt.position = Input.mousePosition + Vector3.left * 5f;

        } else if (!isOverPopUp && !BattleManager.Instance.MouseOverPartyMember())
            Hide();

    }

    public void Show() {
        if (!isShowing) {
            isShowing = true;
            animator.SetBool("isVisible", isShowing);
        }
    }

    public void Hide() {
        if (isShowing) {
            isShowing = false;
            animator.SetBool("isVisible", isShowing);
        }
    }
}

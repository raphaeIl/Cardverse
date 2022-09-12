using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour {

    [HideInInspector] public BattleableEntity[] playerTeam;
    [HideInInspector] public BattleableEntity[] enemyTeam;

    public GameObject battleIntroUI;
    public Text turnStatus;

    [SerializeField] public Transform playerTeamDisplayParent;
    [SerializeField] public Transform enemyTeamDisplayParent;
    [SerializeField] public BattleInfoDisplay entityDisplayPrefab;

    public void StartBattle(BattleableEntity[] playerTeam, BattleableEntity[] enemyTeam) {
        gameObject.SetActive(true);
        this.playerTeam = playerTeam;
        this.enemyTeam = enemyTeam;

        InitBattleHUD();
    } 

    public void InitBattleHUD() {
        if (playerTeam == null || enemyTeam == null)
            throw new System.ArgumentNullException("hud not set up or battle not started");

        foreach (BattleableEntity battleableEntity in playerTeam) {
            BattleInfoDisplay playerInfoDisplay = Instantiate(entityDisplayPrefab, playerTeamDisplayParent.transform);
            playerInfoDisplay.UpdateBattleInfoDisplay(battleableEntity);
            playerInfoDisplay.paddingOffset = -3.0f;
        }

        foreach (BattleableEntity battleableEntity in enemyTeam) {
            BattleInfoDisplay enemyInfoDisplay = Instantiate(entityDisplayPrefab, enemyTeamDisplayParent.transform);
            enemyInfoDisplay.UpdateBattleInfoDisplay(battleableEntity);
            enemyInfoDisplay.paddingOffset = 3.0f;
        }   
    }

    public void UpdateBattleHUD() {
        for (int i = 0; i < playerTeamDisplayParent.childCount; i++) {
            playerTeamDisplayParent.transform.GetChild(i).GetComponent<BattleInfoDisplay>().UpdateBattleInfoDisplay(playerTeam[i]);
        }

        for (int i = 0; i < enemyTeamDisplayParent.childCount; i++) {
            enemyTeamDisplayParent.transform.GetChild(i).GetComponent<BattleInfoDisplay>().UpdateBattleInfoDisplay(enemyTeam[i]);
        }
    }

    public void UpdateTurnStatus(bool isPlayerTurn) {
        turnStatus.text = isPlayerTurn ? "Player's Turn" : "Enemy's Turn";
    }

    public void EndBattle() {
        gameObject.SetActive(false);
        this.playerTeam = null;
        this.enemyTeam = null;
    }

    public void PlayBattleMusic() {
        GameMaster.Instance.AudioManager.StopAllMusic();
        GameMaster.Instance.AudioManager.Play2DMusic(AudioManager.MusicType.Battle_Theme); 
    }

    public void PlayRandomEncounterSFX() {
        GameMaster.Instance.AudioManager.Play2DSFX(AudioManager.SFXType.Random_Encounter);
    }

}

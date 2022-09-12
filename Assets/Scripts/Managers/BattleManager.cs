using System;
using System.Collections;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class BattleManager : MonoBehaviour { // if anyone wants to clean this spaghetti code please go ahead ty ;)

    #region Singleton

    public static BattleManager Instance;

    void Awake() {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    #endregion

    public bool InBattle { get; private set; }

    public bool IsBattleOver { get {

            if (playerTeam == null || enemyTeam == null)
                throw new InvalidOperationException("Battle not started");

            bool allEnemyDead = true;

            for (int i = 0; i < enemyTeam.Count; i++) {
                if (!enemyTeam[i].IsDead) 
                    allEnemyDead = false;
            }

            bool allPlayerDead = true;

            for (int i = 0; i < playerTeam.Count; i++) {
                if (!playerTeam[i].IsDead)
                    allPlayerDead = false;
            }

            if (allPlayerDead) {
                print("All Players Dead, Enemy wins!");
                return true;
            } else if (allEnemyDead) {
                print("All Enemies Dead, Player wins!");
                return true;
            } else
                return false;
    } }

    public Transform playerBattlePosition;
    public Transform enemyBattlePosition;

    public BattleHUD battleHUD;
    public GameObject battleWonUI;
    public TurnbarUI turnbar;

    public event Action<bool> OnNextTurn; // parameter: who's turn is it? true if player's turn false if enemy's

    public List<Character> BattlingPlayerTeam { get { return playerTeam; } }
    public List<Enemy> BattlingEnemyTeam { get { return enemyTeam; } }

    private CameraController camController;
    private CharacterController2D playerController;

    private int playerTeamCount { get { return playerTeam.Where(p => !p.IsDead).Count(); } }
    private int enemyTeamCount { get { return enemyTeam.Where(e => !e.IsDead).Count(); } }

    private List<Character> playerTeam;
    private List<Enemy> enemyTeam;

    private Character selectedPlayer;
    private Enemy selectedEnemy;

    private int attackingPlayer;
    private int attackingEnemy;

    public BattleState battleState;
    private TurnState turnState;

    private Vector2 playerOriginalLocation;

    void Start() {
        camController = Camera.main.GetComponent<CameraController>();
    }

    public void StartBattle(BattleableEntity[] enemyTeam) {
        this.playerTeam = PartyManager.Instance.Party.ActiveParty.Where(member => member != null).ToList();
        this.enemyTeam = enemyTeam.Select(enemy => (Enemy)enemy).ToList();
        
        print($"Battle Started: PlayerTeam ({this.playerTeam.Count}) vs EnemyTeam ({this.enemyTeam.Count})");

        InBattle = true;
        battleState = BattleState.Busy;
        
        GameMaster.Instance.AudioManager.Play2DSFX(AudioManager.SFXType.Battle_Entrance);
        GameMaster.Instance.SceneTransitioner.BattleEntranceTransition();

        // pre battle set up
        turnState = TurnState.PlayerTurn;
        battleHUD.UpdateTurnStatus(true);

        playerController = PartyManager.Instance.PartyController.GetComponentInChildren<CharacterController2D>();

        this.playerTeam.ForEach(player => player.OnHealthChange += this.battleHUD.UpdateBattleHUD);
        this.enemyTeam.ForEach(enemy => enemy.OnHealthChange += this.battleHUD.UpdateBattleHUD);

        this.playerTeam.ForEach(player => player.OnMouseClickEvent += (livingEntity) => { SelectCharacter((Character)livingEntity); });
        this.enemyTeam.ForEach(enemy => enemy.OnMouseClickEvent += (livingEntity) => { SelectEnemy((Enemy)livingEntity); });

        StartCoroutine(SetupBattleEntrancePosition());

        camController.RemoveFocus();
        playerController.DisableAllControls();
        PartyManager.Instance.PartyController.AutoFollow(false);
        InventoryInteractableHandler.Instance.HideAllVisiblePanels();

        this.playerTeam.ForEach(player => player.ConstrainAllMovement());
        this.enemyTeam.ForEach(enemy => enemy.ConstrainAllMovement());
    }

    // setting up players/enemies battle entrance positions 
    public IEnumerator SetupBattleEntrancePosition() {

        playerOriginalLocation = PartyManager.Instance.PartyController.PartyLeader.transform.position;

        for (int i = 0; i < playerTeam.Count; i++)
            GameMaster.Instance.Utils.MoveGameObjectTo(playerTeam[i].WorldInfo, (playerBattlePosition.position + Vector3.up * playerTeam.Count / 2) + Vector3.down * i, 0.5f);

        for (int i = 0; i < enemyTeam.Count; i++)
            GameMaster.Instance.Utils.MoveGameObjectTo(enemyTeam[i].transform, (enemyBattlePosition.position + Vector3.up * enemyTeam.Count / 2) + Vector3.down * i, 0.5f);

        yield return new WaitForSeconds(0.5f);

        battleHUD.StartBattle(playerTeam.ToArray(), enemyTeam.ToArray());


        battleState = BattleState.Idle;
        turnbar.StartTurnbar(true);

        SelectCharacter(playerTeam[attackingEnemy = 0]);
        SelectEnemy(this.enemyTeam[attackingPlayer = 0]);
    }

    public void Battle(BattleableEntity attacker, BattleableEntity victim, AttackType attackType) {
         if (battleState == BattleState.Idle) { // checking battle state should be an outer if statement
             battleState = BattleState.Busy;

            BattleActionsPopUp.Instance.Hide();
        }

        print($"Battle Progress: {attacker.name} attacks {victim.name}!!");

        attacker.Attack(victim, attackType, () => { NextTurn(); });
    }

    public void NextTurn() { // swaps to next turn

        if (IsBattleOver) {
            OnBattleOver();
            return;
        }

        switch (turnState) {
            case TurnState.PlayerTurn:
                turnState = TurnState.EnemyTurn;
                battleState = BattleState.Busy;

                battleHUD.UpdateTurnStatus(false);
                SelectEnemy(enemyTeam.Where(p => !p.IsDead).ToArray()[(++attackingEnemy) % enemyTeamCount]); // enemy attacking
                camController.SetFocus(this.selectedEnemy.WorldInfo, false, 4f);
                Battle(selectedEnemy, playerTeam.Where(p => !p.IsDead).ToArray()[UnityEngine.Random.Range(0, playerTeamCount)], AttackType.Physical);
                break;

            case TurnState.EnemyTurn:
                turnState = TurnState.PlayerTurn;
                battleState = BattleState.Idle;

                SelectCharacter(playerTeam.Where(p => !p.IsDead).ToArray()[(++attackingPlayer) % playerTeamCount]); // player attacking
                SelectEnemy(enemyTeam.Where(e => !e.IsDead).ToArray()[0]); // player selected enemy to attack, defaultly set to the first non-dead enemy
                battleHUD.UpdateTurnStatus(true);
                break;
        }

        if (OnNextTurn != null)
            OnNextTurn(turnState == TurnState.PlayerTurn);

        turnbar.StartTurnbar(turnState == TurnState.PlayerTurn);
    }

    public void SelectCharacter(Character selectedCharacter) {
        if (selectedPlayer != null && selectedPlayer != selectedCharacter) {// hide previously selected player's circle
            GameMaster.Instance.Utils.MoveGameObjectTo(selectedPlayer.WorldInfo, selectedPlayer.WorldInfo.position + Vector3.left * 0.5f);
            selectedPlayer.HideSelectionCircle();
        }

        if (selectedPlayer != selectedCharacter) {
            selectedPlayer = selectedCharacter;
            camController.SetFocus(selectedPlayer.WorldInfo, true);
            GameMaster.Instance.Utils.MoveGameObjectTo(selectedPlayer.WorldInfo, selectedPlayer.WorldInfo.position + Vector3.right * 0.5f);
        }

        selectedPlayer.ShowSelectionCircle();
    }
    
    public void SelectEnemy(Enemy selectedEnemy) {
        if (this.selectedEnemy != null)
            this.selectedEnemy.HideSelectionCircle();

        this.selectedEnemy = selectedEnemy;
        print($"{this.selectedEnemy.GetEntityInfo<LivingEntityInfo>().Name} selected");
        
        this.selectedEnemy.ShowSelectionCircle();
    } 
    
    public void OnBattleOver() { // reset everything
        
        turnState = TurnState.PlayerTurn;

        GameMaster.Instance.AudioManager.StopAllMusic();

        this.playerTeam.ForEach(player => { player.UnConstrainAllMovement(); player.HideSelectionCircle(); player.Revive(); });
        this.enemyTeam.ForEach(enemy => { if (!enemy.IsDead) enemy.Die(); });

        this.playerTeam.ForEach(player => player.OnHealthChange -= this.battleHUD.UpdateBattleHUD);
        this.enemyTeam.ForEach(enemy => enemy.OnHealthChange -= this.battleHUD.UpdateBattleHUD);

        this.playerTeam.ForEach(player => player.OnMouseClickEvent -= (livingEntity) => { SelectCharacter((Character)livingEntity); });
        this.enemyTeam.ForEach(enemy => enemy.OnMouseClickEvent -= (livingEntity) => { SelectEnemy((Enemy)livingEntity); });
        
        playerTeam = null;
        enemyTeam = null;

        battleHUD.EndBattle();

        battleState = BattleState.Idle;
        InBattle = false;
       
        battleWonUI.SetActive(true);
    }

    public void OnContinueButton() {
        GameMaster.Instance.SceneTransitioner.BattleEndTransition();
        battleWonUI.SetActive(false);

        camController.SetFocus(PartyManager.Instance.PartyController.PartyLeader.transform);
        playerController.EnableAllControls();

        PartyManager.Instance.PartyController.PartyLeader.transform.position = playerOriginalLocation;
        PartyManager.Instance.PartyController.AutoFollow(true);

        InventoryInteractableHandler.Instance.ShowAllVisiblePanels();
    }

    public void OnPhysicalAttack() {
        Battle(selectedPlayer, selectedEnemy, AttackType.Physical);
    }

    public void OnMagicAttack() {
        Battle(selectedPlayer, selectedEnemy, AttackType.Magic);
    }

    public void OnElementalAttack() {
        if (selectedPlayer.GetSkill(SkillType.Elemental) == null)
            return;

        Battle(selectedPlayer, selectedEnemy, AttackType.Elemental);
    }

    public bool MouseOverPartyMember() {
        foreach (Character character in playerTeam)
            if (character.IsMouseOver)
                return true;
        return false;
    }
}

public enum BattleState {
    Idle,
    Busy,
}

public enum TurnState {
    PlayerTurn,
    EnemyTurn,
}

public enum AttackType {
    Physical,
    Magic,
    Elemental,
}

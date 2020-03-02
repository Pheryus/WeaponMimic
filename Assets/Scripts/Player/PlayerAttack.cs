using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour {

    Player player;
    PlayerAnimation playerAnimation;
    public enum WeaponTypes { none, sword };

    public Weapon[] equippedWeapons;

    int actualAttackSequence = 0;

    ///True se o personagem está atacando no momento
    public bool onAttack = false;

    ///True se o personagem irá atacar em breve
    bool willAttack = false;

    public bool attackSucessful = false;

    

    //Diferença de frames entre os ataques. usado para o combo de ataques.
    int actualFramesFromLastAttack = 0;


    [Range(0, 30)]
    [Tooltip("Janela de frames que o jogador tem caso ataque antes de um ataque ter sido completado")]
    public int frameWindowAttackInput;

    int framesFromLastAttackInputRemaining, lastWeaponAttack = 0;

    private void Start() {
        player = GetComponent<Player>();
        playerAnimation = GetComponent<PlayerAnimation>();
    }

    public void AttackLeftHand() {
        Attack(0);
    }

    ///Chamada no frame de animação de criação de colisão
    public void CreateAttackCollider(){
        attackSucessful = true;
    }

    void Attack(int i) {
        
        //Save Attack for future
        if (!willAttack && onAttack) {
            Debug.Log("save attack");
            lastWeaponAttack = i;
            SaveAttackFrame();
            return;
        }

        if (onAttack)
            return;

        Debug.Log("atacou");

        if (actualFramesFromLastAttack <= equippedWeapons[i].attackPatterns[actualAttackSequence].frameWindow) {
            actualAttackSequence = (actualAttackSequence + 1) % equippedWeapons[i].attackPatterns.Count;
        }
        else {
            BreakCombo();
        }
        attackSucessful = false;
        onAttack = true;
        willAttack = false;
        playerAnimation.Attack(equippedWeapons[i].weaponName + "Attack" + (actualAttackSequence + 1).ToString());
        actualFramesFromLastAttack = 0;
        if (actualAttackSequence > equippedWeapons[i].attackPatterns.Count) actualAttackSequence = 0;
    
    }

    void SaveAttackFrame(){
        framesFromLastAttackInputRemaining = frameWindowAttackInput;
        willAttack = true;
    }

    public void RefreshFrameCombo() {
        if (onAttack && willAttack){
            SaveAttackFrame();
            Debug.Log("refreshFrameCombo");
            actualFramesFromLastAttack = 0;
        }
    }

    public void BreakCombo() {
        //Debug.Log("break combo: " + actualFramesFromLastAttack);
        actualFramesFromLastAttack = equippedWeapons[lastWeaponAttack].attackPatterns[actualAttackSequence].frameWindow + 1;
        actualAttackSequence = 0;
    }

    private void Update() {

        if (framesFromLastAttackInputRemaining > 0 && willAttack && !onAttack){
            Debug.Log("Execute saved attack");
            framesFromLastAttackInputRemaining = 0; 
            Attack(lastWeaponAttack);
        }        

        if (player.dashState == Player.DashState.none) framesFromLastAttackInputRemaining--;

        if (framesFromLastAttackInputRemaining < 0) framesFromLastAttackInputRemaining = 0;
        
        if (!onAttack && player.dashState == Player.DashState.none)
            actualFramesFromLastAttack++;
    }

    public void AttackRightHand() {
        Attack(1);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour {

    Player player;
    PlayerAnimation playerAnimation;
    public enum WeaponTypes { none, sword };

    public Weapon[] equippedWeapons;

    int actualAttackSequence = 0;

    public bool onAttack = false;

    int actualFramesFromLastAttack = 0;


    [Range(0, 30)]
    [Tooltip("Janela de frames que o jogador tem caso ataque antes de um ataque ter sido completado")]
    public int frameWindowAttackInput;

    int actualFrameFromLastAttackInput, lastWeaponAttack = 0;

    private void Start() {
        player = GetComponent<Player>();
        playerAnimation = GetComponent<PlayerAnimation>();
    }

    public void AttackLeftHand() {
        Attack(0);
    }

    void Attack(int i) {
        
        if (onAttack || player.dashState != Player.DashState.none) {
            Debug.Log("save attack");
            actualFrameFromLastAttackInput = frameWindowAttackInput;
            lastWeaponAttack = i;
            onAttack = true;
            return;
        }

        Debug.Log("atacou");

        if (actualFramesFromLastAttack <= equippedWeapons[i].attackPatterns[actualAttackSequence].frameWindow) {
            Debug.Log("combou");
            actualAttackSequence = (actualAttackSequence + 1) % equippedWeapons[i].attackPatterns.Count;
        }
        else {
            BreakCombo();
        }

        onAttack = true;
        playerAnimation.Attack(equippedWeapons[i].weaponName + "Attack" + (actualAttackSequence + 1).ToString());
        actualFramesFromLastAttack = 0;
        if (actualAttackSequence > equippedWeapons[i].attackPatterns.Count) actualAttackSequence = 0;
        

    }

    public void RefreshFrameCombo() {
        //Debug.Log("refreshFrameCombo");
        actualFramesFromLastAttack = 0;
        actualFrameFromLastAttackInput = frameWindowAttackInput;
    }

    public void BreakCombo() {
        //Debug.Log("break combo");
        actualFramesFromLastAttack = equippedWeapons[lastWeaponAttack].attackPatterns[actualAttackSequence].frameWindow + 1;
        actualAttackSequence = 0;
    }

    private void Update() {
        if (actualFrameFromLastAttackInput > 0 && (!onAttack || player.dashState == Player.DashState.none)) {
            //Debug.Log("aqui");
            actualFrameFromLastAttackInput = 0;
            Attack(lastWeaponAttack);
            
        }

        if (player.dashState == Player.DashState.none) actualFrameFromLastAttackInput--;

        if (actualFrameFromLastAttackInput < 0) actualFrameFromLastAttackInput = 0;
        

        if (!onAttack && player.dashState == Player.DashState.none)
            actualFramesFromLastAttack++;
    }

    public void AttackRightHand() {
        Attack(1);
    }
}

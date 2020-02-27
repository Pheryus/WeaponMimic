using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (Player))]
public class PlayerInput : MonoBehaviour {

	Player player;
    PlayerAttack playerAttack;

    public KeyCode attackLeftButton, attackRightButton;

    List<Vector2> playerInputs;

	void Start () {
        playerInputs = new List<Vector2>();
		player = GetComponent<Player> ();
        playerAttack = GetComponent <PlayerAttack>();

	}

	void Update () {
		Vector2 directionalInput = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
        playerInputs.Add(directionalInput);

		player.SetDirectionalInput(playerInputs);

		if (Input.GetKeyDown (KeyCode.Space)) {
			player.OnJumpInputDown ();
		}
		if (Input.GetKeyUp (KeyCode.Space)) {
			player.OnJumpInputUp ();
		}
        if (Input.GetKeyDown(KeyCode.LeftShift)) {
            player.OnDashInput();
        }
        if (Input.GetKeyDown(attackLeftButton)) playerAttack.AttackLeftHand();

        if (Input.GetKeyDown(attackRightButton)) playerAttack.AttackRightHand();

    }
}

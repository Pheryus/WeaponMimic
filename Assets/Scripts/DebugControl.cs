using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class DebugControl : MonoBehaviour {

    public Player player;

    public TextMeshProUGUI dash, jump, dashSpeed, dashFrameDuration;

    private void Update() {

  
        if (Input.GetKeyDown(KeyCode.Equals) || Input.GetKeyDown(KeyCode.KeypadPlus)) {
            player.framesToConstantDashDuration++;

        }
        if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus)) {
            player.framesToConstantDashDuration--;
        }

        if (Input.GetKeyDown(KeyCode.Alpha9) || Input.GetKeyDown(KeyCode.Keypad9)) {
            float prevTime = (player.framesToConstantDashDuration * player.maxDashSpeed) / 60;
           
            player.maxDashSpeed++;
            player.startDashSpeed = player.maxDashSpeed / 2;
            player.framesToConstantDashDuration = (int)((60 * prevTime) / player.maxDashSpeed);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.Keypad0)) {
            float prevTime = (player.framesToConstantDashDuration * player.maxDashSpeed) / 60;

            player.maxDashSpeed--;
            player.startDashSpeed = player.maxDashSpeed / 2;
            player.framesToConstantDashDuration = (int)((60 * prevTime) / player.maxDashSpeed);
        }

        if (Input.GetKey(KeyCode.O)) {
            player.maxJumpHeight += Time.deltaTime * 1;
            player.UpdatePhysics();
        }
        if (Input.GetKey(KeyCode.P)) {
            player.maxJumpHeight -= Time.deltaTime * 1;
            player.UpdatePhysics();
        }


        if (Input.GetKey(KeyCode.R)) {
            SceneManager.LoadScene("Scene");
        }
        else if (Input.GetKey(KeyCode.Escape))
            Application.Quit();

        dash.text = "Dash Player Units: " + ((player.framesToConstantDashDuration * player.maxDashSpeed) / 60).ToString();
        jump.text = "Jump Player Units: " + (player.maxJumpHeight / 1.5f).ToString();
        dashSpeed.text = "Dash Speed ~: " + (player.maxDashSpeed.ToString());

        dashFrameDuration.text = "Dash Frame Duration: " + player.framesToConstantDashDuration.ToString();
    }

}

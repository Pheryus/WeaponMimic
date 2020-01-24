using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class DebugControl : MonoBehaviour {

    public Player player;

    public TextMeshProUGUI dash, jump, startDashSpeed, framesConstantDash, framesAccelDash, framesDeccelDash, endDashSpeed;

    public GameObject debugMessages;

    private void Update() {
        if (Input.GetKey(KeyCode.R)) {
            SceneManager.LoadScene("Scene");
        }
        else if (Input.GetKey(KeyCode.Escape))
            Application.Quit();

        if (Input.GetKeyDown(KeyCode.Q)) {
            debugMessages.SetActive(!debugMessages.activeSelf);
        }

        if (!debugMessages.activeSelf)
            return;



        if (Input.GetKeyDown(KeyCode.Alpha1)) {
			player.framesToAccelDash++;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha2)) {
			player.framesToAccelDash--;
		}

		if (Input.GetKeyDown(KeyCode.Alpha3)) {
			player.framesToDeccelDash++;
		} else if (Input.GetKeyDown(KeyCode.Alpha4)) {
			player.framesToDeccelDash--;
		}


		if (Input.GetKeyDown(KeyCode.Alpha5)) {
            player.framesToConstantDashDuration++;
        }
        if (Input.GetKeyDown(KeyCode.Alpha6)) {
            player.framesToConstantDashDuration--;
        }


		if (Input.GetKeyDown(KeyCode.Alpha7)) {
			player.startDashSpeed++;
		}
		if (Input.GetKeyDown(KeyCode.Alpha8)) {
			player.startDashSpeed--;
		}

		if (Input.GetKeyDown(KeyCode.Alpha9)) {
			player.maxDashSpeed++;
		}
		if (Input.GetKeyDown(KeyCode.Alpha0)) {
			player.maxDashSpeed--;
		}

        if (Input.GetKey(KeyCode.O)) {
            player.maxJumpHeight += Time.deltaTime * 1;
            player.UpdatePhysics();
        }
        if (Input.GetKey(KeyCode.P)) {
            player.maxJumpHeight -= Time.deltaTime * 1;
            player.UpdatePhysics();
        }




        dash.text = "Dash Player Units: " + GetDashDistance().ToString();
        jump.text = "Jump Player Units: " + (player.maxJumpHeight / 1.5f).ToString();
        startDashSpeed.text = "Start Dash Speed: " + (player.startDashSpeed.ToString());
		endDashSpeed.text = "End Dash Speed: " + (player.maxDashSpeed.ToString());
		
		framesConstantDash.text = "Dash Frame Duration: " + player.framesToConstantDashDuration.ToString();
		framesAccelDash.text = "Frames To Accel Dash:" + player.framesToAccelDash.ToString();
		framesDeccelDash.text = "Frames To Deccel Dash:" + player.framesToDeccelDash.ToString();
	}

	float GetDashDistance() {
		float start = player.startDashSpeed;
		float end = player.maxDashSpeed;
		float t = player.framesToAccelDash;
		float accel = (end - start) / t;
        //Debug.Log("accel: " + accel);
		float dist1 = (end * end - start * start) / (2 * accel);
        //Debug.Log("dist1: " + dist1);
		float dist2 = end * player.framesToConstantDashDuration;

		start = player.maxDashSpeed;
		end = player.moveSpeed;
		accel = (end - start) / player.framesToDeccelDash;

        //Debug.Log("dist2: " + dist2);


        float dist3 = (end * end - start * start) / (2 * accel);


        //Debug.Log("dist3: " + dist3);   
        return (dist1 + dist2 + dist3) / 60;

	}

}

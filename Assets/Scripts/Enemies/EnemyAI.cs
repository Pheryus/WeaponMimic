using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour {

    public List<BaseEnemyAction> baseEnemyAttack;


    /// <summary>
    /// Tempo mínimo entre uma ação e outra
    /// </summary>
    public float actionFrequence;

    float actionFrequenceTimer = 0;

    private void Update() {
        if (actionFrequenceTimer == 0) {
            MakeAction();
        }
        else actionFrequenceTimer += Time.deltaTime;
    }

    private void MakeAction() {
        actionFrequenceTimer = actionFrequence;
        BaseEnemyAction bestAction = null;
        float heuristicValue = 0.1f;
        
        foreach (BaseEnemyAction be in baseEnemyAttack) {
             if (heuristicValue < be.GetHeuristicValue()) {
                heuristicValue = be.GetHeuristicValue();
                bestAction = be;
            }
        }

        if (bestAction == null) {
            MakeIdleAction();
        }
        else {
            bestAction.DoAction();
        }

    }

    private void MakeIdleAction() {

    }

}

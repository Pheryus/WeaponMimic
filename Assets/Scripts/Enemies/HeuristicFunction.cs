using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Heuristic Function", menuName = "New heuristic")]
public class HeuristicFunction : ScriptableObject {

    public float value;

    public float GetHeuristicValue() {
        return value;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class Condition {
	public enum ConditionType { range, health };
	public ConditionType condition;

	[MinMaxRange(0, 10)]
	public RangedFloat range;

}


#if UNITY_EDITOR
[CustomEditor(typeof(BaseEnemyAction))]
[CanEditMultipleObjects] 
public class HeuristicFunctionEditor : Editor {
	private BaseEnemyAction properties;


	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

		BaseEnemyAction heuristic = target as BaseEnemyAction;

		foreach (Condition c in heuristic.conditions) {
			if (c.condition == Condition.ConditionType.range) {
				//c.range. = EditorGUILayout.FloatField("I field:", myScript.i, 1, 100);
			}
		}

	}
}
#endif
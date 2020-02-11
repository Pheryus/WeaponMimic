using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class HeuristicFunction {

	public enum HeuristicType { range };

    public float value;

	public HeuristicType heuristicType;

    public float GetHeuristicValue() {
        return value;
    }

	[MinMaxRange(0, 10)]
	public RangedFloat range;

}

#if UNITY_EDITOR
[CustomEditor(typeof(HeuristicFunction))]
[CanEditMultipleObjects] 
public class HeuristicFunctionEditor : Editor {
	private HeuristicFunction properties;


	public override void OnInspectorGUI() {
		base.OnInspectorGUI();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("range"));
	}
}
#endif
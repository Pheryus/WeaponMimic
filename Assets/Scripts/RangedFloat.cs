using System;

[Serializable]
public struct RangedFloat {

	public float minValue;
	public float maxValue;

	public RangedFloat(float min, float max) {
		minValue = min;
		maxValue = max;
	}

	public float Random
	{
		get { return UnityEngine.Random.Range(minValue, maxValue); }
	}

	public float Lerp(float t) {
		return UnityEngine.Mathf.Lerp(minValue, maxValue, t);
	}

	public float InverseLerp(float value) {
		return UnityEngine.Mathf.InverseLerp(minValue, maxValue, value);
	}
}

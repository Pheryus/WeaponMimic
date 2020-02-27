using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AttackPattern {

    public int dmg;

    [Tooltip("Tempo que o jogador tem para dar o input para continuar o combo de uma arma")]
    public int frameWindow;

}


[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapons")]
public class Weapon : ScriptableObject {

    [SerializeField]
    public List<AttackPattern> attackPatterns;

    public string weaponName;

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//For the moment its only melee weapon

[CreateAssetMenu(fileName ="New Weapon",menuName ="Weapon")]
public class Weapon : ScriptableObject
{
    public string weaponName;
    public int damage;
    public float timeBetweenAttack;
    public int scorePerKill;
    public float range;

    public Sprite sprite;
}

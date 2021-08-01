using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New TurretAttribute", menuName = "TurretAttribute")]
public class TurretAttribute : ScriptableObject
{
    public Sprite artwork;
    public GameObject bullet;
    public string sound;
    public int ammo;
    public int bulletCount;
    public int coolDown;
    public int renewalTime;
}

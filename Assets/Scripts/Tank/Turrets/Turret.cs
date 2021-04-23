using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Turret", menuName = "Turret")]
public class Turret : ScriptableObject
{
    public Sprite artwork;
    public GameObject bullet;
    public int ammo;
    public int bulletCount;
    public int coolDown;
    public int renewalTime;
}

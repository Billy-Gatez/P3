using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/GrenadeStats")]
public class GrenadeStats : ScriptableObject
{
    [Header("Basic Settings")]
    public int damage = 40;
    public float explosionRadius = 5f;
    public float fuseTime = 2f;
    public float speed = 10f;
    public float destroyTime = 5f;

    [Header("Explosion Settings")]
    public bool explodeOnImpact = false;
    public GameObject explosionEffect;
}



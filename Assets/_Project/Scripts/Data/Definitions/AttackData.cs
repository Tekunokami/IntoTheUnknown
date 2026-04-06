using UnityEngine;

[CreateAssetMenu(fileName = "NewAttack", menuName = "GameData/Combat/Attack Data")]
public class AttackData : ScriptableObject
{
    public string attackID;
    public float damage;
    public float range;
    public float cooldown;
    public string animationTrigger; // String to trigger an animation in the Animator
}
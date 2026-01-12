using UnityEngine;

class AttackAction : EnemyAction
{
    Structure targetStructure;

    public AttackAction(EnemyBrain brain, Structure targetStructure) : base(brain)
    {
        this.targetStructure = targetStructure;
    }

    public override void Execute()
    {
        // Implementation of attack logic goes here
        Debug.Log("Enemy is attacking!");
        Complete();
    }
}
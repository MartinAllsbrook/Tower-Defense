using UnityEngine;

class AttackAction : EnemyAction
{
    public AttackAction(EnemyBrain brain) : base(brain)
    {
    }

    public override void Execute()
    {
        // Implementation of attack logic goes here
        Debug.Log("Enemy is attacking!");
        Complete();
    }
}
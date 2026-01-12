using System.Collections;
using UnityEngine;

class AttackAction : EnemyAction
{
    Structure targetStructure;

    public AttackAction(Enemy enemy, Structure targetStructure) : base(enemy)
    {
        this.targetStructure = targetStructure;
    }

    public override IEnumerator Execute()
    {
        while (targetStructure != null && !targetStructure.IsDestroyed)
        {
            targetStructure.DealDamage(enemy.AttackDamage);
            yield return new WaitForSeconds(enemy.AttackInterval);
        }

        // Finish
        Complete();
        yield return null;
    }
}
using System.Collections;
using UnityEngine;

class AttackAction : EnemyAction
{
    Structure targetStructure;

    public AttackAction(Enemy enemy, Structure targetStructure) : base(enemy)
    {
        this.targetStructure = targetStructure;
    }

    public override IEnumerator ExecuteRoutine()
    {
        EnemyMovement movement = enemy.GetComponent<EnemyMovement>();
        movement.SetLookAtTarget(targetStructure.transform.position);

        Health targetHealth = targetStructure.GetComponent<Health>();

        while (targetStructure != null && !targetHealth.IsDead())
        {
            targetHealth.DecreaseHealth(enemy.AttackDamage);
            yield return new WaitForSeconds(enemy.AttackInterval);
        }

        // Finish
        movement.ClearLookAtTarget();
        Complete();
        yield return null;
    }

    public override void StopExecution()
    {
        base.StopExecution();
        
        EnemyMovement movement = enemy.GetComponent<EnemyMovement>();
        movement.ClearLookAtTarget();
    }
}
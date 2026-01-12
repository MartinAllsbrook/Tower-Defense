using System;
using System.Collections;

abstract class EnemyAction
{
    protected Enemy enemy;

    public event Action onComplete;

    public EnemyAction(Enemy enemy)
    {
        this.enemy = enemy;
    }

    protected void Complete()
    {
        onComplete?.Invoke();
    }

    abstract public IEnumerator Execute();
}
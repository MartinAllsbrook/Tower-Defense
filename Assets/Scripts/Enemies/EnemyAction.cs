using System;
using System.Collections;
using UnityEngine;

abstract class EnemyAction
{
    protected Enemy enemy;

    public event Action onComplete;

    public Coroutine coroutine;

    public EnemyAction(Enemy enemy)
    {
        this.enemy = enemy;
    }

    protected void Complete()
    {
        onComplete?.Invoke();
    }

    public virtual void StopExecution()
    {
        enemy.StopCoroutine(coroutine);
    }

    public void Execute()
    {
        coroutine = enemy.StartCoroutine(ExecuteRoutine());
    }

    abstract public IEnumerator ExecuteRoutine();
}
using System;

abstract class EnemyAction
{
    protected EnemyBrain brain;

    public event Action onComplete;

    public EnemyAction(EnemyBrain brain)
    {
        this.brain = brain;
    }

    protected void Complete()
    {
        onComplete?.Invoke();
    }

    abstract public void Execute();
}
using UnityEngine;

public class EnemyIdleState : State<EnemyStateEnum> 
{ 
    private EnemyController _enemy;
    private float _timer;
    public EnemyIdleState(EnemyController enemy) { _enemy = enemy; }
    
    
    public override void Enter()
    {
        _timer = 0f;
        if (_enemy.steeringAgent != null) _enemy.steeringAgent.Stop();
    }
        
    public override void Execute()
    { 
        if (_enemy.CanSeeTarget()) 
        {
            _enemy.TransitionTo(EnemyStateEnum.Chase);
            return; 
        }
        _timer += Time.deltaTime; 
        // Cuando cumple el tiempo, vuelve a Patrol 
        if (_timer >= _enemy.idleTime)
        { 
            _enemy.TransitionTo(EnemyStateEnum.Patrol);
        }
    }
}
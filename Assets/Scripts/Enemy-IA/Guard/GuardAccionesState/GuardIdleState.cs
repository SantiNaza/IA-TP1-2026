using UnityEngine;

public class GuardIdleState : State<GuardStateEnum> 
{ 
    private GuardEnemyController _enemy;
    private float _timer;
    public GuardIdleState(GuardEnemyController enemy) { _enemy = enemy; }
    
    
    public override void Enter()
    {
        _timer = 0f;
        if (_enemy.steeringAgent != null) _enemy.steeringAgent.Stop();
    }
        
    public override void Execute()
    { 
        if (_enemy.CanSeeTarget()) 
        {
            _enemy.TransitionTo(GuardStateEnum.Chase);
            return; 
        }
        _timer += Time.deltaTime; 
        // Cuando cumple el tiempo, vuelve a Patrol 
        if (_timer >= _enemy.idleTime)
        { 
            _enemy.TransitionTo(GuardStateEnum.Patrol);
        }
    }
}
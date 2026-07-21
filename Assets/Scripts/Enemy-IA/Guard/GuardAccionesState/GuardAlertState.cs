using UnityEngine;

public class GuardAlertState : State<GuardStateEnum>
{
    private GuardEnemyController _enemy;

    private float _investigateTimer;
    private const float InvestigateDuration = 3f;

    private const float AlertSpeedMultiplier = 0.6f;

    private bool _reachedLastKnownPosition;

    public GuardAlertState(GuardEnemyController enemy)
    {
        _enemy = enemy;
    }

    public override void Enter()
    {
        _investigateTimer        = 0f;
        _reachedLastKnownPosition = false;

        if (_enemy.steeringAgent != null)
            _enemy.steeringAgent.maxSpeed = _enemy.speed * AlertSpeedMultiplier;

        _enemy.CalculateAlertPath();
    }

    public override void Execute()
    {
        if (_enemy.CanSeeTarget())
        {
            _enemy.TransitionTo(GuardStateEnum.Chase);
            return;
        }

        if (!_reachedLastKnownPosition)
        {
            _enemy.FollowAlertPath();

            if (_enemy.AlertPathFinished())
                _reachedLastKnownPosition = true;
        }
        else
        {
            _enemy.steeringAgent.Stop();
            _enemy.LookAround();

            _investigateTimer += Time.deltaTime;

            if (_investigateTimer >= InvestigateDuration)
                _enemy.TransitionTo(GuardStateEnum.Pathfinding);
        }
    }

    public override void Exit()
    {
        if (_enemy.steeringAgent != null)
            _enemy.steeringAgent.maxSpeed = _enemy.speed;
    }
}
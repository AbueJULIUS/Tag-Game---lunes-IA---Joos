using UnityEngine;

public class FSMClasses : MonoBehaviour
{
    State current;
    public State Current => current;
    PatrolState patrol;
    PursueState pursue;


    private void Awake()
    {
        patrol = new PatrolState(this);
        pursue = new PursueState(this);
        current = patrol;
        current.Enter();
    } 
    private void ChangeState(State newState)
    {
        if(current != newState)
        {
            current.Exit();
            current = newState;
            current.Enter();
        }
    }
    public void ChangeToPursue()
    {
        ChangeState(pursue);
    }
    public void ChangeToPatrol()
    {
        ChangeState(patrol);
    }
    public void ChangeToFlee()
    {

    }
    public void UpdateState(bool canSeePlayer)
    {
        current.Update(canSeePlayer);
    }
}
 
public abstract class State
{
    protected FSMClasses fsm;
    public State(FSMClasses fsm)
    {
        this.fsm = fsm;
    }
    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Update(bool canSeePlayer) { }
}
public class PatrolState : State
{
    EnemyModel model;
    float intervalTimer;
    Vector3 wanderDir;
    public PatrolState(FSMClasses fsm) : base(fsm) { }
    public override void Enter() 
    {
        fsm.TryGetComponent<EnemyModel>(out var enemy);
        if (enemy)
        {
            model = fsm.GetComponent<EnemyModel>();
            intervalTimer = model.WanderTimer;
            wanderDir = fsm.transform.forward;
        }
    }
    public override void Exit() { }
    public override void Update(bool canSeePlayer)
    {
        if (canSeePlayer)
        {
            fsm.ChangeToPursue();
        }
        else
        {
            intervalTimer -= Time.deltaTime;
            if (intervalTimer <= 0)
            {
                wanderDir = SteeringBehaviours.Wander(fsm.transform.forward, 180);                

                intervalTimer = model.WanderTimer;
            }
            model.Move(wanderDir);

        }
    }
}
public class PursueState : State
{
    EnemyModel model;
    Transform target;
    Rigidbody rb;
    public PursueState(FSMClasses fsm) : base(fsm){ }
    public override void Enter() 
    {
        fsm.TryGetComponent<EnemyModel>(out var enemy);
        if (enemy)
        {
            model = fsm.GetComponent<EnemyModel>();
            target = enemy.PlayerTransform;
            rb = enemy.Rb;
        }
    }
    public override void Exit() { }
    public override void Update(bool canSeePlayer)
    {
        if (!canSeePlayer)
        {
            fsm.ChangeToPatrol();
        }
        else
        {
            Vector3 dir = SteeringBehaviours.Pursue(fsm.transform, target, rb, 5);
            model.Move(dir);
        }
    }
}
public class FleeState : State
{
    EnemyModel model;
    Transform target;
    Rigidbody rb;
    public FleeState(FSMClasses fsm) : base(fsm) { }
    public override void Enter()
    {
        fsm.TryGetComponent<EnemyModel>(out var enemy);
        if (enemy)
        {
            model = fsm.GetComponent<EnemyModel>();
            target = enemy.PlayerTransform;
            rb = enemy.Rb;
        }
    }
    public override void Exit() { }
    public override void Update(bool canSeePlayer)
    {               
        Vector3 dir = SteeringBehaviours.Evade(fsm.transform, target, rb, 5);
        model.Move(dir);        
    }
}

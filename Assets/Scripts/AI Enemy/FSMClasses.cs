using System.Collections;
using UnityEngine;

public class FSMClasses : MonoBehaviour
{
    State current;
    public State Current => current;
    PatrolState patrol;
    PursueState pursue;
    FleeState flee;
    InvestigateState investigate;

    private void Awake()
    {
        patrol = new PatrolState(this);
        pursue = new PursueState(this);
        flee = new FleeState(this);
        investigate = new InvestigateState(this);
    }
    void Start()
    {
        StartCoroutine(WaitForMap());
    }

    IEnumerator WaitForMap()
    {
        while (!EnemyManager.Instance.Map.IsReady)
            yield return null;

        InitializeFSM();
    }
    void InitializeFSM()
    {
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
        ChangeState(flee);
    }
    public void ChangeToInvestigate()
    {
        ChangeState(investigate);
    }
    public void UpdateState(bool canSeePlayer)
    {
        current.Update(canSeePlayer);
    }
}
 
public abstract class State
{
    protected FSMClasses fsm;
    protected EnemyBase model;
    public State(FSMClasses fsm)
    {
        this.fsm = fsm;
        model = fsm.GetComponent<EnemyBase>();
    }
    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Update(bool canSeePlayer) { }
}
public class PatrolState : State
{
    Node randomNode;
    float intervalTimer;
    Vector3 wanderDir;
    public PatrolState(FSMClasses fsm) : base(fsm) { }
    void PickNewTarget()
    {
        randomNode = model.Manager.Map.AllNodes[
            Random.Range(0, model.Manager.Map.AllNodes.Count)
        ];

        model.RequestPath(randomNode);
    }
    public override void Enter() 
    {
        PickNewTarget();
    }
    public override void Exit() { }
    public override void Update(bool canSeePlayer)
    {
        if (canSeePlayer)
        {
            fsm.ChangeToPursue();
            return;
        }
        if (model.Manager.LastKnownPlayerPos.HasValue)
        {
            fsm.ChangeToInvestigate();
            return;
        }
        model.FollowPath();

        if (model.currentPath == null || model.pathIndex >= model.currentPath.Count)
        {
            PickNewTarget(); // nuevo destino random
        }

    }
}
public class PursueState : State
{

    Transform target;
    Rigidbody rb;
    public PursueState(FSMClasses fsm) : base(fsm){ }
    public override void Enter() 
    {
        target = model.PlayerTransform;
        rb = model.Rb;
    }
    public override void Exit() { }
    public override void Update(bool canSeePlayer)
    {
        if (model.Tagged)
        {
            fsm.ChangeToFlee();
            return;
        }

        if (!canSeePlayer)
        {
            if (model.Manager.LastKnownPlayerPos.HasValue)
                fsm.ChangeToInvestigate();
            else
                fsm.ChangeToPatrol();

            return;
        }

        Node playerNode = model.FindClosestNode(model.PlayerTransform.position);

        model.RequestPath(playerNode);
        model.FollowPath();
    }
}
public class FleeState : State
{
    Transform target;
    Rigidbody rb;
    public FleeState(FSMClasses fsm) : base(fsm) { }
    public override void Enter()
    {
        target = model.PlayerTransform;
        rb = model.Rb;
    }
    public override void Exit() { }
    public override void Update(bool canSeePlayer)
    {
        if (!model.Tagged)
        {
            if (canSeePlayer)
                fsm.ChangeToPursue();
            else
                fsm.ChangeToPatrol();

            return;
        }

        Vector3 dir = SteeringBehaviours.Evade(
            fsm.transform,
            target,
            rb,
            5);

        model.Move(dir);
    }
}
public class InvestigateState : State
{
    EnemyManager manager;

    public InvestigateState(FSMClasses fsm) : base(fsm) { }

    public override void Enter()
    {
        manager = model.Manager;
    }

    public override void Update(bool canSeePlayer)
    {
        //si encuentra, persigue
        if (canSeePlayer)
        {
            fsm.ChangeToPursue();
            return;
        }

        //si ya no conce, vuelve a patrullar
        if (!manager.LastKnownPlayerPos.HasValue)
        {
            fsm.ChangeToPatrol();
            return;
        }

        Node targetNode = model.FindClosestNode(manager.LastKnownPlayerPos.Value);

        model.RequestPath(targetNode);
        model.FollowPath();

        if (Vector3.Distance(model.transform.position, targetNode.transform.position) < 1f)
        {
            manager.ClearPlayerPosition();
            fsm.ChangeToPatrol();
        }
    }
}

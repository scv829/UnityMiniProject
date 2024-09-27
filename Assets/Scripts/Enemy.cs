using System.Collections;
using UnityEngine;
using UnityEngine.AI;


public class Enemy : MonoBehaviour
{
    [Header("Property")]
    [SerializeField] SearchArea searchArea;
    [SerializeField] NavMeshAgent agent;
    [Header("Attack")]
    [SerializeField] AttackArea attackArea;
    [SerializeField] Transform baseTarget;
    [SerializeField] float attackRange;
    private float realAttackRange;
    [Header("State")]
    [SerializeField] State curState;
    public enum State { Trace, Attack, Die, Size }
    BaseState[] states = new BaseState[(int)State.Size];
    private Coroutine setTargetCoroutine;
    [SerializeField] bool isStop;


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        states[(int)State.Trace] = new TraceState(this);
        states[(int)State.Attack] = new AttackState(this);
        states[(int)State.Die] = new DieState(this);
        realAttackRange = GetComponent<CapsuleCollider>().radius + attackRange;

        isStop = agent.isStopped;
    }

    private void Start()
    {
        baseTarget = GameObject.FindWithTag("Nexus").transform;
        curState = State.Trace;
        states[(int)curState].Enter();
    }

    private void Update()
    {
        states[(int)curState].Update();
    }

    public void ChangeState(State state)
    {
        states[(int)curState].Exit();
        curState = state;
        states[(int)curState].Enter();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, (realAttackRange * realAttackRange));
    }


    private class TraceState : BaseState
    {
        [SerializeField] Enemy enemy;
        [SerializeField] Vector3 targetPosition;

        public TraceState(Enemy enemy) { this.enemy = enemy; }

        public override void Update()
        {
            // Trace
            targetPosition = (enemy.searchArea.Target != null) ? enemy.searchArea.Target.position : enemy.baseTarget.position;
            enemy.agent.destination = targetPosition;


            // if AttackArea in player or Nexus
            if (enemy.attackArea.Target != null && enemy.attackArea.Target.Equals(enemy.searchArea.Target))
            {
                enemy.ChangeState(State.Attack);
            }
        }

    }

    private class AttackState : BaseState
    {
        [SerializeField] GameObject attackPrefab;
        [SerializeField] float attackDamage;
        [SerializeField] float attackSpeed;
        [SerializeField] Enemy enemy;
        [SerializeField] Vector3 targetPosition;
        private Coroutine attackCoroutine;

        public AttackState(Enemy enemy) { this.enemy = enemy; }

        public override void Enter()
        {
            Debug.Log("enemy Attack Start");
            attackCoroutine = enemy.StartCoroutine(attacking());
            enemy.agent.isStopped = !enemy.agent.isStopped;
        }

        public override void Update()
        {
            targetPosition = (enemy.searchArea.Target != null) ? enemy.searchArea.Target.position : enemy.baseTarget.position;
            enemy.agent.destination = targetPosition;

            if (enemy.attackArea.Target == null)
            {
                enemy.ChangeState(State.Trace);
            }
        }

        public override void Exit()
        {
            Debug.Log("enemy Attack Stop!");
            enemy.StopCoroutine(attackCoroutine);
            attackCoroutine = null;
            enemy.agent.isStopped = !enemy.agent.isStopped;
        }

        private IEnumerator attacking()
        {

            while (true)
            {
                Debug.LogWarning($"{enemy.name} is Attaking!");
                yield return new WaitForSeconds(attackSpeed);
            }
        }
    }

    private class DieState : BaseState
    {
        private Enemy enemy;

        public DieState(Enemy enemy) { this.enemy = enemy; }

        public override void Enter()
        {
            Debug.Log($"{enemy.name} is Dead");
            Destroy(enemy.gameObject);
        }
    }

}

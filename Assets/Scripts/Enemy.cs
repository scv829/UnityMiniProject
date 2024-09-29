using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


public class Enemy : MonoBehaviour, IHit
{
    [Header("Property")]
    [SerializeField] SearchArea searchArea;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] float hp;
    [Header("Attack")]
    [SerializeField] AttackArea attackArea;
    [SerializeField] Transform baseTarget;
    [SerializeField] float attackSpeed;
    [SerializeField] float attackDamage;
    [Header("UI")]
    [SerializeField] Slider hpBar;
    [SerializeField] float offset;
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
    }

    private void Start()
    {
        hpBar.maxValue = hp;
        hpBar.value = hp;
        hpBar.gameObject.SetActive(false);
        baseTarget = GameObject.FindWithTag("Nexus").transform;
        curState = State.Trace;
        states[(int)curState].Enter();
    }

    private void Update()
    {
        hpBar.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, transform.localScale.y + offset, 0));
        states[(int)curState].Update();
    }

    public void ChangeState(State state)
    {
        states[(int)curState].Exit();
        curState = state;
        states[(int)curState].Enter();
    }


    public void TakeDamage(float damage)
    {
        hpBar.gameObject.SetActive(true);
        hp -= damage;
        if (hp <= 0)
        {
            ChangeState(State.Die);
        }
        hpBar.value = hp;
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


            // 공격 범위안에 들어왔으며 적이 피격가능한 오브젝트일 때
            if (enemy.attackArea.Target != null && enemy.attackArea.Target.GetComponent<IHit>() is IHit)
            {
                // 공격 실행
                enemy.ChangeState(State.Attack);
            }
        }

    }

    private class AttackState : BaseState
    {
        [SerializeField] Enemy enemy;
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
                enemy.attackArea.Target.GetComponent<IHit>().TakeDamage(enemy.attackDamage);
                yield return new WaitForSeconds(enemy.attackSpeed);
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

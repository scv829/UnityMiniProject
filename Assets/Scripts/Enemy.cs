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
    [SerializeField] Animator animator;
    [SerializeField] GameObject hitPoint;

    [Header("Attack")]
    [SerializeField] AttackArea attackArea;
    [SerializeField] Transform baseTarget;
    [SerializeField] float attackSpeed;
    [SerializeField] float attackDamage;

    [Header("Die")]
    [SerializeField] GameObject dieEffect;

    [Header("UI")]
    [SerializeField] Slider hpBar;
    [SerializeField] float offset;

    [Header("State")]
    [SerializeField] State curState;
    public enum State { Trace, Attack, Die, Size }
    BaseState[] states = new BaseState[(int)State.Size];
    private Coroutine setTargetCoroutine;

    [Header("Object_Pool")]
    [SerializeField] EnemyPool returnPoll;
    [SerializeField] EnemyType enemyType;
    public enum EnemyType { Trace, Attack, Die, Size }

    public int Type { set { enemyType = (EnemyType)value; } }
    public EnemyPool ReturnPoll { set { returnPoll = value; } }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        states[(int)State.Trace] = new TraceState(this);
        states[(int)State.Attack] = new AttackState(this);
        states[(int)State.Die] = new DieState(this);
    }

    private void OnEnable()
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

    public Transform HitPoint()
    {
        return hitPoint.transform;
    }

    private class TraceState : BaseState
    {
        [SerializeField] Enemy enemy;
        [SerializeField] Vector3 targetPosition;

        public TraceState(Enemy enemy) { this.enemy = enemy; }

        public override void Update()
        {
            // Trace
            if(enemy.baseTarget != null)
            {
                targetPosition = (enemy.searchArea.Target != null) ? enemy.searchArea.Target.position : enemy.baseTarget.position;
                enemy.agent.destination = targetPosition;
            }
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
        [SerializeField] float attackCoolTime;
        [SerializeField] float currentAttackCoolTime;
        
        private Coroutine attackCoroutine;

        public AttackState(Enemy enemy) { 
            this.enemy = enemy;

            // 공격 속도에 따른 애니메이션 배속 설정
            attackCoolTime = 1f / enemy.attackSpeed;
            
            if (enemy.attackSpeed > 1) enemy.animator.SetFloat("AttackSpeed", enemy.attackSpeed);
            else enemy.animator.SetFloat("AttackSpeed", 1);

            // 처음 공격할 때는 바로 공격
            currentAttackCoolTime = attackCoolTime;
        }

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
                // 만약 공격 쿨타임이 돌았으면
                if(currentAttackCoolTime >= attackCoolTime)
                {
                    // 쿨타임 초기화
                    currentAttackCoolTime = 0f;
                    // 공격 개시
                    enemy.animator.SetTrigger("AttackTrigger");
                    enemy.attackArea.Target.GetComponent<IHit>().TakeDamage(enemy.attackDamage);
                }
                
                currentAttackCoolTime += Time.deltaTime;
                // 쿨타임을 매 프레임으로 확인 
                yield return null;
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
            GameObject obj = Instantiate(enemy.dieEffect);
            obj.transform.position = enemy.transform.position;
            Destroy(obj, 2f);
        }
    }

}

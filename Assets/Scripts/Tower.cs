using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tower : MonoBehaviour, IHit
{
    [Header("Property")]
    [SerializeField] float hp;

    [Header("Attack")]
    [SerializeField] AttackArea attackArea;
    [SerializeField] float attackRange;
    [SerializeField] GameObject attackPrefab;
    [SerializeField] float attackDamage;
    [SerializeField] float attackSpeed;
    [SerializeField] Vector3 targetPosition;

    [Header("State")]
    [SerializeField] State curState;
    public enum State { Idle, Attack, Die, Size }
    BaseState[] states = new BaseState[(int)State.Size];

    [Header("UI")]
    [SerializeField] Slider hpBar;
    [SerializeField] float offset;

    [Header("Die")]
    [SerializeField] GameObject dieEffect;

    private void Awake()
    {
        states[(int)State.Idle] = new IdleState(this);
        states[(int)State.Attack] = new AttackState(this);
        states[(int)State.Die] = new DieState(this);
    }

    private void Start()
    {
        hpBar.maxValue = hp;
        hpBar.value = hp;
        hpBar.gameObject.SetActive(false);
        curState = State.Idle;
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

    private class IdleState : BaseState
    {
        private Tower tower;

        public IdleState(Tower tower)
        {
            this.tower = tower;
        }

        public override void Update()
        {
            // 공격 범위 안에 적이 들어왔을 때
            if (tower.attackArea.Target != null)
            {
                // 공격 시작
                tower.ChangeState(State.Attack);
            }
        }

    }

    private class AttackState : BaseState
    {
        private Tower tower;
        private Coroutine attackCoroutine;



        public AttackState(Tower tower)
        {
            this.tower = tower;
        }

        public override void Enter()
        {
            // 공격을 시작
            Debug.Log("Tower Attack Start");
            // 공격하는 코루틴 시작
            attackCoroutine = tower.StartCoroutine(attacking());
        }

        public override void Update()
        {
            // 공격할 타겟이 없으면 그만하기
            if(tower.attackArea.Target == null)
            {
                tower.ChangeState(State.Idle);
            }
        }

        public override void Exit()
        {
            // 공격 멈추기
            Debug.Log("tower Attack Stop!");
            tower.StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }

        private IEnumerator attacking()
        {

            while (true)
            {
                GameObject instance = Instantiate(tower.attackPrefab, tower.transform.position, Quaternion.identity);
                instance.GetComponent<AttackObejct>().Setting(tower.attackArea.Target, tower.attackDamage);
                yield return new WaitForSeconds(tower.attackSpeed);
            }
        }

    }

    private class DieState : BaseState
    {
        private Tower tower;

        public DieState(Tower tower)
        {
            this.tower = tower;
        }

        public override void Enter()
        {
            Destroy(tower.gameObject);
            GameObject obj = Instantiate(tower.dieEffect);
            obj.transform.position = tower.transform.position;
            Destroy(obj, 2f);
        }
    }

}


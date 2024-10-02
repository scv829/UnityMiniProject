using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Nexus : MonoBehaviour, IHit, Interaction, IUpgrade
{
    [Header("Property")]
    [SerializeField] float hp;
    [SerializeField] float maxHp;
    [SerializeField] List<Mesh> meshes;
    [SerializeField] MeshFilter currentMesh;
    [SerializeField] MeshRenderer render;
    [SerializeField] NavMeshObstacle navMeshObstacle;
    [SerializeField] BoxCollider boxCollider;

    [Header("Attack")]
    [SerializeField] AttackArea attackArea;
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

    [Header("Interaction")]
    [SerializeField] int[] upgradeCost;
    [SerializeField] int currentLevel;
    [SerializeField] int useCoinCount;

    private Coroutine upgradeCoroutine;

    private void Awake()
    {
        states[(int)State.Idle] = new IdleState(this);
        states[(int)State.Attack] = new AttackState(this);
        states[(int)State.Die] = new DieState(this);

        upgradeCoroutine = null;

        render = GetComponent<MeshRenderer>();
        navMeshObstacle = GetComponent<NavMeshObstacle>();
        boxCollider = GetComponent<BoxCollider>();
        currentMesh = GetComponent<MeshFilter>();
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

    public Transform HitPoint()
    {
        return gameObject.transform;
    }

    public void InteractAction()
    {
        if (Input.GetKeyDown(KeyCode.Space) && upgradeCoroutine == null && currentLevel < upgradeCost.Length)
        {
            upgradeCoroutine = StartCoroutine(UseCoinToUpgrade());
        }
        else if (Input.GetKeyUp(KeyCode.Space) && upgradeCoroutine != null)
        {
            StopCoroutine(upgradeCoroutine);
            upgradeCoroutine = null;
        }
    }

    public void GetMission()
    {
        // UI 설정
        if (currentLevel < upgradeCost.Length) Debug.Log($"UpgradeCost : {useCoinCount} / {upgradeCost[currentLevel]} ");
        else Debug.Log("MaxLevel");
    }

    // 실제 업그레이드
    public void Upgrade()
    {
        if (currentLevel == 0)
        {
            boxCollider.enabled = true;
            render.enabled = true;
            navMeshObstacle.enabled = true;
        }
        else if(currentLevel == 1)
        {
            maxHp *= 2;
            hpBar.maxValue = maxHp;
            hp = maxHp;
            attackArea.gameObject.SetActive(true);
        }
        else
        {
            attackDamage *= 2;
            attackSpeed *= 2;
            attackArea.GetComponent<SphereCollider>().radius *= 1.5f;
            maxHp *= 2;
            hpBar.maxValue = maxHp;
            hp = maxHp;
        }

        currentMesh.mesh = meshes[currentLevel++];
    }

    IEnumerator UseCoinToUpgrade()
    {
        while (GameManager.instance.IsEnough)
        {
            GameManager.instance.DecreaseCoin();
            useCoinCount++;

            if (useCoinCount == upgradeCost[currentLevel])
            {
                Upgrade();
                useCoinCount = 0;
                Debug.Log("do");
                break;
            }
            else if (useCoinCount > upgradeCost[currentLevel])
            {
                for(int i = 0; i < useCoinCount - upgradeCost[currentLevel]; i++) GameManager.instance.IncreaseCoin();

                Upgrade();
                useCoinCount = 0;
                Debug.Log("do");
                break;
            }

            Debug.Log($"useCoin : {useCoinCount}");
            yield return new WaitForSeconds(0.5f);
        }
    }

    private class IdleState : BaseState
    {
        private Nexus nexus;

        public IdleState(Nexus nexus)
        {
            this.nexus = nexus;
        }

        public override void Update()
        {
            // 기지의 레벨이 1(업그레이드 진행)이상이고 공격 범위 안에 적이 들어왔을 때
            if (nexus.currentLevel >= 1 && nexus.attackArea.Target != null)
            {
                // 공격 시작
                nexus.ChangeState(State.Attack);
            }
        }

    }

    private class AttackState : BaseState
    {
        private Nexus nexus;
        private Coroutine attackCoroutine;

        public AttackState(Nexus nexus)
        {
            this.nexus = nexus;
        }

        public override void Enter()
        {
            // 공격을 시작
            Debug.Log("Tower Attack Start");
            // 공격하는 코루틴 시작
            attackCoroutine = nexus.StartCoroutine(attacking());
        }

        public override void Update()
        {
            // 공격할 타겟이 없으면 그만하기
            if (nexus.attackArea.Target == null)
            {
                nexus.ChangeState(State.Idle);
            }
        }

        public override void Exit()
        {
            // 공격 멈추기
            Debug.Log("tower Attack Stop!");
            nexus.StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }

        private IEnumerator attacking()
        {
            while (true)
            {
                GameObject instance = Instantiate(nexus.attackPrefab, nexus.transform.position, Quaternion.identity);
                instance.GetComponent<AttackObejct>().Setting(nexus.attackArea.Target, nexus.attackDamage);
                yield return new WaitForSeconds(nexus.attackSpeed);
            }
        }

    }

    private class DieState : BaseState
    {
        private Nexus nexus;

        public DieState(Nexus nexus)
        {
            this.nexus = nexus;
        }

        public override void Enter()
        {
            Destroy(nexus.gameObject);
            GameObject obj = Instantiate(nexus.dieEffect);
            obj.transform.position = nexus.transform.position;
            Destroy(obj, 2f);

            // 게임 오버 로직
            Time.timeScale = 0f;
        }
    }
}

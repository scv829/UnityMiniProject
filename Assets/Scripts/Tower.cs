using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Tower : MonoBehaviour, IHit, Interaction, IUpgrade
{
    [Header("Property")]
    [SerializeField] float hp;
    [SerializeField] GameObject hitPoint;
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
    [SerializeField] float maxHp;
    [SerializeField] float offset;

    [Header("Die")]
    [SerializeField] GameObject dieEffect;

    [Header("Interaction")]
    [SerializeField] int[] upgradeCost;
    [SerializeField] int currentLevel;
    [SerializeField] int useCoinCount;
    [TextArea(3, 5)] 
    [SerializeField] string info;

    private StringBuilder sb;
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

        sb = new StringBuilder();
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
        return hitPoint.transform;
    }

    public void GetMission()
    {
        // UI 설정
        if (currentLevel < upgradeCost.Length)
        {
            sb.Clear();
            // 건물에 대한 설명
            sb.AppendLine(info);

            if(currentLevel == 0)
            {
                sb.AppendLine("\n건설 비용");
            }
            else
            {
                // 목표 레벨 : 현재 레밸 -> 목표 레벨
                sb.AppendLine($"{currentLevel} -> {currentLevel + 1}");
                // 공격력
                sb.AppendLine($"{attackDamage} -> {attackDamage * 2}");
                // 공격속도
                sb.AppendLine($"{attackSpeed} -> {attackSpeed * 2}");
                // 공격 범위
                sb.AppendLine($"{attackArea.Redius} -> {attackArea.Redius * 2}");
                sb.AppendLine("\n업그레이드 비용 비용");
            }

            // 코스트
            sb.AppendLine($"Cost : {useCoinCount} / {upgradeCost[currentLevel]} ");
        }
        else
        {
            sb.Clear();
            // 건물에 대한 설명
            sb.AppendLine(info);
            // 목표 레벨 : 현재 레밸 -> 목표 레벨
            sb.AppendLine($"{currentLevel} -> MaxLevel");
            // 공격력
            sb.AppendLine($"{attackDamage} -> MaxLevel");
            // 공격속도
            sb.AppendLine($"{attackSpeed} -> MaxLevel");
            // 공격 범위
            sb.AppendLine($"{attackArea.Redius} -> MaxLevel");

            // 코스트
            Debug.Log($"UpgradeCost : MaxLevel ");
        }

        GameManager.instance.SetUpgradeMission(sb, gameObject.name);
    }

    // 실제 업그레이드
    public void Upgrade()
    {
        if(currentLevel == 0)
        {
            boxCollider.enabled = true;
            render.enabled = true;
            navMeshObstacle.enabled = true;
            attackArea.gameObject.SetActive(true);
        }
        else
        {
            attackDamage *= 2;
            attackSpeed *= 2;
            attackArea.Redius *= 2;
            hp = maxHp * 2;
        }

        currentMesh.mesh = meshes[currentLevel++];
        if (GameManager.instance.IsShowUpgradeUI) GetMission();
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
                break;
            }
            
            GetMission();
            yield return new WaitForSeconds(0.5f);
        }
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


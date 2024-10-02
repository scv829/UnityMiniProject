using System.Collections;
using System.Collections.Generic;
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

    private void Start()
    {
        hpBar.maxValue = hp;
        hpBar.value = hp;
        hpBar.gameObject.SetActive(false);
    }

    private void Update()
    {
        hpBar.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, transform.localScale.y + offset, 0));
    }

    public void TakeDamage(float damage)
    {
        hpBar.gameObject.SetActive(true);
        hp -= damage;
        if (hp <= 0)
        {
            Debug.Log("Gameover");
            Destroy(gameObject);
            GameObject obj = Instantiate(dieEffect);
            obj.transform.position = transform.position;
            Destroy(obj, 2f);

            Time.timeScale = 0f;
        }
        hpBar.value = hp;
    }

    public Transform HitPoint()
    {
        return gameObject.transform;
    }

    public void InteractAction()
    {
        if (Input.GetKeyDown(KeyCode.Space) && upgradeCoroutine == null)
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
            attackArea.gameObject.SetActive(true);
        }
        else
        {
            attackDamage *= 2;
            attackSpeed *= 2;
            attackArea.GetComponent<SphereCollider>().radius *= 2;
            hp = maxHp * 2;
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
            Debug.Log($"useCoin : {useCoinCount}");
            yield return new WaitForSeconds(0.5f);
        }
    }
}

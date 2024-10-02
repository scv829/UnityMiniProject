using System;
using System.Collections;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Nexus")]
    [SerializeField] bool isBuildNexus;     // 본진을 지었는지 확인
    [SerializeField] GameObject tutorial;

    public bool BuildNexus { get { return isBuildNexus; } set { isBuildNexus = value; tutorial.SetActive(false);  } }

    // 웨이브에 대한 정보
    [Header("Wave")]
    [SerializeField] UnityEvent startWave;  // 웨이브 시작이벤트
    [SerializeField] UnityEvent endWave;  // 웨이브 종료 이벤트
    [SerializeField] bool isStartWave;      // 웨이브 시작 했는지 확인
    [SerializeField] int currentWave;        // 총 웨이브의 수
    [SerializeField] int totalWave;        // 총 웨이브의 수
    [SerializeField] int enemiesCount;      // 웨이브에 등장하는 몬스터 종류
    [SerializeField] int[,] waveArray;      // 2차원 배열 [wave 스테이지][적종류] -> 적이 나와야 하는 수

    public bool IsStartWave { get { return isStartWave; } }

    [Space]
    [Header("Charge")]
    [SerializeField] Slider gaugeSlider;
    [SerializeField] TextMeshProUGUI chargeTimeText;
    [SerializeField] float chargeGauge;

    [Space]
    [Header("Coin")]
    [SerializeField] TextMeshProUGUI coinCountText;
    [SerializeField] int coinCount;

    [Space]
    [Header("UI")]
    [Header("Upgrade")]
    [SerializeField] GameObject UpgradeUI;
    [SerializeField] TextMeshProUGUI buildingName;
    [SerializeField] TextMeshProUGUI buildingUpgradeText;
    [SerializeField] bool isShowUpgradeUI;
    [SerializeField] Transform upgradeTarget;
    [Header("Wave")]
    [SerializeField] Image WaveUI;
    [SerializeField] TextMeshProUGUI waveText;
    [Header("Result")]
    [SerializeField] GameObject gameResultUI;
    [SerializeField] TextMeshProUGUI gameResultText;

    private StringBuilder textStringBuilder;
    private Coroutine holdingCoroutine;
    

    public void GetWave(ref int[] wave)
    {
        for (int i = 0; i < waveArray.GetLength(1); i++)
        {
            wave[i] = waveArray[currentWave, i];
        }
    }

    public void WaveClear() { currentWave++; endWave?.Invoke(); isStartWave = false; }

    public bool IsEnough { get { return (coinCount > 0); } }
    public void IncreaseCoin() => coinCount++;
    public void DecreaseCoin() => coinCount--;
    public bool IsShowUpgradeUI { get { return isShowUpgradeUI; } set { isShowUpgradeUI = value; UpgradeUI.SetActive(isShowUpgradeUI); } }
    public Transform UpgradeTarget { set { upgradeTarget = value; } }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(this);

        holdingCoroutine = null;
        textStringBuilder = new StringBuilder();

        // 웨이브와 해당 웨이브에 나올 몬스터의 수 배정
        waveArray = new int[,]
            {
                { 1, 0, 0 },
                { 0, 2, 0 },
                { 2, 1, 1 },
                { 4, 2, 2 },
            };

        // 총 웨이브의 수 지정
        totalWave = waveArray.GetLength(0);
        currentWave = 0;
        isStartWave = false;
        isBuildNexus = false;
    }

    private void Update()
    {
        CoinCounting();     // 코인 흭득시 -> UI 변경

        if (currentWave >= totalWave)
        {
            GameClear();
        }
    }

    private void CoinCounting()
    {
        textStringBuilder.Clear();
        textStringBuilder.Append($"{coinCount}");

        coinCountText.SetText(textStringBuilder);
    }

    // 스페이스바 홀딩 -> 게임 시작
    public void HoldingSpace()
    {
        // 스페이스 바 홀드하면 웨이브 시작
        if (Input.GetKeyDown(KeyCode.Space) && holdingCoroutine == null && !isStartWave && isBuildNexus)
        {
            gaugeSlider.gameObject.SetActive(true);
            chargeTimeText.gameObject.SetActive(true);

            holdingCoroutine = StartCoroutine(IncreaseChargeGauge());
        }
        else if (Input.GetKeyUp(KeyCode.Space) && holdingCoroutine != null && !isStartWave)
        {
            StopCoroutine(holdingCoroutine);
            holdingCoroutine = StartCoroutine(DecreaseChargeGauge());
        }
        else if (chargeGauge <= 0 && holdingCoroutine != null)
        {
            StopCoroutine(holdingCoroutine);
            holdingCoroutine = null;
            chargeGauge = 0;

            gaugeSlider.gameObject.SetActive(false);
            chargeTimeText.gameObject.SetActive(false);
        }
    }

    IEnumerator IncreaseChargeGauge()
    {
        while (chargeGauge <= 3)
        {
            textStringBuilder.Clear();
            textStringBuilder.Append($"{chargeGauge:F1} s");

            chargeTimeText.SetText(textStringBuilder);
            gaugeSlider.value = chargeGauge;

            chargeGauge += Time.deltaTime;
            yield return null;
        }

        gaugeSlider.gameObject.SetActive(false);
        chargeTimeText.gameObject.SetActive(false);

        startWave?.Invoke();
        isStartWave = true;
        SetText();
    }

    IEnumerator DecreaseChargeGauge()
    {
        while (chargeGauge > 0)
        {
            textStringBuilder.Clear();
            textStringBuilder.Append($"{chargeGauge:F1} s");

            chargeTimeText.SetText(textStringBuilder);
            gaugeSlider.value = chargeGauge;

            chargeGauge -= Time.deltaTime;
            yield return null;
        }

    }

    public void SetUpgradeMission(StringBuilder sb, string name)
    {
        UpgradeUI.transform.position = Camera.main.WorldToScreenPoint(upgradeTarget.position + new Vector3(0, -30f, 0));
        textStringBuilder.Clear();
        textStringBuilder.Append(name);
        buildingName.SetText(textStringBuilder);
        buildingUpgradeText.SetText(sb);
    }

    public void SetText()
    {
        textStringBuilder.Clear();
        textStringBuilder.Append($"{currentWave} / {totalWave}");

        waveText.SetText(textStringBuilder);

        StartCoroutine(FadeInFadeOut());
    }

    IEnumerator FadeInFadeOut()
    {
        WaveUI.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        WaveUI.gameObject.SetActive(false);
    }

    private void GameClear()
    {
        textStringBuilder.Clear();
        textStringBuilder.Append("Victory");

        gameResultUI.gameObject.SetActive(true);
        gameResultText.SetText(textStringBuilder);
    }

    public void GameOver()
    {
        textStringBuilder.Clear();
        textStringBuilder.Append("Defeat");
        gameResultUI.gameObject.SetActive(true);

        gameResultText.SetText(textStringBuilder);
    }

    public void ReturnMenu()
    {
        // Scene 매니저로 씬 전환하는 부분
        Debug.Log("씬 전환!");
    }

}

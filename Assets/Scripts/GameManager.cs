using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Nexus")]
    [SerializeField] bool isBuildNexus;     // 본진을 지었는지 확인
    [SerializeField] GameObject tutorial;

    public bool BuildNexus { get { return isBuildNexus; } set { isBuildNexus = value; } }

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

    [Space]
    [Header("Sound")]
    [SerializeField] AudioClip audioClip;


    public void GetWave(ref int[] wave)
    {
        for (int i = 0; i < waveArray.GetLength(1); i++)
        {
            wave[i] = waveArray[currentWave, i];
        }
    }

    public void WaveClear() { currentWave++; endWave?.Invoke(); isStartWave = false; }

    public bool IsEnough { get { return (coinCount > 0); } }
    public void IncreaseCoin() { coinCount++; CoinUIUpdate(); }
    public void DecreaseCoin() { coinCount--; CoinUIUpdate(); }
    public bool IsShowUpgradeUI { get { return isShowUpgradeUI; } set { isShowUpgradeUI = value; UpgradeUI.SetActive(isShowUpgradeUI); } }
    public Transform UpgradeTarget { set { upgradeTarget = value; } }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else Destroy(this);

        holdingCoroutine = null;
        textStringBuilder = new StringBuilder();

        // 웨이브와 해당 웨이브에 나올 몬스터의 수 배정
        waveArray = new int[,]
            {
                { 1, 0, 0 },
            //{ 0, 2, 0 },
            //{ 2, 0, 1 },
            };

        // 총 웨이브의 수 지정
        totalWave = waveArray.GetLength(0);
        currentWave = 0;
        isStartWave = false;
        isBuildNexus = false;

        Time.timeScale = 1f;
    }

    private void Update()
    {
        if (currentWave >= totalWave)
        {
            GameClear();
        }
    }

    private void CoinUIUpdate()
    {
        textStringBuilder.Clear();
        textStringBuilder.Append($"{coinCount}");

        coinCountText.SetText(textStringBuilder);
    }

    // 스페이스바 홀딩 -> 게임 시작
    public void HoldingSpace()
    {
        // 스페이스 바 홀드하면 웨이브 시작
        if (Input.GetKeyDown(KeyCode.Space) && !isStartWave && isBuildNexus)
        {
            gaugeSlider.gameObject.SetActive(true);     // 게이지 UI 활성화
            chargeTimeText.gameObject.SetActive(true);  // 시간 UI 활성화

            // 감소하는 코루틴 실행 중 -> 중단하기
            if (holdingCoroutine != null) StopCoroutine(holdingCoroutine);
            // 충전하는 코루틴 시작
            holdingCoroutine = StartCoroutine(IncreaseChargeGauge());
        }
        else if (Input.GetKeyUp(KeyCode.Space) && !isStartWave)
        {
            // 충전하는 코루틴 실행 중
            if(holdingCoroutine != null)
            {
                // 코루틴 중단하기
                StopCoroutine(holdingCoroutine);
                // 감소하는 코루틴 시작
                holdingCoroutine = StartCoroutine(DecreaseChargeGauge());
            }
        }
        // 감소하는 코루틴으로 모두 감소하면
        else if (chargeGauge <= 0 && holdingCoroutine != null)
        {
            // 감소하는 코루틴 중단
            StopCoroutine(holdingCoroutine);
            holdingCoroutine = null;
            chargeGauge = 0;

            gaugeSlider.gameObject.SetActive(false);    // 게이지 UI 비활성화
            chargeTimeText.gameObject.SetActive(false); // 시간 UI 비활성화
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

        if(tutorial != null) tutorial.SetActive(false);
        startWave?.Invoke();
        isStartWave = true;
        chargeGauge = 0;
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
        UpgradeUI.transform.position = Camera.main.WorldToScreenPoint(upgradeTarget.position + Vector3.down * 8f);
        textStringBuilder.Clear();
        textStringBuilder.Append(name);
        buildingName.SetText(textStringBuilder);
        buildingUpgradeText.SetText(sb);
    }

    public void SetText()
    {
        textStringBuilder.Clear();
        textStringBuilder.Append($"{currentWave + 1} / {totalWave}");

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
        Time.timeScale = 0f;

        textStringBuilder.Clear();
        textStringBuilder.Append("Defeat");
        gameResultUI.gameObject.SetActive(true);

        gameResultText.SetText(textStringBuilder);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void ReturnMenu()
    {
        SceneManager.LoadScene("StartScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

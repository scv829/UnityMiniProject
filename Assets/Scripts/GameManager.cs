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

    // 웨이브에 대한 정보
    [Header("Wave")]
    [SerializeField] UnityEvent startWave;  // 웨이브 시작이벤트
    [SerializeField] UnityEvent endWave;  // 웨이브 종료 이벤트
    [SerializeField] bool isStartWave;      // 웨이브 시작 했는지 확인
    [SerializeField] int currentWave;        // 총 웨이브의 수
    [SerializeField] int totalWave;        // 총 웨이브의 수
    [SerializeField] int enemiesCount;      // 웨이브에 등장하는 몬스터 종류
    [SerializeField] int[,] waveArray;      // 2차원 배열 [wave 스테이지][적종류] -> 적이 나와야 하는 수

    [Space]
    [Header("Charge")]
    [SerializeField] Slider gaugeSlider;
    [SerializeField] TextMeshProUGUI chargeTimeText;
    [SerializeField] float chargeGauge;

    [Space]
    [Header("Coin")]
    [SerializeField] TextMeshProUGUI coinCountText;
    [SerializeField] int coinCount;

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

    
    public void IncreaseCoin() => coinCount++;

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
    }

    private void Update()
    {
        HoldingSpace();     // 스페이스바 홀딩 -> 게임 시작
        CoinCounting();     // 코인 흭득시 -> UI 변경


        if (currentWave >= totalWave)
        {
           // 게임 승리 로직
        }
    }

    private void CoinCounting()
    {
        textStringBuilder.Clear();
        textStringBuilder.Append($"{coinCount}");

        coinCountText.SetText(textStringBuilder);
    }

    private void HoldingSpace()
    {
        // 스페이스 바 홀드하면 웨이브 시작
        if (Input.GetKeyDown(KeyCode.Space) && holdingCoroutine == null && !isStartWave)
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

}

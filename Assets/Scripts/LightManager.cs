using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    [SerializeField] Light mainLight;   // 해
    [SerializeField] Light spotLight;   // 기지 주변을 밝혀줄 빛

    [SerializeField] Color day;         // 낮의 빛색
    [SerializeField] Color night;       // 밤의 빛색
    [SerializeField] float delay;       // 부드럽게 변하기 위한 딜레이 시간
    [SerializeField] float duration;    // 변하는데 걸리는 시간

    public void DayToNight()
    {
        // 이미 밤이면 할 이유 없음
        if (mainLight.color.Equals(night)) return;

        spotLight.gameObject.SetActive(true);
        StartCoroutine(ChangeLight(day, night));
    }

    public void NightToDay()
    {
        // 이미 낮이면 할 이유 없음
        if (mainLight.color.Equals(day)) return;

        spotLight.gameObject.SetActive(false);
        StartCoroutine(ChangeLight(night, day));
    }

    private IEnumerator ChangeLight(Color from, Color to)
    {
        // 변하는 비율 
        float percent = 0;
        // 증가량
        float increment = delay / duration;

        while (percent < 1)
        {
            mainLight.color = Color.Lerp (from, to, percent);
            percent += increment;
            yield return new WaitForSeconds(delay);
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] UnityEvent startWave;

    // 웨이브에 대한 정보
    // 2차원 배열 [wave 스테이지][적종류] -> 적이 나와야 하는 수
   


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(this);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            startWave?.Invoke();
        }
    }
}

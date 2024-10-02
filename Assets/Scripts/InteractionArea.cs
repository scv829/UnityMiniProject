using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionArea : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] GameObject InteractionObject;
    [SerializeField] Interaction interaction;
    [SerializeField] IUpgrade upgrade;
    [SerializeField] GameObject spot;

    private void Start()
    {
        // 상호 작용 기능이 있는 오브젝트냐 -> 이 스크립트가 있으면 무조건 있을텐데 혹시 모르는 예외처리
        if(InteractionObject.GetComponent<Interaction>() is not null)
        {
            interaction = InteractionObject.GetComponent<Interaction>();
        }

        // 상호작용 중 업그레이드 기능이 있는 오브젝트냐
        if (InteractionObject.GetComponent<IUpgrade>() is not null)
        {
            upgrade = InteractionObject.GetComponent<IUpgrade>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 상호작용 가능한 오브젝트(유저) 이면서 웨이브(전투) 시작 전일때
        if(other.gameObject.CompareTag("Player") && !GameManager.instance.IsStartWave)
        {
            // GameManager.instance 에게 UI 보이기
            upgrade?.GetMission();

            // 이 오브젝트의 상호작용 함수 추가
            if (other.gameObject.GetComponent<PlayerCointroller>() is not null)
            {
                other.gameObject.GetComponent<PlayerCointroller>().interactEvent?.RemoveAllListeners();
                other.gameObject.GetComponent<PlayerCointroller>().interactEvent.AddListener(interaction.InteractAction);
            }

            // 건물이 아닌 상호작용일 경우 Spot이 없어서 예외처리
            if (spot != null) spot.SetActive(true);
        }
    }


    private void OnTriggerExit(Collider other)
    {
        // 상호작용 가능한 오브젝트(유저) 이면서 웨이브(전투) 시작 전일때
        if (other.gameObject.CompareTag("Player") && !GameManager.instance.IsStartWave)
        {
            // GameManager.instance 에게 UI 안보이기
            Debug.Log("Close");

            // 이 오브젝트의 상호작용 함수 제거
            other.gameObject.GetComponent<PlayerCointroller>().interactEvent?.RemoveAllListeners();

            // 게임 매니저의 시작으로 변경
            other.gameObject.GetComponent<PlayerCointroller>().interactEvent.AddListener(GameManager.instance.HoldingSpace);
            
            // 건물이 아닌 상호작용일 경우 Spot이 없어서 예외처리
            if (spot != null) spot.SetActive(false);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, transform.GetComponent<SphereCollider>().radius);
    }
}

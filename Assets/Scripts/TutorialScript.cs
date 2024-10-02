using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class TutorialScript : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Animator animator;

    [TextArea(3, 5)]
    [SerializeField] string[] comments;      // 이동, 달기기, 건물 상호작용, 스페이스 바로 웨이브 진행
    [SerializeField] int currentComment;

    private StringBuilder sb;

    private void Awake()
    {
        sb = new StringBuilder();
    }

    private void Start()
    {
        ShowCommnet(0);
    }

    private void Update()
    {
        if (GameManager.instance.BuildNexus && currentComment < comments.Length) ShowCommnet(3);
    }

    public void ShowCommnet(int n)
    {
        sb.Clear();
        animator.SetTrigger("DownTrigger");

        sb.Append($"{comments[n]}");
        text.SetText(sb);
    }

    private void OnDisable()
    {
        animator.SetTrigger("UpTrigger");
    }

}

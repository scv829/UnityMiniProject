using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUpgrade
{
    void GetMission();   // 업그레이드 조건
    void Upgrade();      // 업그레이드 실행
}

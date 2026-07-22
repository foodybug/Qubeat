using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Briefing : MonoBehaviour
{
    IEnumerator Start()
    {
        yield return new WaitForSeconds(2f);

        MainFlow.Instance.SetCurStageIdx(1);
        MainFlow.Instance.SceneConversion("1");
    }
}

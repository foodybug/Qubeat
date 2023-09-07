using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSetter_Linear : MonoBehaviour
{
    public static StageSetter_Linear instance;

    public const int stageCount = 100;

    [SerializeField] Animation animStage;

    [SerializeField] List<AnimationCurve> curveRoamer = new List<AnimationCurve>();
    [SerializeField] List<AnimationCurve> curveStone = new List<AnimationCurve>();
    [SerializeField] List<AnimationCurve> curveStalker = new List<AnimationCurve>();
    [SerializeField] List<AnimationCurve> curveBullet = new List<AnimationCurve>();

    private void Awake()
    {
        instance = this;

        //animStage.Play();
    }

    //public List<YCreationData> GetStageData(int index)
    //{
    //    animStage.clip.frameRate = index;
    //    //animStage.Play();

    //    Debug.Log("StageSetter:: GetStageData: index = " + index + ", cntRoamer = " + cntRoamer);

    //    List<YCreationData> list = new List<YCreationData>();
    //    for (int i = 0; i < cntRoamer; ++i)
    //    {
    //        Vector3 pos = YStageManager.Instance.GetRandomPlane2DPosInStage_ExceptPlayerPos();
    //        list.Add(new CreationData_Roamer(pos, 2));
    //    }

    //    return list;
    //}
    public List<YCreationData> GetStageData(int index)
    {
        List<YCreationData> list = new List<YCreationData>();
        for(int i=0; i<curveRoamer.Count; ++i)
        {
            float v = curveRoamer[i].Evaluate(index * 0.01f);
            int c = (int)(v * 100f);
            for(int j=0; j<c; ++j)
            {
                Vector3 pos = YStageManager.Instance.GetRandomPlane2DPosInStage_ExceptPlayerPos();
                list.Add(new CreationData_Roamer(pos, i + 1));
            }
        }
        for (int i = 0; i < curveStone.Count; ++i)
        {
            float v = curveStone[i].Evaluate(index * 0.01f);
            int c = (int)(v * 100f);
            for (int j = 0; j < c; ++j)
            {
                Vector3 pos = YStageManager.Instance.GetRandomPlane2DPosInStage_ExceptPlayerPos();
                list.Add(new CreationData_Stone(pos, i + 1));
            }
        }
        for (int i = 0; i < curveStalker.Count; ++i)
        {
            float v = curveStalker[i].Evaluate(index * 0.01f);
            int c = (int)(v * 100f);
            for (int j = 0; j < c; ++j)
            {
                Vector3 pos = YStageManager.Instance.GetRandomPlane2DPosInStage_ExceptPlayerPos();
                list.Add(new CreationData_Stalker(pos, i + 1));
            }
        }
        for (int i = 0; i < curveBullet.Count; ++i)
        {
            float v = curveBullet[i].Evaluate(index * 0.01f);
            int c = (int)(v * 100f);
            for (int j = 0; j < c; ++j)
            {
                Vector3 pos = YStageManager.Instance.GetRandomPlane2DPosInStage_ExceptPlayerPos();
                list.Add(new CreationData_Bullet(pos, i + 1));
            }
        }

        return list;
    }

    //int stageIndex = -1;
    //public void SetStageIndex(int stageIndex)
    //{
    //    this.stageIndex = stageIndex;
    //}
    //public IEnumerator GetStageData(List<YCreationData> list)
    //{
    //    //animStage.clip.frameRate = stageIndex;

    //    yield return null;

    //    Debug.Log("StageSetter:: GetStageData: index = " + stageIndex + ", cntRoamer = " + cntRoamer);

    //    list = new List<YCreationData>();
    //    for (int i = 0; i < cntRoamer; ++i)
    //    {
    //        Vector3 pos = YStageManager.Instance.GetRandomPlane2DPosInStage_ExceptPlayerPos();
    //        list.Add(new CreationData_Roamer(pos, 2));
    //    }
    //}
}

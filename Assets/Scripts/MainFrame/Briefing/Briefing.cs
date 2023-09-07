using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Firebase.Database;
using System.Threading.Tasks;

public class Briefing : MonoBehaviour
{
    [SerializeField] string strRankReceived = "Your Ranking is \n{0}";
    [SerializeField] string strRankNotReceived = "Ranking is not Found";
    [SerializeField] string strGoNext = "Ready\n to\n Crash!";

    [SerializeField] Text txtRank;
    [SerializeField] Text txtGoNext;

    [SerializeField] float waitTimeUntilReceiveRank = 2f;
    [SerializeField] float waitTimeUntilGoNext = 1f;

    int curRank = -1;
    bool rankReceived = false;

    private void Awake()
    {
        _Init();
    }

    void _Init()
    {
        txtRank.text = "Checking your ranking..";
        txtGoNext.text = strGoNext;
        txtGoNext.gameObject.SetActive(false);

        curRank = -1;
    }

    //IEnumerator Start()
    //{
    //    CheckRank();

    //    yield return new WaitForSeconds(waitTimeUntilReceiveRank);

    //    if(rankReceived == true)
    //    {
    //        txtRank.text = string.Format(strRankReceived, curRank);
    //    }
    //    else
    //    {
    //        txtRank.text = strRankNotReceived;
    //    }

    //    yield return new WaitForSeconds(waitTimeUntilGoNext);

    //    txtRank.gameObject.SetActive(false);
    //    txtGoNext.gameObject.SetActive(true);

    //    MainFlow.Instance.SetCurStageIdx(1);
    //    MainFlow.Instance.SceneConversion("1");
    //}

    void Start()
    {
        _Start();
    }

    async void _Start()
    {
        CheckRank();

        await Task.Delay((int)(waitTimeUntilReceiveRank * 1000f));

        if (rankReceived == true)
        {
            txtRank.text = string.Format(strRankReceived, curRank);
        }
        else
        {
            txtRank.text = strRankNotReceived;
        }

        await Task.Delay((int)(waitTimeUntilGoNext * 1000f));

        txtRank.gameObject.SetActive(false);
        txtGoNext.gameObject.SetActive(true);

        MainFlow.Instance.SetCurStageIdx(1);
        MainFlow.Instance.SceneConversion("1");
    }

    async void CheckRank()
    {
        rankReceived = false;

        DatabaseReference r = FirebaseDatabase.DefaultInstance.RootReference;

        string uuid = SystemInfo.deviceUniqueIdentifier;

        //var t = await r.OrderByChild("completeStage").GetValueAsync();
        var dss = await r.OrderByChild("completeStage").GetValueAsync();
        //DataSnapshot dss = t.Result;
        Debug.Log("dss.ChildrenCount = " + dss.ChildrenCount);

        foreach (DataSnapshot node in dss.Children)
        {
            Debug.Log("key = " + node.Key);

            IDictionary info = node.Value as IDictionary;
            if (info == null)
            {
                Debug.LogWarning("info is not IDictionary");
                continue;
            }

            //if(info.Contains("uuid") == true && info["uuid"].ToString() == uuid)
            //{
            //    int completeStage = (int)info["completeStage"];
            //    dss.ChildrenCount
            //}

            //foreach (KeyValuePair<object, object> node2 in info)
            //{
            //    Debug.Log("key = " + node2.Key + ", value = " + node2.Value);
            //}

            //foreach(object node2 in info.Keys)
            //{
            //    Debug.Log("key = " + node2.ToString());
            //}
            //foreach (object node2 in info.Values)
            //{
            //    Debug.Log("value = " + node2.ToString());
            //}

            if (info.Contains("name") == true && info.Contains("completeStage") == true)
                Debug.Log("name:" + info["name"] + ", completeStage:" + info["completeStage"]);
            else
                Debug.LogWarning("containing name = " + info.Contains("name") + ", containing completeStage = " + info.Contains("completeStage"));
        }

        rankReceived = true;

        //r.Child("rank").Child(uuid).GetValueAsync().ContinueWith(task =>
        //{
        //    if (task.IsFaulted)
        //        Debug.LogError("ERROR");
        //    else if (task.IsCompleted)
        //    {
        //        DataSnapshot dss = task.Result;
        //        Debug.Log("dss.ChildrenCount = " + dss.ChildrenCount);

        //        foreach (DataSnapshot node in dss.Children)
        //        {
        //            Debug.Log("key = " + node.Key);

        //            IDictionary info = node.Value as IDictionary;
        //            if (info == null)
        //            {
        //                Debug.LogWarning("info is not IDictionary");
        //                continue;
        //            }
        //            if (info.Contains("name") == true && info.Contains("v") == true)
        //                Debug.Log("name:" + info["name"] + ", v:" + info["v"]);
        //            else
        //                Debug.LogWarning("containing name = " + info.Contains("name") + ", containing v = " + info.Contains("v"));
        //        }

        //        rankReceived = true;
        //    }
        //});
    }
}

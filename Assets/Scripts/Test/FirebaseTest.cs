using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Firebase;
using Firebase.Database;
//using Firebase.Unity;
using System.Threading;
using System.Threading.Tasks;

public class FirebaseTest : MonoBehaviour
{
    enum eMode { Save, Load, Clear, Test }
    [SerializeField] eMode mode = eMode.Save;

    [SerializeField] TestInfo info;

    [SerializeField] bool save = false;
    [SerializeField] bool load = false;
    [Range(0, 100)][SerializeField] int loadID = 0;

    [Tooltip("쓰레드 로그")]
    [SerializeField] List<string> log = new List<string>();

    #region - test -
    bool goNext = true;
    IEnumerator Coding()
    {
        while (true)
        {
            string log = "number: ";
            List<int> list = new List<int>();
            for (int i = 0; i < 10; ++i)
            {
                int v = UnityEngine.Random.Range(-3, 3);
                list.Add(v);
                log += v + ", ";
            }
            //Debug.Log(log);

            int[] ar = list.ToArray();
            //int[] ar = new int[] { 1,2,3,0,3 };

            int[] foods = ar;

            int answer = 0;
            int[] satisfying = new int[3] { 0, 0, 0 };
            string log1 = "";
            string log2 = "";
            string log3 = "";

            for (int i = 0; i < foods.Length - 2; ++i)
            {
                for (int j = i + 1; j < foods.Length - 1; ++j)
                {
                    //if (i >= j || j >= foods.Length - 1)
                    //    continue;

                    //for(int k=j+1; k<foods.Length; ++k)
                    //{
                    for (int m = 0; m <= i; ++m)
                    {
                        satisfying[0] += foods[m];
                        log1 += foods[m] + ", ";
                    }
                    //Debug.Log("[i:" + i + "]satisfying[0] = " + satisfying[0]);
                    //Console.WriteLine("[i:" + i + "]satisfying[0] = " + satisfying[0]);
                    for (int m = i + 1; m <= j; ++m)
                    {
                        satisfying[1] += foods[m];
                        log2 += foods[m] + ", ";
                    }
                    //Debug.Log("[j:" + j + "]satisfying[1] = " + satisfying[1]);
                    //Console.WriteLine("[j:" + j + "]satisfying[1] = " + satisfying[1]);
                    for (int m = j + 1; m < foods.Length; ++m)
                    {
                        satisfying[2] += foods[m];
                        log3 += foods[m] + ", ";
                    }
                    //Debug.Log("===[rest]satisfying[2] = " + satisfying[2]);
                    //Console.WriteLine("[rest]satisfying[2] = " + satisfying[2]);


                    if (satisfying[0] == satisfying[1] && satisfying[1] == satisfying[2])
                    {
                        Debug.Log(log);
                        Debug.Log("satisfying1 = " + log1);
                        Debug.Log("satisfying2 = " + log2);
                        Debug.Log("satisfying3 = " + log3);
                        Debug.Log("-------------- found! satisfying[0] = " + satisfying[0] + ", satisfying[1] = " + satisfying[1] + ", satisfying[2] = " + satisfying[2] +
                            ", i = " + i + ", j = " + j);// + ", k = " + k);
                                                         //Console.WriteLine("found! satisfying = " + satisfying[0] +
                                                         //    ", i = " + i + ", j = " + j);// + ", k = " + k);
                        ++answer;
                    }
                    else
                    {
                        //Debug.Log("result: satisfying[0] = " + satisfying[0] + "satisfying[1] = " + satisfying[1] + "satisfying[2] = " + satisfying[2] +
                        //    ", i = " + i + ", j = " + j);
                    }

                    yield return null;

                    for (int n = 0; n < 3; ++n)
                        satisfying[n] = 0;

                    log1 = "";
                    log2 = "";
                    log3 = "";
                    //}
                }
            }

            if (answer == 0)
                continue;
            else
            {
                Debug.Log("answer is " + answer);
                //answer = 0;
                //goNext = false;
                //yield return new WaitWhile(() => goNext == false);
            }
        }
        
        //return answer;
    }

    async Task _TaskTest()
    {
        Task<int> t = TaskTest();

        for (int i = 0; i < 10; i++)
        {
            Debug.Log("Do Something Before TaskTest");
        }

        int v = await t;

        for (int i = 0; i < 10; i++)
        {
            Debug.Log("Do Something after TaskTest");
        }

        Console.ReadLine();
    }

    async Task<int> TaskTest()
    {
        Debug.Log("--------------- TaskTest Begin -----------------");

        await Task.Delay(1000);

        for (int i = 0; i < 10; i++)
        {
            Debug.Log("--------------- Do Something in TaskTest -----------------");
        }

        await Task.Delay(10);

        Debug.Log("--------------- TaskTest 1 -----------------");

        Task.Delay(10);

        Debug.Log("--------------- TaskTest 2 -----------------");

        return 1;
    }
    #endregion

    void Start()
    {
        //StartCoroutine(Coding());
        //_TaskTest();

        _DBProc();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("Cancel key input. result = " + m_Task.Result);
            m_CancelTokenSource.Cancel(); // ④ CancellationTokenSource의 Cancel() 메서드를 호출해 작업 취소 
            Debug.Log("Cancel call. result = " + m_Task.Result);
            if (m_Task != null)
            {
                Debug.Log("Count : " + m_Task.Result);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) == true)
            goNext = true;
    }

    async void _DBProc()
    {
        DatabaseReference r = FirebaseDatabase.DefaultInstance.RootReference;

        switch (mode)
        {
            case eMode.Test:
                Test();
                break;
            case eMode.Save:
                info.uuid = SystemInfo.deviceUniqueIdentifier;//초기화

                for (int i = 0; i < 100; ++i)
                {
                    string json = JsonUtility.ToJson(info);
                    Debug.Log("json = " + json);
                    await r.Child(info.name).SetRawJsonValueAsync(json);
                    if (UnityEngine.Random.Range(0, 3) == 0)
                    {
                        await r.Child(info.name).Child("child").SetRawJsonValueAsync(json);
                    }

                    Debug.Log("Completed info.name = " + info.name);

                    info.Random();
                }

                Debug.Log("Save ends");
                break;
            case eMode.Load:
                Query d = r.OrderByChild("v").StartAt(900);
                await d.GetValueAsync().ContinueWith(task =>
                {
                    DataSnapshot dss = task.Result;
                    Debug.Log(dss.ChildrenCount);

                    foreach (DataSnapshot node in dss.Children)
                    {
                        //Debug.Log("key = " + node.Key);

                        IDictionary info = node.Value as IDictionary;
                        if (info == null)
                        {
                            Debug.LogWarning("info is not IDictionary");
                            continue;
                        }

                        if (info.Contains("name") == true && info.Contains("v") == true)
                            Debug.Log("name:" + info["name"] + ", v:" + info["v"]);
                        else
                            Debug.LogWarning("containing name = " + info.Contains("name") + ", containing v = " + info.Contains("v"));
                    }
                });
                break;
            case eMode.Clear:
                await r.RemoveValueAsync();
                Debug.Log("Clear finished");
                break;
        }
    }

    CancellationTokenSource m_CancelTokenSource; // ① CancellationTokenSource 클래스 필드에서 선언  
    //Task<int> m_Task;
    Task<string> m_Task;
    void Test()
    {
        m_CancelTokenSource = new CancellationTokenSource();  // ② CancellationTokenSource 객체를 생성 
        CancellationToken cancellationToken = m_CancelTokenSource.Token;
        m_Task = Task.Factory.StartNew(TaskMethod, cancellationToken);

        //TaskMethod();
    }

    private string TaskMethod()
    {
        int count = 0;
        for (int i = 0; i < 50; i++)
        {
            if (m_CancelTokenSource.Token.IsCancellationRequested) // ③ 비동기 작업 메서드 안에서 작업이 취소되었는지를 체크 
            {
                log.Add("Cancellation. i = " + i);
                Debug.Log("Cancellation. i = " + i);
                //break;
            }

            ++count;
            Thread.Sleep(100);
            //Task.Delay(200);
        }
        //return count;
        return count.ToString();
    }

    [Serializable]
    class TestInfo
    {
        const string strAscii = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public string name;
        public string uuid;
        public int completeStage;

        public void Random()
        {
            name = "user" + UnityEngine.Random.Range(0, 100);
            uuid = _SetRandomAscii(3) + UnityEngine.Random.Range(0, 10000);
            completeStage = UnityEngine.Random.Range(0, 1000);
        }

        string _SetRandomAscii(int cnt)
        {
            string r = "";
            for(int i=0; i<cnt; ++i)
            {
                r += strAscii[UnityEngine.Random.Range(0, strAscii.Length)];
            }

            return r;
        }
    }
}

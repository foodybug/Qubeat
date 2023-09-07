using UnityEngine;
using System.Collections;

public class QLDisplay : MonoBehaviour {
    
    [SerializeField] Material matDefault;
    [SerializeField] UnityEngine.UI.Text selectEpisode;
    [SerializeField] UnityEngine.UI.Text completeEpisode;
    [SerializeField] float moveSpeed = 1f;
    [SerializeField] float sleepTimePerOneMove = 0.5f;
    [SerializeField] int minPlayerDistanceFromGoal = 5;
    [SerializeField] int maxEnemyDistanceFromGoal = 3;
    [SerializeField] float restartTime = 2f;
    float totalElapsedTime = 0f;
    //int curEpisode = 0;
    readonly string strTesting = "Current Episode : ";
    //readonly string strLblEpisode = "Episode : ";

    GameObject[,] grid;

    GameObject player;
    GameObject enemy;
    GameObject goal;

    RectPos playerPos; RectPos playerDest;
    RectPos enemyPos; RectPos enemyDest;
    RectPos goalPos;

    QLProtoType ql;
    const int oneGrid = 8;

    [SerializeField] UnityEngine.UI.Text txtResult;

    IEnumerator Start()
    {
        yield return StartCoroutine("_WaitForQTable_CR");

        _Init_Grid();

        completeEpisode.text = "Started";

        OnButtonUp();
    }

    IEnumerator _WaitForQTable_CR()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            ql = GetComponent<QLProtoType>();
            if (ql == null)
                Debug.LogError("QL class is not found");
            else //if(ql.episodeIndex > 0)
                break;
        }
    }

    IEnumerator _Move_CR()
    {
        yield return null;

        float beginTime = Time.time;

        playerDest = playerPos;
        
        Vector3 vPlayerDest;
        Vector3 vEnemyDest;
        while (true)
        {
            if (ql.GetAction(playerPos, enemyPos, goalPos, ref playerDest, ref enemyDest))
            {
                vPlayerDest = grid[playerDest.x, playerDest.y].transform.position;
                vEnemyDest = grid[enemyDest.x, enemyDest.y].transform.position;

                float ratio = 0f;
                while (true)
                {
                    yield return null;

                    player.transform.position = Vector3.Lerp(player.transform.position, vPlayerDest, ratio);
                    enemy.transform.position = Vector3.Lerp(enemy.transform.position, vEnemyDest, ratio);

                    ratio += Time.deltaTime * moveSpeed;

                    if (ratio > 1f)
                        break;
                }

                playerPos = playerDest;
                enemyPos = enemyDest;

                if (playerPos == goalPos)
                {
                    txtResult.text = "Goal";
                    txtResult.gameObject.SetActive(true);
                    break;
                }
                else if (playerPos == enemyPos)
                {
                    txtResult.text = "Fail";
                    txtResult.gameObject.SetActive(true);
                    break;
                }
            }
            else
                continue;

            yield return new WaitForSeconds(sleepTimePerOneMove);
        }

        float elapsedTime = Time.time - beginTime;
        totalElapsedTime += elapsedTime;
        completeEpisode.text = txtResult.text + " : time lapse = " + elapsedTime.ToString("f1") + ", total time = " + totalElapsedTime.ToString("f1");
        //Debug.Log(txtResult.text + " : time lapse = " + (Time.time - beginTime).ToString("f1") + ", loop = " + ql.loopCount);
        yield return new WaitForSeconds(restartTime);
        OnButtonUp();
    }

    void _Init_Grid()
    {
        grid = new GameObject[QLProtoType.oneGrid, QLProtoType.oneGrid];

        for (int i=0; i< QLProtoType.oneGrid; ++i)
        {
            for (int j = 0; j < QLProtoType.oneGrid; ++j)
            {
                GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                obj.name = i + ", " + j;
                obj.transform.localScale = new Vector3(0.9f, 0.9f, 0.1f);
                obj.transform.SetParent(transform);
                Renderer renderer = obj.GetComponent<Renderer>();
                renderer.material = _GetNewDefaultMaterial(Color.white);
                //obj.transform.position = new Vector3(i - QLProtoType.oneGrid * 0.5f, j - QLProtoType.oneGrid * 0.5f, -0.1f);
                obj.transform.position = new Vector3(i, -j, -0.1f);

                if (ql.CheckGrid_Obstacle(i, j))
                    renderer.material.color = Color.black;

                grid[i, j] = obj;
            }
        }
    }

    void _Init_Entity()
    {
        playerPos = RectPos.GetRandom(ql);
        int count = 0;
        while (true)
        {
            if (count++ > 999)
            {
                Debug.LogError("invalid positions");
                break;
            }

            goalPos = RectPos.GetRandom(ql);
            if (goalPos.GetDistance(playerPos) < minPlayerDistanceFromGoal)
                continue;
            else
                break;
        }

        count = 0;
        while (true)
        {
            if (count++ > 999)
            {
                Debug.LogError("invalid positions");
                break;
            }

            enemyPos = RectPos.GetRandom(ql);
            if (goalPos == enemyPos || goalPos.GetDistance(enemyPos) > maxEnemyDistanceFromGoal)
                continue;
            else
                break;
        }

        if (player != null) Destroy(player);
        if (enemy != null) Destroy(enemy);
        if (goal != null) Destroy(goal);

        player = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        player.transform.localScale *= 0.5f;
        Renderer playerRenderer = player.GetComponent<Renderer>();
        playerRenderer.material = _GetNewDefaultMaterial(Color.green);
        player.transform.position = grid[playerPos.x, playerPos.y].transform.position;

        enemy = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Renderer enemyRenderer = enemy.GetComponent<Renderer>();
        enemyRenderer.material = _GetNewDefaultMaterial(Color.red);
        enemy.transform.position = grid[enemyPos.x, enemyPos.y].transform.position;

        goal = GameObject.CreatePrimitive(PrimitiveType.Cube);
        goal.transform.localScale *= 0.7f;
        Renderer goalRenderer = goal.GetComponent<Renderer>();
        goalRenderer.material = _GetNewDefaultMaterial(Color.blue);
        goal.transform.position = grid[goalPos.x, goalPos.y].transform.position;
    }

    //Vector3

    Material _GetNewDefaultMaterial(Color color)
    {
        Material newMat = new Material(matDefault);
        newMat.color = color;
        return newMat;
    }

    void Update()
    {
        //if (Input.GetKeyUp(KeyCode.LeftArrow))
        //{
        //    if (curEpisode > 1)
        //    {
        //        curEpisode--;
        //        selectEpisode.text = strTesting + curEpisode;
        //    }
        //}
        //if (Input.GetKeyUp(KeyCode.RightArrow))
        //{
        //    if (ql != null && curEpisode < ql.episodeIndex)
        //    {
        //        curEpisode++;   
        //        selectEpisode.text = strTesting + curEpisode;
        //    }
        //}
    }

    public void OneEpisodeEnd()
    {
        //if (ql != null)
        //    completeEpisode.text = "Episode [" + ql.episodeIndex + "] complete";
    }

    public void OneLoopEnd()
    {
        if (ql != null)
            completeEpisode.text = "Loop [" + ql.loopCount + "] complete";
    }

    public void OnButtonUp()
    {
        if (ql == null || grid == null)// || ql.episodeIndex == 0)
            return;

        _Init_Entity();

        txtResult.gameObject.SetActive(false);

        //selectEpisode.text = strTesting + ql.episodeIndex;

        StopCoroutine("_Move_CR");
        StartCoroutine("_Move_CR");
    }
}

public struct RectPos
{
    public int x;
    public int y;

    public RectPos(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public static bool operator ==(RectPos c1, RectPos c2)
    {
        return c1.x == c2.x && c1.y == c2.y;
    }

    public static bool operator !=(RectPos c1, RectPos c2)
    {
        return c1.x != c2.x || c1.y != c2.y;
    }

    public Vector3 vector3
    {
        get
        {
            return new Vector3((float)x, (float)y, 0f);
        }
    }

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public int GetDistance(RectPos pos)
    {
        return Mathf.Abs(x - pos.x) + Mathf.Abs(y - pos.y);
    }

    public static RectPos GetRandom(QLProtoType ql)
    {
        if (ql == null)
        {
            Debug.LogWarning("QLProtoType is not initialized. set [0, 0] value");
            return new RectPos(0, 0);
        }
        else
        {
            while (true)
            {
                RectPos v = new RectPos(Random.Range(0, QLProtoType.oneGrid), Random.Range(0, QLProtoType.oneGrid));
                if (!ql.CheckGrid_Obstacle(v.x, v.y))
                    return v;
            }
        }
    }
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 8x8 grid

//          0:up
//  1:left          2:right
//          3:down

public enum eDirection {Up, Left, Right, Down }

public class QLProtoType : MonoBehaviour
{
    #region - member -
    // constant parameters
    public const int oneGrid = 8;
    public const int maxGrid = 64;
    const int maxAction = 4;
    readonly int playerStateSize = maxGrid;
    readonly int goalStateSize = maxGrid;
    readonly byte monsterStateSize = maxGrid;

    
    readonly int maxLoopPerEpisode = 20;
    readonly float alpha = 0.1f;
    readonly float gamma = 0.9f;

    public float[,,,] qTable { get; internal set; }// [player, monster, goal, action]
    float[,,,] innerQTable;
    float[,,] reward;// [player, monster, goal]

    Dictionary<int, int> dicObstacle = new Dictionary<int, int>();

    [SerializeField] int maxObtacle = 10;
    [SerializeField] float enemyMoveProb = 100f;
    public int episodeIndex { get; internal set; }
    public int loopCount { get; internal set; }
    #endregion
    #region - init -
    void Awake()
    {
        episodeIndex = 0;
        loopCount = 0;
    }

    void Start()
    {
        qTable = innerQTable = new float[maxGrid, maxGrid, maxGrid, maxAction];

        for (int i = 0; i < maxObtacle; ++i)
        {
            int v = Random.Range(0, maxGrid);
            if (!dicObstacle.ContainsKey(v))
                dicObstacle.Add(v, v);
            else
                --i;
        }

        InitState();

        Episode();
    }

    void InitState()
    {
        reward = new float[maxGrid, maxGrid, maxGrid];
        for (int i = 0; i < maxGrid; ++i)//player
        {
            for (int j = 0; j < maxGrid; ++j)
            {
                for (int k = 0; k < maxGrid; ++k)//goal
                {
                    if (i == k && !dicObstacle.ContainsKey(i))//reach at goal
                    {
                        reward[i, j, k] = 1f;
                    }
                }
            }
        }
    }

    void Episode()
    {
        StartCoroutine("Episode_CR");
    }

    IEnumerator Episode_CR()
    {
        float beganTime = Time.time;
        Debug.Log("Episode_CR begin");
        
        while (true)
        {
            loopCount = 0;

            for (int i = 0; i < maxGrid; ++i)
            {
                for (int j = 0; j < maxGrid; ++j)
                {
                    yield return null;

                    for (int k = 0; k < maxGrid; ++k)
                    {
                        int curPlayer = k; int curMonster = j; int curGoal = i;
                        int maxLoop = 10;
                        
                        while (true)
                        {
                            if (maxLoop-- <= 0)
                                break;

                            if (curPlayer == curMonster || curPlayer == curGoal)
                                break;

                            int action = Random.Range(0, maxAction);
                            int nextPosition = _GetNextPosition(curPlayer, action);
                            int nextMonster = _GetNextMonsterPosition(nextPosition, curMonster);// consider current player position
                            if (_CheckDirectionEnable(nextPosition, curMonster, nextMonster))
                            {
                                int nextPlayer = nextPosition;
                                int nextGoal = i;

                                float r = reward[nextPlayer, nextMonster, nextGoal];
                                float nextMaxValue = _GetMaxQValue(nextPlayer, nextMonster, nextGoal);
                                innerQTable[curPlayer, curMonster, curGoal, action] = (r + gamma * nextMaxValue);

                                curPlayer = nextPlayer; curMonster = nextMonster; curGoal = nextGoal;
                            }
                        }

                        loopCount++;
                        //QLDisplay display = GetComponent<QLDisplay>();
                        //if (display != null)
                        //    display.OneLoopEnd();
                    }
                }
            }

            //Debug.Log("episode ends. elapsed time = " + (Time.time - beganTime));
            //QLDisplay display = GetComponent<QLDisplay>();
            //if (display != null)
            //    display.OneEpisodeEnd();

            //qTable = innerQTable.Clone() as float[,,,];
            episodeIndex++;
        }
    }

    int _GetNextPosition(int curPosition, int action)
    {
        int nextPosition = -2;
        switch (action)
        {
            case 0:
                nextPosition = curPosition - oneGrid;
                if (nextPosition < 0)
                    nextPosition = -1;
                break;
            case 1:
                if (curPosition % oneGrid == 0)
                    nextPosition = -1;
                else
                    nextPosition = curPosition - 1;
                break;
            case 2:
                if (curPosition % oneGrid == oneGrid - 1)
                    nextPosition = -1;
                else
                    nextPosition = curPosition + 1;
                break;
            case 3:
                nextPosition = curPosition + oneGrid;
                if (nextPosition >= maxGrid)
                    nextPosition = -1;
                break;
        }

        if (nextPosition == -2 && action != -1)
        {
            Debug.LogError("invalid action result(nextPosition = -2). curPosition = " +
                curPosition + ", action = " + action + ", nextPosition = " + nextPosition);
        }

        return nextPosition;
    }

    bool _CheckDirectionEnable(int nextPosition, int curMonster, int nextMonster)
    {
        if (nextPosition == curMonster || nextPosition == nextMonster)
            return false;

        if (nextPosition < 0)
            return false;

        if (dicObstacle.ContainsKey(nextPosition))
            return false;

        return true;
    }

    int _GetNextMonsterPosition(int curPlayer, int curMonster)
    {
        if (Random.Range(0f, 100f) < enemyMoveProb)
        {
            int playerPosX, playerPosY, monsterPosX, monsterPosY;
            __GetPositionAs2Dimension(curPlayer, out playerPosX, out playerPosY);
            __GetPositionAs2Dimension(curMonster, out monsterPosX, out monsterPosY);

            int dir = __GetDirection(monsterPosX, monsterPosY, playerPosX, playerPosY);
            int next = _GetNextPosition(curMonster, dir);

            if (next < 0 || next >= maxGrid || dicObstacle.ContainsKey(next))
            {
                dir = __GetDirection(monsterPosX, monsterPosY, playerPosX, playerPosY, dir);
                next = _GetNextPosition(curMonster, dir);

                if (next < 0 || next >= maxGrid || dicObstacle.ContainsKey(next))
                    return curMonster;
            }

            return next;
        }
        else
            return curMonster;
    }

    int __GetDirection(int curX, int curY, int destX, int destY, int prevDir = -1)
    {
        int offsetX = destX - curX;
        int offsetY = destY - curY;

        int distX = Mathf.Abs(offsetX);
        int distY = Mathf.Abs(offsetY);

        bool flip = 50f < Random.Range(0f, 100f);

            //retry
        if (prevDir == 0 || prevDir == 3)
        {
            if (offsetX < 0)
                return 1;
            else if (offsetX > 0)
                return 2;
            else
                return flip ? 1 : 2;
        }
        else if (prevDir == 1 || prevDir == 2)
        {
            if (offsetY < 0)
                return 0;
            else if (offsetY > 0)
                return 3;
            else
                return flip ? 0 : 3;
        }

        //first try
        if (distX == distY)
        {
            if (50f < Random.Range(0f, 100f))
            {
                if (offsetY < 0)
                    return 0;
                else if (offsetY > 0)
                    return 3;
                else
                    return flip ? 0 : 3;
            }
            else
            {
                if (offsetX < 0)
                    return 1;
                else if (offsetX > 0)
                    return 2;
                else
                    return flip ? 1 : 2;
            }
        }
        else if (distX > distY)
        {
            if (offsetX < 0)
                return 1;
            else if (offsetX > 0)
                return 2;
            else
                return flip ? 1 : 2;
        }
        else if (distX < distY)
        {
            if (offsetY < 0)
                return 0;
            else if (offsetY > 0)
                return 3;
            else
                return flip ? 0 : 3;
        }

        return -1;
    }

    float _GetMaxQValue(int player, int monster, int goal)
    {
        float max = 0f;
        for (int i = 0; i < maxAction; ++i)
        {
            if (innerQTable[player, monster, goal, i] > max)
                max = innerQTable[player, monster, goal, i];
        }

        return max;
    }

    int _GetActionFromQValue(int player, int monster, int goal, bool playing = false)
    {
        int action = Random.Range(-1, maxAction);

        float[,,,] table = null;
        table = (playing == true ? qTable : innerQTable);
        float max = 0f;
        for (int i = 0; i < maxAction; ++i)
        {
            //int next = _GetNextPosition(player, i);
            //if (dicObstacle.ContainsKey(next))
            //    continue;

            if (table[player, monster, goal, i] > max)
            {
                max = table[player, monster, goal, i];
                action = i;
            }
        }

        return action;
    }
    #endregion
    #region - public -
    public bool GetAction(RectPos player2dPos, RectPos enemy2dPos, RectPos goal2dPos,
        ref RectPos playerPos, ref RectPos enemyPos)
    {
        int player1dPos = __GetPositionAs1Dimension(player2dPos.x, player2dPos.y);
        int enemy1dPos = __GetPositionAs1Dimension(enemy2dPos.x, enemy2dPos.y);
        int goal1dPos = __GetPositionAs1Dimension(goal2dPos.x, goal2dPos.y);
        
        int pAction = _GetActionFromQValue(player1dPos, enemy1dPos, goal1dPos, true);
        int pPos = _GetNextPosition(player1dPos, pAction);
        int ePos = _GetNextMonsterPosition(player1dPos, enemy1dPos);

        if (!_CheckDirectionEnable(pPos, enemy1dPos, ePos))
            pPos = player1dPos;

        if (pPos == -1 || ePos == -1)
            return false;

        __GetPositionAs2Dimension(pPos, out playerPos.x, out playerPos.y);
        __GetPositionAs2Dimension(ePos, out enemyPos.x, out enemyPos.y);

        return true;
    }

    public bool CheckGrid_Obstacle(int x, int y)
    {
        int obstaclePos = __GetPositionAs1Dimension(x, y);
        return dicObstacle.ContainsKey(obstaclePos);
    }
    #endregion
    #region - method -
    void __GetPositionAs2Dimension(int position, out int x, out int y)
    {
        x = -1; y = -1;

        x = position % oneGrid;
        y = position / oneGrid;
    }

    int __GetPositionAs1Dimension(int x, int y)
    {
        int pos = y* oneGrid +x;
        if (pos < 0 || pos >= maxGrid)
        {
            Debug.LogError("invalid position. x = " + x + ", y = " + y + ", result = " + pos);
            return -1;
        }

        return pos;
    }
    #endregion
}

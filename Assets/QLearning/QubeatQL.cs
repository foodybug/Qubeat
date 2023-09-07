using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//  enemy state
//  1 << 0,     1 << 1,     1 << 2,         1 << 3,     1 << 4,
//  1 << 5,     1 << 6,     1 << 7,         1 << 8,     1 << 9,
//  1 << 10,    1 << 11,    1 << 12:player, 1 << 13,    1 << 14,
//  1 << 15,    1 << 16,    1 << 17,        1 << 18,    1 << 19,
//  1 << 20,    1 << 21,    1 << 22,        1 << 23,    1 << 24

//  boss state 
//  12 is no boss exist
//  0,      1,      2,          3,      4,
//  5,      6,      7,          8,      9,
//  10,     11,     12:no boss, 13,     14,
//  15,     16,     17,         18,     19,
//  20,     21,     22,         23,     24

//  actions
//  6,      7,      8,
//  11,     player, 13,
//  16,     17,     18,     

public enum eAction
{
    UP_LEFT = 6, UP = 7, UP_RIGHT = 8,
    LEFT = 11, CENTER = 12, RIGHT = 13,
    DOWN_LEFT = 16, DOWN = 17, DOWN_RIGHT = 18, MAX = 8
}

class QubeatQL : MonoBehaviour
{
    // constant parameters
    readonly int stateArrayLength = 25;//5 x 5: 12 is no use
    readonly int bossState = 25;// include no boss exist
    readonly byte unitState = 1;// 0, 1//, 2, 3
    readonly int stateDigit = 1;

    readonly int maxLoopPerEpisode = 20;
    readonly float alpha = 0.1f;
    readonly float gamma = 0.9f;

    Dictionary<int, eAction> dicActionToGrid = new Dictionary<int, eAction>();
    Dictionary<int, int> dicActionToShift = new Dictionary<int, int>();
    int[] intToAction;

    int maximumStateNumber;
    const int maximumActionNumber = 8;

    byte[,,] states;// [enemy state, boss state, action]
    byte[,,] reward;// [enemy state, boss state, action]

    public byte[,,] qStates { get { return states; } }

    void Start()
    {
        byte[,,] test = new byte[33554431, 25, 8];

        //byte[][][] test = new byte[33554431][][];

        //for(int i=0; i<33554431; ++i)
        //{
        //    test[i] = new byte[25][];

        //    for (int j=0; j<25; ++j)
        //    {
        //        test[i][j] = new byte[8];

        //        for (int k=0; k<8; ++k)
        //        {
        //            test[i][j][k] = 0;
        //        }
        //    }
        //}

        //for(int i=0; i<10; ++i)
        //{
        //    int a = Random.Range(0, 33554431);
        //    int b = Random.Range(0, 25);
        //    int c = Random.Range(0, 8);

        //    byte result = test[a][b][c];
        //    Debug.Log(result);
        //}

        //InitParameters();

        //InitState();

        //Episode();
    }

    void InitParameters()
    {
        //- set maximumStateNumber
        for (int i = 0; i < stateArrayLength; ++i)
        {
            maximumStateNumber |= 1 << i;
        }

        //// while boss state is 6,7,8,   11,13   15,16,17 reward is max
        dicActionToGrid.Add(6, eAction.UP_LEFT);
        dicActionToGrid.Add(7, eAction.UP);
        dicActionToGrid.Add(8, eAction.UP_RIGHT);

        dicActionToGrid.Add(11, eAction.LEFT);
        dicActionToGrid.Add(13, eAction.RIGHT);

        dicActionToGrid.Add(16, eAction.DOWN_LEFT);
        dicActionToGrid.Add(17, eAction.DOWN);
        dicActionToGrid.Add(18, eAction.DOWN_RIGHT);


        intToAction = new int[(int)eAction.MAX];
        intToAction[0] = (int)eAction.UP_LEFT;
        intToAction[1] = (int)eAction.UP;
        intToAction[2] = (int)eAction.UP_RIGHT;

        intToAction[3] = (int)eAction.LEFT;
        intToAction[4] = (int)eAction.RIGHT;

        intToAction[5] = (int)eAction.DOWN_LEFT;
        intToAction[6] = (int)eAction.DOWN;
        intToAction[7] = (int)eAction.DOWN_RIGHT;


        dicActionToShift.Add((int)eAction.UP, 5);
        dicActionToShift.Add((int)eAction.DOWN, -5);
        dicActionToShift.Add((int)eAction.DOWN_RIGHT, -5);

        maxLearningRatio = maximumStateNumber * maximumActionNumber * maxLoopPerEpisode;
    }

    void InitState()
    {
        Debug.Log("maximumStateNumber = " + maximumStateNumber);

        states = new byte[maximumStateNumber, bossState, maximumActionNumber];
        reward = new byte[maximumStateNumber, bossState, maximumActionNumber];

        for (int i = 0; i < maximumStateNumber; ++i)
        {
            for (int j = 0; j < bossState; ++j)
            {
                for (int k = 0; k < maximumActionNumber; ++k)
                {
                    int action = intToAction[k];
                    if (action == k && _CheckDirectionEnable(maximumStateNumber, action))
                        reward[i, j, k] = byte.MaxValue;
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
        for (int i = 0; i < maximumStateNumber; ++i)
        {
            for (int j = 0; j < bossState; ++j)
            {
                int curEnemyState = i;
                int curBossState = j;
                int loop = 0;

                while (true)
                {
                    yield return null;

                    if (loop > maxLoopPerEpisode)
                        break;

                    ++loop;
                    learningRatio += 1f;

                    int intActionNumber = Random.Range(0, maximumActionNumber);
                    int action = intToAction[intActionNumber];
                    if (_CheckDirectionEnable(curEnemyState, action))
                    {
                        int nextEnemyState;
                        int nextBossState;
                        _GetRandomNextStateFromAction(curEnemyState, curBossState, action, out nextEnemyState, out nextBossState);

                        byte r = reward[curEnemyState, curBossState, intActionNumber];
                        states[curEnemyState, curBossState, intActionNumber] = (byte)(r + gamma * _GetMaxQValue(curEnemyState, curBossState));

                        curEnemyState = nextEnemyState;
                        curBossState = nextBossState;

                        //// Q(s,a)= Q(s,a) + alpha * (R(s,a) + gamma * Max(next state, all actions) - Q(s,a))
                        //Q[currentState ,possibleAction] = (int)(R[currentState, action] + (gamma * maximum(action, false)));
                    }
                    else
                        break;
                }
            }
        }
    }

    bool _CheckDirectionEnable(int state, int action)
    {
        int shift = action;
        int result = state & (1 << shift);

        // state 0 is safe, 1 is danger
        return result == 1 ? false : true;
    }

    ulong _GetStateNumber(ulong[] stateIndex)
    {
        ulong resultNumber = 0;
        for (int i = 0; i < stateArrayLength; ++i)
        {
            ulong state = stateIndex[i] << (stateDigit * i);
            resultNumber |= state;
        }

        return resultNumber;
    }

    void _GetNextStateFromAction(int enemyState, int bossState, int action, out int nextEnemyState, out int nextBossState)
    {
        switch ((eAction)action)
        {
            // up
            case eAction.UP_LEFT:
                nextEnemyState = enemyState << 5;
                for (int i = 1; i < 5; ++i)
                    nextEnemyState = nextEnemyState & ~(1 << (i * 5));

                nextBossState = bossState + 6;
                if (nextBossState % 5 == 0) nextBossState = 12;
                break;
            case eAction.UP:
                nextEnemyState = enemyState << 5;
                nextBossState = bossState + 5;
                break;
            case eAction.UP_RIGHT:
                nextEnemyState = enemyState << 5;
                for (int i = 1; i < 5; ++i)
                    nextEnemyState = nextEnemyState & ~(1 << (i * 5 + 4));

                nextBossState = bossState + 4;
                if (nextBossState % 5 == 4) nextBossState = 12;
                break;
            // horizontal    
            case eAction.LEFT:
                nextEnemyState = enemyState;
                for (int i = 0; i < 5; ++i)
                    nextEnemyState = nextEnemyState & ~(1 << (i * 5));

                nextBossState = bossState + 1;
                if (nextBossState % 5 == 0) nextBossState = 12;
                break;
            case eAction.RIGHT:
                nextEnemyState = enemyState;
                for (int i = 0; i < 5; ++i)
                    nextEnemyState = nextEnemyState & ~(1 << (i * 5 + 4));

                nextBossState = bossState - 1;
                if (nextBossState % 5 == 4) nextBossState = 12;
                break;
            // down
            case eAction.DOWN_LEFT:
                nextEnemyState = enemyState >> 5;
                for (int i = 0; i < 4; ++i)
                    nextEnemyState = nextEnemyState & ~(1 << (i * 5));

                nextBossState = bossState - 4;
                if (nextBossState % 5 == 0) nextBossState = 12;
                break;
            case eAction.DOWN:
                nextEnemyState = enemyState >> 5;
                nextBossState = bossState - 5;
                break;
            case eAction.DOWN_RIGHT:
                nextEnemyState = enemyState >> 5;
                for (int i = 0; i < 4; ++i)
                    nextEnemyState = nextEnemyState & ~(1 << (i * 5 + 4));

                nextBossState = bossState - 6;
                if (nextBossState % 5 == 4) nextBossState = 12;
                break;
            default:
                nextEnemyState = enemyState;
                nextBossState = bossState;
                break;
        }

        if (bossState < 0 || bossState > 24)// case boss not exist
            bossState = 12;
    }

    void _GetRandomNextStateFromAction(int enemyState, int bossState, int action, out int nextEnemyState, out int nextBossState)
    {
        switch (action)
        {
            // up
            case 6:
                nextEnemyState = enemyState << 5;
                for (int i = 1; i < 5; ++i)
                    nextEnemyState = nextEnemyState & ~(Random.Range(0, 2) << (i * 5));

                nextBossState = bossState + 6;
                if (nextBossState % 5 == 0) nextBossState = 12;
                break;
            case 7:
                nextEnemyState = enemyState << 5;
                for (int i = 0; i < 5; ++i)
                    nextEnemyState = nextEnemyState & ~(Random.Range(0, 2) << i);

                nextBossState = bossState + 5;
                break;
            case 8:
                nextEnemyState = enemyState << 5;
                for (int i = 1; i < 5; ++i)
                    nextEnemyState = nextEnemyState & ~(Random.Range(0, 2) << (i * 5 + 4));

                nextBossState = bossState + 4;
                if (nextBossState % 5 == 4) nextBossState = 12;
                break;
            // horizontal    
            case 11:
                nextEnemyState = enemyState;
                for (int i = 0; i < 5; ++i)
                    nextEnemyState = nextEnemyState & ~(Random.Range(0, 2) << (i * 5));

                nextBossState = bossState + 1;
                if (nextBossState % 5 == 0) nextBossState = 12;
                break;
            case 13:
                nextEnemyState = enemyState;
                for (int i = 0; i < 5; ++i)
                    nextEnemyState = nextEnemyState & ~(Random.Range(0, 2) << (i * 5 + 4));

                nextBossState = bossState - 1;
                if (nextBossState % 5 == 4) nextBossState = 12;
                break;
            // down
            case 16:
                nextEnemyState = enemyState >> 5;
                for (int i = 0; i < 4; ++i)
                    nextEnemyState = nextEnemyState & ~(Random.Range(0, 2) << (i * 5));

                nextBossState = bossState - 4;
                if (nextBossState % 5 == 0) nextBossState = 12;
                break;
            case 17:
                nextEnemyState = enemyState >> 5;
                for (int i = 20; i < 25; ++i)
                    nextEnemyState = nextEnemyState & ~(Random.Range(0, 2) << i);

                nextBossState = bossState - 5;
                break;
            case 18:
                nextEnemyState = enemyState >> 5;
                for (int i = 0; i < 4; ++i)
                    nextEnemyState = nextEnemyState & ~(Random.Range(0, 2) << (i * 5 + 4));

                nextBossState = bossState - 6;
                if (nextBossState % 5 == 4) nextBossState = 12;
                break;
            default:
                nextEnemyState = enemyState;
                nextBossState = bossState;
                break;
        }

        if (bossState < 0 || bossState > 24)// case boss not exist
            bossState = 12;
    }

    byte _GetMaxQValue(int enemyState, int bossState)
    {
        byte max = 0;
        for (int i = 0; i < maximumActionNumber; ++i)
        {
            if (states[enemyState, bossState, i] > max)
                max = states[enemyState, bossState, i];
        }

        return max;
    }

    ulong[] GenerateStateArray()
    {
        ulong[] ar = new ulong[stateArrayLength];
        ar[0] = 3;
        ar[1] = 1;
        ar[2] = 1;
        return ar;
    }

    float learningRatio = 0f;
    float maxLearningRatio = 0f;
    void OnGUI()
    {
        learningRatio = GUI.HorizontalSlider(new Rect(100, 100, 100, 10), learningRatio, 0f, maxLearningRatio);
    }

    public eAction GetDirection(int state, int boss)
    {
        return eAction.DOWN;
    }
}

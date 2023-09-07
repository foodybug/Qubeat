//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;

////  enemy state
////                          1 << 0
////             1 << 1,      1 << 2,         1 << 3,
////  1 << 4,    1 << 5,      1 << 6:player,  1 << 7,     1 << 8,
////             1 << 9,      1 << 10,        1 << 11,
////                          1 << 12

//// all state count : 8192

////  boss state 
////  6 is no boss exist
////                          0
////              1,          2,              3,
////  4,          5,          6:no boss,       7,          8,
////              9,          10,             11,
////                          12

////  actions
////          2,
////  5,      6:player, 7,
////          10,  

//public enum eAction
//{
//    UP = 2,
//    LEFT = 5, CENTER = 6, RIGHT = 7,
//    DOWN = 10,
//}

//class QubeatQL : MonoBehaviour
//{
//    // constant parameters
//    readonly int stateArrayLength = 13;//6 is no use
//    readonly int bossState = 13;// include no boss exist
//    readonly byte unitState = 1;// 0, 1//, 2, 3
//    readonly int stateDigit = 1;

//    readonly int maxLoopPerEpisode = 20;
//    readonly float alpha = 0.1f;
//    readonly float gamma = 0.9f;

//    Dictionary<int, eAction> dicActionToGrid = new Dictionary<int, eAction>();
//    Dictionary<int, int> dicActionToShift = new Dictionary<int, int>();
//    int[] intToAction;

//    int maximumStateNumber;
//    const int maximumActionNumber = 4;

//    byte[,,] states;// [enemy state, boss state, action]
//    byte[,,] reward;// [enemy state, boss state, action]

//    public byte[,,] qStates { get { return states; } }

//    void Start()
//    {
//        InitParameters();

//        InitState();

//        //Episode();
//    }

//    void InitParameters()
//    {
//        //- set maximumStateNumber
//        for (int i = 0; i < stateArrayLength; ++i)
//        {
//            maximumStateNumber |= 1 << i;
//        }

//        //// while boss state is 6,7,8,   11,13   15,16,17 reward is max
//        dicActionToGrid.Add(2, eAction.UP);

//        dicActionToGrid.Add(5, eAction.LEFT);
//        dicActionToGrid.Add(7, eAction.RIGHT);

//        dicActionToGrid.Add(10, eAction.DOWN);


//        intToAction = new int[maximumActionNumber];
//        intToAction[0] = (int)eAction.UP;
//        intToAction[1] = (int)eAction.LEFT;
//        intToAction[2] = (int)eAction.RIGHT;
//        intToAction[3] = (int)eAction.DOWN;


//        dicActionToShift.Add((int)eAction.UP, 5);
//        dicActionToShift.Add((int)eAction.DOWN, -5);

//        maxLearningRatio = maximumStateNumber * maximumActionNumber * maxLoopPerEpisode;
//    }

//    void InitState()
//    {
//        Debug.Log("maximumStateNumber = " + maximumStateNumber);

//        states = new byte[maximumStateNumber, bossState, maximumActionNumber];
//        reward = new byte[maximumStateNumber, bossState, maximumActionNumber];

//        for (int i = 0; i < maximumStateNumber; ++i)
//        {
//            for (int j = 0; j < bossState; ++j)
//            {
//                for (int k = 0; k < maximumActionNumber; ++k)
//                {
//                    int action = intToAction[k];
//                    if (action == j && _CheckDirectionEnable(i, action))
//                        reward[i, j, k] = byte.MaxValue;
//                }
//            }
//        }
//    }

//    void Episode()
//    {
//        StartCoroutine("Episode_CR");
//    }

//    IEnumerator Episode_CR()
//    {
//        for (int i = 0; i < maximumStateNumber; ++i)
//        {
//            for (int j = 0; j < bossState; ++j)
//            {
//                int curEnemyState = i;
//                int curBossState = j;
//                int loop = 0;

//                while (true)
//                {
//                    yield return null;

//                    if (loop > maxLoopPerEpisode)
//                        break;

//                    ++loop;
//                    learningRatio += 1f;

//                    int intActionNumber = Random.Range(0, maximumActionNumber);
//                    int action = intToAction[intActionNumber];
//                    if (_CheckDirectionEnable(curEnemyState, action))
//                    {
//                        int nextEnemyState;
//                        int nextBossState;
//                        _GetRandomNextStateFromAction(curEnemyState, curBossState, action, out nextEnemyState, out nextBossState);

//                        byte r = reward[curEnemyState, curBossState, intActionNumber];
//                        states[curEnemyState, curBossState, intActionNumber] = (byte)(r + gamma * _GetMaxQValue(curEnemyState, curBossState));

//                        curEnemyState = nextEnemyState;
//                        curBossState = nextBossState;

//                        //// Q(s,a)= Q(s,a) + alpha * (R(s,a) + gamma * Max(next state, all actions) - Q(s,a))
//                        //Q[currentState ,possibleAction] = (int)(R[currentState, action] + (gamma * maximum(action, false)));
//                    }
//                    else
//                        break;
//                }
//            }
//        }
//    }

//    bool _CheckDirectionEnable(int state, int action)
//    {
//        int shift = action;
//        int result = state & (1 << shift);

//        // state 0 is safe, 1 is danger
//        return result == 1 ? false : true;
//    }

//    ulong _GetStateNumber(ulong[] stateIndex)
//    {
//        ulong resultNumber = 0;
//        for (int i = 0; i < stateArrayLength; ++i)
//        {
//            ulong state = stateIndex[i] << (stateDigit * i);
//            resultNumber |= state;
//        }

//        return resultNumber;
//    }

//    void _GetNextStateFromAction(int enemyState, int bossState, int action, out int nextEnemyState, out int nextBossState)
//    {
//        switch ((eAction)action)
//        {
//            case eAction.UP:
//                nextEnemyState = enemyState << 5;
//                nextBossState = bossState + 5;
//                break;
//            case eAction.LEFT:
//                nextEnemyState = enemyState;
//                for (int i = 0; i < 5; ++i)
//                    nextEnemyState = nextEnemyState & ~(1 << (i * 5));

//                nextBossState = bossState + 1;
//                if (nextBossState % 5 == 0) nextBossState = 12;
//                break;
//            case eAction.RIGHT:
//                nextEnemyState = enemyState;
//                for (int i = 0; i < 5; ++i)
//                    nextEnemyState = nextEnemyState & ~(1 << (i * 5 + 4));

//                nextBossState = bossState - 1;
//                if (nextBossState % 5 == 4) nextBossState = 12;
//                break;
//            case eAction.DOWN:
//                nextEnemyState = enemyState >> 5;
//                nextBossState = bossState - 5;
//                break;
//            default:
//                nextEnemyState = enemyState;
//                nextBossState = bossState;
//                break;
//        }

//        if (bossState < 0 || bossState > 24)// case boss not exist
//            bossState = 12;
//    }

//    void _GetRandomNextStateFromAction(int enemyState, int bossState, int action, out int nextEnemyState, out int nextBossState)
//    {
//        switch (action)
//        {
//            // up
//            case 2:
//                nextEnemyState = enemyState << 5;
//                for (int i = 0; i < 5; ++i)
//                    nextEnemyState = nextEnemyState & ~(Random.Range(0, 2) << i);

//                nextBossState = bossState + 5;
//                break;
//            // horizontal    
//            case 5:
//                nextEnemyState = enemyState;
//                for (int i = 0; i < 5; ++i)
//                    nextEnemyState = nextEnemyState & ~(Random.Range(0, 2) << (i * 5));

//                nextBossState = bossState + 1;
//                if (nextBossState % 5 == 0) nextBossState = 12;
//                break;
//            case 7:
//                nextEnemyState = enemyState;
//                for (int i = 0; i < 5; ++i)
//                    nextEnemyState = nextEnemyState & ~(Random.Range(0, 2) << (i * 5 + 4));

//                nextBossState = bossState - 1;
//                if (nextBossState % 5 == 4) nextBossState = 12;
//                break;
//            // down
//            case 10:
//                nextEnemyState = enemyState >> 5;
//                for (int i = 20; i < 25; ++i)
//                    nextEnemyState = nextEnemyState & ~(Random.Range(0, 2) << i);

//                nextBossState = bossState - 5;
//                break;
//            default:
//                nextEnemyState = enemyState;
//                nextBossState = bossState;
//                break;
//        }

//        if (bossState < 0 || bossState > 12)// case boss not exist
//            bossState = 6;
//    }

//    byte _GetMaxQValue(int enemyState, int bossState)
//    {
//        byte max = 0;
//        for (int i = 0; i < maximumActionNumber; ++i)
//        {
//            if (states[enemyState, bossState, i] > max)
//                max = states[enemyState, bossState, i];
//        }

//        return max;
//    }

//    ulong[] GenerateStateArray()
//    {
//        ulong[] ar = new ulong[stateArrayLength];
//        ar[0] = 3;
//        ar[1] = 1;
//        ar[2] = 1;
//        return ar;
//    }

//    float learningRatio = 0f;
//    float maxLearningRatio = 0f;
//    void OnGUI()
//    {
//        learningRatio = GUI.HorizontalSlider(new Rect(100, 100, 100, 10), learningRatio, 0f, maxLearningRatio);
//    }

//    public eAction GetDirection(int state, int boss)
//    {
//        return eAction.DOWN;
//    }
//}

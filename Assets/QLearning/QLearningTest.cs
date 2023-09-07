using UnityEngine;
using System.Collections;

public class QLearningTest : MonoBehaviour {

    const  int qSize = 6;
    const double gamma = 0.8;
    const int iterations = 10;
    int[] initialStates = new int[] { 1, 3, 5, 2, 4, 0 };

    int[,] R = new int[,]{{-1, -1, -1, -1, 0, -1},
			    {-1, -1, -1, 0, -1, 100},
			    {-1, -1, -1, 0, -1, -1},
			    {-1, 0, 0, -1, 0, -1},
			    {0, -1, -1, 0, -1, 100},
			    {-1, 0, -1, -1, 0, 100}};

    int[,] Q = new int[qSize, qSize];
    int currentState;

    //void episode(int initialState);
    //void chooseAnAction();
    //int getRandomAction(int upperBound, int lowerBound);
    //void initialize();
    //int maximum(int state, bool returnIndexOnly);
    //int reward(int action);

    void Start()
    {
        int newState;

        initialize();

        //Perform learning trials starting at all initial states.
        for (int j = 0; j <= (iterations - 1); j++)
        {
            for (int i = 0; i <= (qSize - 1); i++)
            {
                episode(initialStates[i]);
            } // i
        } // j

        Debug.Log("Print out Q matrix.");
        for (int i = 0; i <= (qSize - 1); i++)
        {
            string str = "";
            for (int j = 0; j <= (qSize - 1); j++)
            {
                str += Q[i, j] + ",";
            }

            Debug.Log(str);
        }

        Debug.Log("Perform tests, starting at all initial states.");
        for (int i = 0; i <= (qSize - 1); i++)
        {
            string str = "";
            currentState = initialStates[i];
            newState = 0;
            do
            {
                newState = maximum(currentState, true);
                str = currentState + ", ";
                currentState = newState;
            } while (currentState < 5);
            Debug.Log(str + " 5");
        } // i
    }

    void episode(int initialState)
    {

        currentState = initialState;

        //Travel from state to state until goal state is reached.
        do
        {
            chooseAnAction();
        } while (currentState == 5);

        //When currentState = 5, run through the set once more to
        //for convergence.
        for (int i = 0; i <= (qSize - 1); i++)
        {
            chooseAnAction();
        } // i
    }

    void chooseAnAction()
    {

        int possibleAction;

        //Randomly choose a possible action connected to the current state.
        possibleAction = getRandomAction(qSize, 0);

        if (R[currentState, possibleAction] >= 0)
        {
            Q[currentState ,possibleAction] = reward(possibleAction);
            currentState = possibleAction;
        }
    }

    int getRandomAction(int upperBound, int lowerBound)
    {

        int action;
        bool choiceIsValid = false;
        int range = (upperBound - lowerBound) + 1;

        //Randomly choose a possible action connected to the current state.
        do
        {
            //Get a random value between 0 and 6.
            action = lowerBound + Random.Range(0, 6);
            if (R[currentState, action] > -1)
            {
                choiceIsValid = true;
            }
        } while (choiceIsValid == false);

        return action;
    }

    void initialize()
    {
        for (int i = 0; i <= (qSize - 1); i++)
        {
            for (int j = 0; j <= (qSize - 1); j++)
            {
                Q[i, j] = 0;
            } // j
        } // i
    }

    int maximum(int state, bool returnIndexOnly)
    {
        // if returnIndexOnly = true, a Q matrix index is returned.
        // if returnIndexOnly = false, a Q matrix element is returned.

        int winner;
        bool foundNewWinner;
        bool done = false;

        winner = 0;

        do
        {
            foundNewWinner = false;
            for (int i = 0; i <= (qSize - 1); i++)
            {
                if ((i < winner) || (i > winner))
                {     //Avoid self-comparison.
                    if (Q[state, i] > Q[state, winner])
                    {
                        winner = i;
                        foundNewWinner = true;
                    }
                }
            } // i

            if (foundNewWinner == false)
            {
                done = true;
            }

        } while (done == false);

        if (returnIndexOnly == true)
        {
            return winner;
        }
        else {
            return Q[state, winner];
        }
    }

    int reward(int action)
    {

        return (int)(R[currentState, action] + (gamma * maximum(action, false)));
    }
}

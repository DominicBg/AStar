using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AStar
{
class AStarGenerator
{
        public static bool showDetails = false;

        public static List<Operation> GeneratePlan(World world, State initialState, Goal goal, Heuristic heuristic)
        {
            heuristic.InitialiseGoal(goal);

            SortedSet<State> open = new SortedSet<State>();
            HashSet<State> open2 = new HashSet<State>();
            HashSet<State> close = new HashSet<State>();

            State currentState = initialState;

            open.Add(currentState);
            open2.Add(currentState);

            bool foundSolution = false;

            while(open.Count > 0)
            {
                currentState = open.First();
                open.Remove(currentState);

                if(showDetails)
                    Console.WriteLine(currentState.ToString());

                open2.Remove(currentState);
                close.Add(currentState);

                if (goal.GoalSatisfied(currentState))
                {
                    foundSolution = true;
                    open.Clear();
                    break;
                }

                List<Operation> nextOperations = world.GetActions(currentState);
                List<State> nextStates = GenerateNextStates(world, currentState, nextOperations);

                VisitNextStates(heuristic, open, open2, close, currentState, nextStates, nextOperations);
            }

            if (foundSolution)
            {
                return GetActionsFromParent(currentState);
            }
            else
            {
                return null;
            }
        }

        private static void VisitNextStates(Heuristic heuristic, SortedSet<State> open, HashSet<State> open2, HashSet<State> close, State currentState, List<State> nextStates, List<Operation> nextActions)
        {
            for (int i = 0; i < nextStates.Count; i++)
            {
                State nextState = nextStates[i];
                Operation nextAction = nextActions[i];

                if (!close.Contains(nextState))
                {
                    nextState.parent = currentState;
                    if (open.Contains(nextState))
                    {
                        //Si existe deja, update les stats

                        double newg = currentState.g + nextAction.cost;
                        double newf = newg + heuristic.EstimateCost(nextState);
                        if (newf < nextState.f)
                        {
                            open.Remove(nextState);
                            nextState.g = newg;
                            nextState.f = newf;
                            open.Add(nextState);
                        }
                    }
                    else
                    {
                        //Jamais été visité
                        nextState.g = currentState.g + nextAction.cost;
                        nextState.f = nextState.g + heuristic.EstimateCost(nextState);
                        open.Add(nextState);
                        open2.Add(nextState);

                    }
                }
            }
        }

        static List<State> GenerateNextStates(World world, State state, List<Operation> operations)
        {
            List<State> nextStates = new List<State>();
            foreach (Operation operation in operations)
            {
                State nextState = world.Executer(state, operation);
                nextState.parent = state;
                nextState.actionFromParent = operation;
                nextStates.Add(nextState);
            }
            return nextStates;
        }

        static List<Operation> GetActionsFromParent(State state)
        {
            if(state == null)
                return null;

            State currentState = state;
            List<Operation> actions = new List<Operation>();
            while(currentState.parent != null)
            {
                actions.Add(currentState.actionFromParent);
                currentState = currentState.parent;
            }
            actions.Reverse();
            return actions;
        }
}
}

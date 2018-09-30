using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AStarReferences
{
    public class Operation
    {
        public double cost;
    }

    public abstract class Goal
    {
        public abstract bool GoalSatisfied(State state);
    }

    public abstract class State : IComparable<State>
    {
        public State parent;

        /** Action à partir de parent permettant d'atteindre cet état. */
        public Operation actionFromParent;

        /** f=g+h. */
        public double f;

        /** Meilleur coût trouvé pour atteindre cet été à partir de l'état initial. */
        public double g;

        /** Estimation du coût restant pour atteindre le but. */
        public double h;

        public abstract int CompareTo(State other);
    }

    public abstract class Heuristic
    {
        Goal goal;
        public virtual void InitialiseGoal(Goal goal)
        {
            this.goal = goal;
        }

        /** Retourne une estimation du coût restant pour atteindre le but b à partir de l'état du monde e.
         *  Pour être admissible, cette fonction ne doit jamais surestimer le coût restant.
         */
        public abstract double EstimateCost(State state);
    }

    public abstract class World
    {
        /** Retourne la liste des actions exécutables dans l'état de ce monde. */
        public abstract List<Operation> GetActions(State state);

        /** Exécute une action dans un état du monde et retourne un nouvel état résultat. */
        public abstract State Executer(State state, Operation a);
    }

}

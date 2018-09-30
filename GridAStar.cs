using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AStar
{
    public class GridAStar
    {
        public char[,] map;
        public int width;
        public int heigth;

        public GridAStar(bool printDetails)
        {
            AStarGenerator.showDetails = printDetails;
        }
        public GridAStar(){}

        public char GetMap(Vector2 position)
        {
            return GetMap(position.x, position.y);
        }
        public char GetMap(int x, int y)
        {
            return map[y, x];
        }

        public List<Vector2> FindPath(Vector2 startPosition, Vector2 endPosition, char[,] map, char[] collisions)
        {
            this.map = map;
            width = map.GetLength(1);
            heigth = map.GetLength(0);

            GridWorld world = new GridWorld(map, collisions, this);
            GridState initialState = new GridState(startPosition, this);

            GridGoal goal = new GridGoal(endPosition);

            GridHeuristic heuristic = new GridHeuristic(goal);

            List<Operation> operations = AStarGenerator.GeneratePlan(world, initialState, goal, heuristic);
            //Si pas de chemin
            if (operations == null)
                return null;

            //Convertis les chemins en vector2
            List<Vector2> directions = new List<Vector2>();
            for (int i = 0; i < operations.Count; i++)
            {
                GridOperation gridOperation = (GridOperation)operations[i];
                directions.Add(gridOperation.direction);
            }
            return directions;
        }
    }

    public class GridWorld : World
    {
        char[,] map;
        char[] collisions;
        GridAStar gridAStar;

        Vector2[] directions =
        {
            new Vector2(1,0),
            new Vector2(0,1),
            new Vector2(-1,0),
            new Vector2(0,-1),
        };

        public GridWorld(char[,] map, char[] collisions, GridAStar gridAStar)
        {
            this.map = map;
            this.collisions = collisions;
            this.gridAStar = gridAStar;
        }

        //Genere les autres states possibles
        public override State Executer(State state, Operation a)
        {
            GridState gridState = (GridState)state;
            GridOperation gridAction = (GridOperation)a;

            return new GridState(gridState.position + gridAction.direction, gridAStar);
        }

        //Genere les direction possibles pour aller au autres place
        public override List<Operation> GetActions(State state)
        {
            GridState gridState = (GridState)state;
            List<Operation> operations = new List<Operation>();
            for (int i = 0; i < directions.Length; i++)
            {
               Vector2 newPosition = gridState.position + directions[i];
               if (InBound(newPosition) && !HasCollision(newPosition))
               {
                    operations.Add(new GridOperation(directions[i]));
               }
            }
            return operations;
        }

        bool HasCollision(Vector2 position)
        {
            char currentChar = gridAStar.GetMap(position);
            for (int i = 0; i < collisions.Length; i++)
            {
                if (currentChar == collisions[i])
                    return true;
            }
            return false;
        }

        bool InBound(Vector2 position)
        {
            return position.x > 0 && position.y > 0 && position.x < gridAStar.width && position.y < gridAStar.heigth;
        }
    }

    public class GridOperation : Operation
    {
        public Vector2 direction;

        public GridOperation(Vector2 direction)
        {
            this.direction = direction;
        }

        public override string ToString()
        {
            return direction.ToString();
        }
    }

    public class GridState : State
    {
        public Vector2 position;
        GridAStar gridAStar;

        public GridState(Vector2 position, GridAStar gridAStar)
        {
            this.position = position;
            this.gridAStar = gridAStar;
        }

        public override int CompareTo(State other)
        {
            GridState otherState = (GridState)other;
            if (f == other.f)
                return 0;
            else if (f > other.f)
                return 1;
            else
                return -1;
        }

        public override bool Equals(object obj)
        {
            var state = obj as GridState;
            return state != null && (position == state.position);
        }

        public override int GetHashCode()
        {
            return 1206833562 + EqualityComparer<Vector2>.Default.GetHashCode(position);
        }

        public override string ToString()
        {
            string stringMap = "";
            for (int i = 0; i < gridAStar.map.GetLength(0); i++)
            {
                for (int j = 0; j < gridAStar.map.GetLength(1); j++)
                {
                    if (j == position.x && i == position.y)
                        stringMap += "X";
                    else
                        stringMap += gridAStar.map[i, j];

                }
                stringMap += "\n";
            }

            return stringMap;
        }
    }

    public class GridHeuristic : Heuristic
    {
        GridGoal gridGoal;

        public GridHeuristic(GridGoal gridGoal)
        {
            this.gridGoal = gridGoal;
        }

        public override double EstimateCost(State state)
        {
            GridState gridState = (GridState)state;
            return Vector2.ManhattanDistance(gridState.position, gridGoal.endPosition);
        }
    }

    public class GridGoal : Goal
    {
        public Vector2 endPosition;

        public GridGoal(Vector2 endPosition)
        {
            this.endPosition = endPosition;
        }

        public override bool GoalSatisfied(State state)
        {
            GridState gridState = (GridState)state;
            return gridState.position == endPosition;
        }
    }
}

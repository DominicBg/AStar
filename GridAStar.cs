using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AStar
{
    public class GridAStar<T>
    {
        public T[,] map;
        public int width;
        public int heigth;
        public bool showPosition = true;
        public bool showGrid = false;
        public Vector2 goalPosition;

        public GridAStar(bool printDetails)
        {
            AStarGenerator.showDetails = printDetails;
        }
        public GridAStar(){}

        public T GetMap(Vector2 position)
        {
            return GetMap(position.x, position.y);
        }
        public T GetMap(int x, int y)
        {
            return map[x, y];
        }

        public List<Vector2> FindPath(Vector2 startPosition, Vector2 endPosition, T[,] map, T[] collisions)
        {
            this.map = map;
            width = map.GetLength(1);
            heigth = map.GetLength(0);

            GridWorld<T> world = new GridWorld<T>(map, collisions, this);
            GridState<T> initialState = new GridState<T>(startPosition, this);
           // Console.WriteLine(initialState.ToString());
            
            GridGoal<T> goal = new GridGoal<T>(endPosition);
            goalPosition = endPosition;
            GridHeuristic<T> heuristic = new GridHeuristic<T>(goal);

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

    public class GridWorld<T> : World
    {
        T[,] map;
        T[] collisions;
        GridAStar<T> gridAStar;

        Vector2[] directions =
        {
            new Vector2(1,0),
            new Vector2(0,1),
            new Vector2(-1,0),
            new Vector2(0,-1),
        };

        public GridWorld(T[,] map, T[] collisions, GridAStar<T> gridAStar)
        {
            this.map = map;
            this.collisions = collisions;
            this.gridAStar = gridAStar;
        }

        //Genere les autres states possibles
        public override State Executer(State state, Operation a)
        {
            GridState<T> gridState = (GridState<T>)state;
            GridOperation gridAction = (GridOperation)a;

            return new GridState<T>(gridState.position + gridAction.direction, gridAStar);
        }

        //Genere les direction possibles pour aller au autres place
        public override List<Operation> GetActions(State state)
        {
            GridState<T> gridState = (GridState<T>)state;
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
            T currentCollision = gridAStar.GetMap(position);
            for (int i = 0; i < collisions.Length; i++)
            {
                if (currentCollision.Equals(collisions[i]))
                    return true;
            }
            return false;
        }

        bool InBound(Vector2 position)
        {
            return position.x >= 0 && position.y >= 0 && position.x < gridAStar.width && position.y < gridAStar.heigth;
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

    public class GridState<T> : State
    {
        public Vector2 position;
        GridAStar<T> gridAStar;

        public GridState(Vector2 position, GridAStar<T> gridAStar)
        {
            this.position = position;
            this.gridAStar = gridAStar;
        }

        public override int CompareTo(State other)
        {
            GridState<T> otherState = (GridState<T>)other;
            if (f == other.f && position == otherState.position)
                return 0;
            else if (f > other.f)
                return 1;
            else
                return -1;
        }

        public override bool Equals(object obj)
        {
            var state = obj as GridState<T>;
            return state != null && (position == state.position);
        }

        public override int GetHashCode()
        {
            return 1206833562 + EqualityComparer<Vector2>.Default.GetHashCode(position);
        }

        public override string ToString()
        {
            string stringMap = "";
            if (gridAStar.showGrid)
            {
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
            }
            return ((gridAStar.showPosition) ? position.ToString() : " ") + "\n" + stringMap;
        }
    }

    public class GridHeuristic<T> : Heuristic
    {
        GridGoal<T> gridGoal;

        public GridHeuristic(GridGoal<T> gridGoal)
        {
            this.gridGoal = gridGoal;
        }

        public override double EstimateCost(State state)
        {
            GridState<T> gridState = (GridState<T>)state;
            return Vector2.ManhattanDistance(gridState.position, gridGoal.endPosition);
        }
    }

    public class GridGoal<T> : Goal
    {
        public Vector2 endPosition;

        public GridGoal(Vector2 endPosition)
        {
            this.endPosition = endPosition;
        }

        public override bool GoalSatisfied(State state)
        {
            GridState<T> gridState = (GridState<T>)state;
            return gridState.position == endPosition;
        }
    }
}

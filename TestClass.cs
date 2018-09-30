using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AStar;

class TestClass
{
    static void Main()
    { 
        int[,] mapInt = {
                { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
                { 1,0,1,0,1,0,0,0,0,1,0,1,1,2,1},
                { 1,0,0,0,0,0,1,1,0,0,0,0,0,0,1},
                { 1,0,1,0,1,1,1,1,1,0,1,1,1,0,1},
                { 1,0,1,0,0,0,0,0,1,0,0,0,1,0,1},
                { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
        };

        char[,] map = IntToCharArray(mapInt);
        char[] collisions = { '1' };

        Vector2 start = new Vector2(1, 4);
        Vector2 end = FindEndPosition(map, '2');

        GridAStar gridAStar = new GridAStar(false);

        List<Vector2> directions = gridAStar.FindPath(start, end, map, collisions);

        if(directions == null)
        {
            Console.WriteLine("No path");
        }
        else
        {
            //Show all directions
            for (int i = 0; i < directions.Count; i++)
            {
                Console.WriteLine(directions[i].ToString());
            }
        }
        Console.ReadLine();
    }

    static char[,] IntToCharArray(int[,] intArray)
    {
        int rows = intArray.GetLength(0);
        int cols = intArray.GetLength(1);

        char[,] charArray = new char[rows, cols];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                charArray[i, j] = intArray[i, j].ToString()[0];
            }
        }
        return charArray;
    }

    static Vector2 FindEndPosition(char[,] map, char endCharacter)
    {
        Vector2 end = new Vector2(0,0);
        for (int x = 0; x < map.GetLength(1); x++)
        {
            for (int y = 0; y < map.GetLength(0); y++)
            {
                if (map[y,x] == endCharacter)
                {
                    end = new Vector2(x, y);
                    Console.WriteLine("End position = " + end);
                }
            }
        }
        return end;
    }
}


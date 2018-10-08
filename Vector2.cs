using LHGames.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Vector2
{
    public int x;
    public int y;

    public Vector2(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public Vector2(Point point)
    {
        x = point.X;
        y = point.Y;
    }

    public static bool operator ==(Vector2 v1, Vector2 v2)
    {
        return v1.x == v2.x && v1.y == v2.y;
    }

    public static bool operator !=(Vector2 v1, Vector2 v2)
    {
        return !(v1 == v2);
    }

    public static Vector2 operator +(Vector2 v1, Vector2 v2)
    {
        return new Vector2(v1.x + v2.x, v1.y + v2.y);
    }

    public static Vector2 operator -(Vector2 v1, Vector2 v2)
    {
        return new Vector2(v1.x - v2.x, v1.y - v2.y);
    }

    public override bool Equals(object obj)
    {
        var vector = obj as Vector2;
        return vector != null &&
               x == vector.x &&
               y == vector.y;
    }

    public override int GetHashCode()
    {
        var hashCode = 1502939027;
        hashCode = hashCode * -1521134295 + x.GetHashCode();
        hashCode = hashCode * -1521134295 + y.GetHashCode();
        return hashCode;
    }

    public override string ToString()
    {
        if (x == 0 && y == 1)
            return "down";
        else if (x == 0 && y == -1)
            return "up";
        else if (x == 1 && y == 0)
            return "right";
        else if (x == -1 && y == 0)
            return "left";

        return x.ToString() + " " + y.ToString();
    }

    public static int ManhattanDistance(Vector2 v1, Vector2 v2)
    {
        return Math.Abs(v1.x - v2.x) + Math.Abs(v1.y - v2.y);
    }


}
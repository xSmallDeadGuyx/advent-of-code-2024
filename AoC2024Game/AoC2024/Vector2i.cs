using System;
using System.Numerics;

namespace AoC2024;

public struct Vector2i : IEquatable<Vector2i>
{
    public int X;
    public int Y;

    public Vector2i()
    {
        X = 0;
        Y = 0;
    }

    public Vector2i(int v)
    {
        X = v;
        Y = v;
    }

    public Vector2i(int x, int y)
    {
        X = x;
        Y = y;
    }

    public Vector2i(Vector2 v)
    {
        X = (int)v.X;
        Y = (int)v.Y;
    }

    public bool Equals(Vector2i other)
    {
        return X == other.X && Y == other.Y;
    }

    public Vector2 ToVector2()
    {
        return new Vector2(X, Y);
    }

    public static Vector2i operator +(Vector2i a, Vector2i b) => new Vector2i(a.X + b.X, a.Y + b.Y);
    public static Vector2i operator -(Vector2i a, Vector2i b) => new Vector2i(a.X - b.X, a.Y - b.Y);
    public static Vector2i operator *(Vector2i a, Vector2i b) => new Vector2i(a.X * b.X, a.Y * b.Y);
    public static Vector2i operator /(Vector2i a, Vector2i b) => new Vector2i(a.X / b.X, a.Y / b.Y);

    public static Vector2i operator *(Vector2i v, int s) => new Vector2i(v.X * s, v.Y * s);
    public static Vector2i operator *(Vector2i v, float s) => new Vector2i((int)(v.X * s), (int)(v.Y * s));
    public static Vector2i operator /(Vector2i v, int s) => new Vector2i(v.X / s, v.Y / s);
    public static Vector2i operator /(Vector2i v, float s) => new Vector2i((int)(v.X / s), (int)(v.Y / s));
    
    // Static vector types
    public static Vector2i Zero
    {
        get => new Vector2i(0, 0);
    }

    public static Vector2i One
    {
        get => new Vector2i(1, 1);
    }

    public static Vector2i UnitX
    {
        get => new Vector2i(1, 0);
    }

    public static Vector2i UnitY
    {
        get => new Vector2i(0, 1);
    }

    // Useful static vector arrays
    public static Vector2i[] Cardinals
    {
        get => new Vector2i[] {new Vector2i(0, -1), new Vector2i(1, 0), new Vector2i(0, 1), new Vector2i(-1, 0)};
    }

    public static Vector2i[] AllDirections
    {
        get => new Vector2i[] {new Vector2i(0, -1), new Vector2i(1, -1), new Vector2i(1, 0), new Vector2i(1, 1), new Vector2i(0, 1), new Vector2i(-1, 1), new Vector2i(-1, 0), new Vector2i(-1, -1)};
    }
}
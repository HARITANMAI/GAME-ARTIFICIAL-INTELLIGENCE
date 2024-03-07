using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Maths
{
    public static float Magnitude(Vector2 a)
    {
        return Mathf.Sqrt(a.x * a.x + a.y * a.y);
    }

    public static Vector2 Normalise(Vector2 a)
    {
        float mag = Magnitude(a);
        //a.x = a.x / mag;
        //a.y = a.y / mag;
        //return a;

        if (mag > 0) return new Vector2(a.x / mag, a.y / mag);
        else return Vector2.zero;
    }

    public static float Dot(Vector2 lhs, Vector2 rhs)
    {
        lhs = Normalise(lhs);
        rhs = Normalise(rhs);
        return (lhs.x * rhs.x) + (lhs.y * rhs.y);

    }

    /// <summary>
    /// Returns the radians of the angle between two vectors
    /// </summary>
    public static float Angle(Vector2 lhs, Vector2 rhs)
    {
        return Mathf.Acos(Dot(lhs,rhs));
    }

    /// <summary>
    /// Translates a vector by X angle in degrees
    /// </summary>
    public static Vector2 RotateVector(Vector2 vector, float angle)
    {
        float rad = angle * Mathf.Deg2Rad;
        Vector2 newVec;
        newVec.x = vector.x * Mathf.Cos(rad) - vector.y * Mathf.Sin(rad);
        newVec.y = vector.x * Mathf.Sin(rad) + vector.y * Mathf.Cos(rad);
		return newVec;
	}
}

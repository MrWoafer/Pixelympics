using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class Functions
{
    public static float RoundToRange(float value, float min, float max)
    {
        if (value > max)
        {
            return max;
        }
        else if (value < min)
        {
            return min;
        }
        else
        {
            return value;
        }
    }

    public static int RoundToRange(int value, int min, int max)
    {
        if (value > max)
        {
            return max;
        }
        else if (value < min)
        {
            return min;
        }
        else
        {
            return value;
        }
    }

    /*public static bool RandomBool()
    {
        if (Random.Range(0f, 1f) > 0.5f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }*/

    public static float Mod(float x, float y)
    {
        return (x % y + y) % y;
    }

    public static float Mod(int x, int y)
    {
        return (x % y + y) % y;
    }

    public static float Sinh(float x)
    {
        return (Mathf.Exp(x) - Mathf.Exp(-x)) / 2f;
    }
    public static float Cosh(float x)
    {
        return (Mathf.Exp(x) + Mathf.Exp(-x)) / 2f;
    }
    public static float Tanh(float x)
    {
        return Sinh(x) / Cosh(x);
    }

    public static float Arsinh(float x)
    {
        return Mathf.Log(x + Mathf.Sqrt(1f + x * x));
    }
    public static float Arcosh(float x)
    {
        if (x >= 1f)
        {
            return Mathf.Log(x + Mathf.Sqrt(x * x - 1f));
        }
        throw new System.ArgumentException("Domain of arcosh is {x : x >= 1}");
    }
    public static float Artanh(float x)
    {
        if (x > -1f && x < 1f)
        {
            return 0.5f * Mathf.Log((1f + x) / (1f - x));
        }
        throw new System.ArgumentException("Domain of artanh is (-1, 1)");
    }

    /// <summary>
    /// Calculates sin(x), where x is in degrees.
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    public static float Sind(float x)
    {
        return Mathf.Sin(x * Mathf.Deg2Rad);
    }
    /// <summary>
    /// Calculates cos(x), where x is in degrees.
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    public static float Cosd(float x)
    {
        return Mathf.Cos(x * Mathf.Deg2Rad);
    }

    public static bool RandomBool(float probability = 0.5f)
    {
        if (probability <= 0f)
        {
            return false;
        }
        if (probability >= 1f)
        {
            return true;
        }
        else
        {
            return Random.Range(0f, 1f) <= probability;
        }
    }

    public static int WeightedRand(int[] weights)
    {
        int randomIndex = Random.Range(0, Enumerable.Sum(weights));
        int runningTotal = 0;

        for (int i = 0; i < weights.Length; i++)
        {
            runningTotal += weights[i];
            if (randomIndex < runningTotal)
            {
                return i;
            }
        }
        return -1;
    }

    public static Vector2 Lerp(Vector2 a, Vector2 b, float t)
    {
        return new Vector2(Mathf.Lerp(a.x, b.x, t), Mathf.Lerp(a.y, b.y, t));
    }
    public static Vector3 Lerp(Vector3 a, Vector3 b, float t)
    {
        return new Vector3(Mathf.Lerp(a.x, b.x, t), Mathf.Lerp(a.y, b.y, t), Mathf.Lerp(a.z, b.z, t));
    }

    public static Vector2 LerpUnclamped(Vector2 a, Vector2 b, float t)
    {
        return new Vector2(Mathf.LerpUnclamped(a.x, b.x, t), Mathf.LerpUnclamped(a.y, b.y, t));
    }
    public static Vector3 LerpUnclamped(Vector3 a, Vector3 b, float t)
    {
        return new Vector3(Mathf.LerpUnclamped(a.x, b.x, t), Mathf.LerpUnclamped(a.y, b.y, t), Mathf.LerpUnclamped(a.z, b.z, t));
    }

    /// <summary>
    /// Returns whether the inputs have the same sign. Zero is counted as positive and negative.
    /// </summary>
    /// <returns></returns>
    public static bool AreSameSign(float x, float y)
    {
        return x * y >= 0f;
    }

    /// <summary>
    /// Returns whether the inputs have the same sign. Zero is counted as positive and negative.
    /// </summary>
    /// <returns></returns>
    public static bool AreSameSign(float x, float y, float z)
    {
        //return AreSameSign(x, y) && AreSameSign(y, z) && AreSameSign(z, x);
        return AreSameSign(x, y) && AreSameSign(y, z);
    }

    /// <summary>
    /// Returns whether the inputs have the same sign. Zero is counted as positive and negative.
    /// </summary>
    /// <returns></returns>
    public static bool AreSameSign(float x, float y, float z, float w)
    {
        //return AreSameSign(x, y) && AreSameSign(x, z) && AreSameSign(x, w) && AreSameSign(y, z) && AreSameSign(y, w) && AreSameSign(z, w);
        return AreSameSign(x, y) && AreSameSign(y, z) && AreSameSign(z, w);
    }

    /// <summary>
    /// Returns whether the inputs have the same sign. Zero is counted as positive and negative.
    /// </summary>
    /// <returns></returns>
    public static bool AreSameSign(float[] nums)
    {
        for (int i = 0; i < nums.Length - 1; i++)
        {
            if (!AreSameSign(nums[i], nums[i + 1]))
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Puts the x and y components of the Vector3 into a Vector2.
    /// </summary>
    /// <param name="vector3"></param>
    /// <returns></returns>
    public static Vector2 Vector3To2(Vector3 vector3)
    {
        return new Vector2(vector3.x, vector3.y);
    }

    /// <summary>
    /// Returns a Vector3 with the x and y components of the Vector2 and a 0 in the z component.
    /// </summary>
    /// <param name="vector2"></param>
    /// <returns></returns>
    public static Vector3 Vector2To3(Vector2 vector2)
    {
        return new Vector3(vector2.x, vector2.y, 0f);
    }
    
    /// <summary>
    /// Returns an equivalent angle in the range (-180, 180].
    /// </summary>
    public static float ModAngle(float angle)
    {
        if (Mod(angle, 360f) <= 180f)
        {
            return Mod(angle, 360f);
        }
        else
        {
            return Mod(angle, 360f) - 360f;
        }
    }

    public static Vector2 PolarToCartesian(float modulus, float argument)
    {
        return new Vector2(Mathf.Cos(argument * Mathf.Deg2Rad), Mathf.Sin(argument * Mathf.Deg2Rad)) * modulus;
    }

    /// <summary>
    /// Returns the polar form (modulus, argument) of the Cartesian vector.
    /// </summary>
    public static Vector2 CartesianToPolar(Vector2 vector)
    {
        if (vector.magnitude == 0f)
        {
            return new Vector2(0f, 0f);
        }
        else
        {
            float angle;
            if (vector.x == 0f)
            {
                if (vector.y > 0f)
                {
                    angle = 90f;
                }
                else
                {
                    angle = -90f;
                }
            }
            else
            {
                if (vector.x > 0f && vector.y >= 0f)
                {
                    angle = Mathf.Rad2Deg * Mathf.Atan(vector.y / vector.x);
                }
                else if (vector.x < 0f && vector.y >= 0f)
                {
                    angle = 180f - Mathf.Rad2Deg * Mathf.Atan(vector.y / -vector.x);
                }
                else if (vector.x > 0f && vector.y < 0f)
                {
                    angle = -Mathf.Rad2Deg * Mathf.Atan(-vector.y / vector.x);
                }
                else
                {
                    angle = -180f + Mathf.Rad2Deg * Mathf.Atan(vector.y / vector.x);
                }
            }

            return new Vector2(vector.magnitude, angle);
        }
    }

    private static System.Random rng = new System.Random();

    /// <summary>
    /// Randomises the order of the arrays but makes the same swaps to each array.
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <param name="array1"></param>
    /// <param name="array2"></param>
    public static void PairedShuffle<T1, T2>(T1[] array1, T2[]array2)
    {
        if (array1.Length != array2.Length)
        {
            throw new System.Exception("Array lengths must be the same. Lengths: " + array1.Length + " and " + array2.Length);
        }

        /// Fisher-Yates shuffle algorithm
        int n = array1.Length;
        while (n > 1)
        {
            int k = rng.Next(n--);

            T1 temp = array1[n];
            array1[n] = array1[k];
            array1[k] = temp;

            T2 temp2 = array2[n];
            array2[n] = array2[k];
            array2[k] = temp2;
        }
    }

    /// <summary>
    /// Returns the array but with only one occurrence of each element.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <returns></returns>
    public static T[] RemoveDuplicates<T>(T[] array)
    {
        List<T> noDuplicates = new List<T>();
        foreach (T item in array)
        {

            if (!noDuplicates.Contains(item))
            {
                noDuplicates.Add(item);
            }
        }

        return noDuplicates.ToArray();
    }

    /// <summary>
    /// Randomises the order of the array.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array1"></param>
    public static void Shuffle<T>(T[] array1)
    {
        /// Fisher-Yates shuffle algorithm
        int n = array1.Length;
        while (n > 1)
        {
            int k = rng.Next(n--);

            T temp = array1[n];
            array1[n] = array1[k];
            array1[k] = temp;
        }
    }

    /// <summary>
    /// Turns an array into a string with commas separating each element.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <returns></returns>
    public static string ArrayToString<T>(T[] array)
    {
        string str = "";

        for (int i = 0; i < array.Length; i++)
        {
            str += array[i].ToString();

            if (i < array.Length - 1)
            {
                str += ", ";
            }
        }

        return str;
    }

    /// <summary>
    /// Returns a colour with the given RGB values with them ranging from 0-255.
    /// </summary>
    /// <param name="r"></param>
    /// <param name="g"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static Color Color255(float r, float g, float b)
    {
        return new Color(r / 255f, g / 255f, b / 255f);
    }

    /// <summary>
    /// Returns a colour with the given RGBA values with them ranging from 0-255.
    /// </summary>
    /// <param name="r"></param>
    /// <param name="g"></param>
    /// <param name="b"></param>
    /// <param name="a"></param>
    /// <returns></returns>
    public static Color Color255(float r, float g, float b, float a)
    {
        return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
    }

    /// <summary>
    /// Returns the number of times the given element occurs in the given array.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="element"></param>
    /// <returns></returns>
    public static int CountOccurrences<T>(T[] array, T element)
    {
        return array.Where(i => i.Equals(element)).Count();
    }

    /// <summary>
    /// Returns the number of times the given element occurs in the given list.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="element"></param>
    /// <returns></returns>
    public static int CountOccurrences<T>(List<T> array, T element)
    {
        return array.Where(i => i.Equals(element)).Count();
    }
}

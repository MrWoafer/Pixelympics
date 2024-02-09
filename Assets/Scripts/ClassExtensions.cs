using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public static class ClassExtensions
{
    private static System.Random rng = new System.Random();

    /// <summary>
    /// Randomises the order of the array.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    public static void Shuffle<T>(this T[] array)
    {
        int n = array.Length;
        while (n > 1)
        {
            int k = rng.Next(n--);
            T temp = array[n];
            array[n] = array[k];
            array[k] = temp;
        }
    }

    /// <summary>
    /// Returns the index of the given item.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public static int IndexOf<T>(this T[] array, T item)
    {
        return Array.IndexOf(array, item);
    }

    /// <summary>
    /// Returns the number of times the given element occurs in the array.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="element"></param>
    /// <returns></returns>
    public static int CountOccurrences<T>(this T[] array, T element)
    {
        return array.Where(i => i.Equals(element)).Count();
    }

    /// <summary>
    /// Returns the number of times the given element occurs in the list.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="element"></param>
    /// <returns></returns>
    public static int CountOccurrences<T>(this List<T> array, T element)
    {
        return array.Where(i => i.Equals(element)).Count();
    }
}

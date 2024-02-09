using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Matrix2x2
{
    private float[,] matrix;

    public Matrix2x2()
    {
        matrix = new float[2, 2];
    }
    public Matrix2x2(float a, float b, float c, float d)
    {
        matrix = new float[2, 2];
        matrix[0, 0] = a;
        matrix[0, 1] = b;
        matrix[1, 0] = c;
        matrix[1, 1] = d;
    }

    public float this[int i, int j]
    {
        set
        {
            if (i >= 0 && i < 2)
            {
                if (j >= 0 && j < 2)
                {
                    matrix[i, j] = value;
                }
                else
                {
                    throw new IndexOutOfRangeException("Column index out of range. Range: 0-2" + ". Column index: " + j);
                }
            }
            else
            {
                throw new IndexOutOfRangeException("Row index out of range. Range: 0-2" + ". Row index: " + i);
            }
        }
        get
        {
            if (i >= 0 && i < 2)
            {
                if (j >= 0 && j < 2)
                {
                    return matrix[i, j];
                }
                else
                {
                    throw new IndexOutOfRangeException("Column index out of range. Range: 0-2" + ". Column index: " + j);
                }
            }
            else
            {
                throw new IndexOutOfRangeException("Row index out of range. Range: 0-2" + ". Row index: " + i);
            }
        }
    }

    public static Vector2 operator *(Matrix2x2 matrix, Vector2 vector)
    {
        return new Vector2(matrix[0, 0] * vector.x + matrix[0, 1] * vector.y, matrix[1, 0] * vector.x + matrix[1, 1] * vector.y);
    }

    /// <summary>
    /// Returns the matrix that will rotate a point by 'angle' about the origin.
    /// </summary>
    /// <param name="angle">The angle, in radians, measured anti-clockwise from the positive x-axis.</param>
    /// <returns></returns>
    public static Matrix2x2 RotationMatrix(float angle)
    {
        return new Matrix2x2(Mathf.Cos(angle), -Mathf.Sin(angle), Mathf.Sin(angle), Mathf.Cos(angle));
    }

    /// <summary>
    /// Returns the matrix that will rotate a point by 'angle' about the origin.
    /// </summary>
    /// <param name="angle">The angle, in degrees, measured anti-clockwise from the positive x-axis.</param>
    /// <returns></returns>
    public static Matrix2x2 RotationMatrixDegrees(float angle)
    {
        return Matrix2x2.RotationMatrix(angle * Mathf.Deg2Rad);
    }
}

/*public class Matrix
{
    public int rows { get; }
    public int columns { get; }

    private float[,] matrix;

    public Matrix(int rows, int columns)
    {
        this.rows = rows;
        this.columns = columns;
        SetDimensions();
    }

    private void SetDimensions()
    {
        matrix = new float[rows, columns];
    }

    public float this[int i, int j]
    {
        set
        {
            if (i >= 0 && i < rows)
            {
                if (j >= 0 && j < columns)
                {
                    matrix[i, j] = value;
                }
                else
                {
                    throw new IndexOutOfRangeException("Column index out of range. Range: 0-" + columns + ". Column index: " + j);
                }
            }
            else
            {
                throw new IndexOutOfRangeException("Row index out of range. Range: 0-" + rows + ". Row index: " + i);
            }
        }
        get
        {
            if (i >= 0 && i < rows)
            {
                if (j >= 0 && j < columns)
                {
                    return matrix[i, j];
                }
                else
                {
                    throw new IndexOutOfRangeException("Column index out of range. Range: 0-" + columns + ". Column index: " + j);
                }
            }
            else
            {
                throw new IndexOutOfRangeException("Row index out of range. Range: 0-" + rows + ". Row index: " + i);
            }
        }
    }

    public static Matrix operator +(Matrix matrix1, Matrix matrix2)
    {
        if (matrix1.rows == matrix2.rows)
        {
            if (matrix1.columns == matrix2.columns)
            {
                Matrix newMatrix = new Matrix(matrix1.rows, matrix2.rows);

                for (int i = 0; i < matrix1.rows; i++)
                {
                    for (int j = 0; j < matrix1.columns; j++)
                    {
                        newMatrix[i, j] = matrix1[i, j] + matrix2[i, j];
                    }
                }

                return newMatrix;
            }
            else
            {
                throw new ArgumentException("Matrices' column counts are not equal. Column counts: " + matrix1.columns + " and " + matrix2.columns);
            }
        }
        else
        {
            throw new ArgumentException("Matrices' row counts are not equal. Row counts: " + matrix1.rows + " and " + matrix2.rows);
        }
    }

    public static Vector2 operator *(Matrix matrix, Vector2 vector)
    {
        if (matrix.rows == )
        Vector2 newVector = Vector2.zero;
    }
}*/

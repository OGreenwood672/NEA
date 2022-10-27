using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityGenerationsUtilities
{

    public static int[,] merge_sort_2d(int[,] arr, int index_to_sort_by)
    {

        if (arr.GetLength(0) == 1) { return arr; }

        int mid_index = (int)Mathf.Floor(arr.GetLength(0) / 2);

        int[,] left = new int[mid_index, arr.GetLength(1)];
        int[,] right = new int[arr.GetLength(0) - mid_index, arr.GetLength(1)]; //Initially did .Length
        for (int i=0; i<arr.GetLength(0); i++)
        {

            if (i < mid_index)
            {
                for (int j=0; j<arr.GetLength(1); j++)
                {
                    left[i, j] = arr[i, j];
                }

            } else {

                for (int j=0; j<arr.GetLength(1); j++)
                {
                    right[i - mid_index, j] = arr[i, j];  // Used % instead of -
                }

            }

        }

        
        left = merge_sort_2d(left, index_to_sort_by);
        right = merge_sort_2d(right, index_to_sort_by);

        int[,] res = new int[arr.GetLength(0), arr.GetLength(1)];
        int left_index = 0;
        int right_index = 0;

        while (true)
        {

            if (left[left_index, index_to_sort_by] < right[right_index, index_to_sort_by])
            {
                for (int j=0; j<arr.GetLength(1); j++)
                {
                    res[left_index+right_index, j] = left[left_index, j];
                }
                left_index++;

            } else {

                for (int j=0; j<arr.GetLength(1); j++)
                {
                    res[left_index+right_index, j] = right[right_index, j];
                }
                right_index++;

            }

            if (left_index == mid_index || right_index == arr.GetLength(0) - mid_index)
            {
                break;
            }

        }

        while (left_index != mid_index)
        {
            for (int j=0; j<arr.GetLength(1); j++)
            {
                res[left_index+right_index, j] = left[left_index, j];
            }
            left_index++;
        }
        while (right_index != arr.GetLength(0)-mid_index)
        {
            for (int j=0; j<arr.GetLength(1); j++)
            {
                res[left_index+right_index, j] = right[right_index, j];
            }
            right_index++;
        }

        return res;

    }

    public static int constrain(int x, int minimum, int maximum)
    {

        return Mathf.Min(maximum, Mathf.Max(x, minimum));

    }
}

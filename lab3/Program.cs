using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        System.Console.WriteLine("Ковачёв Констатин Владимирович\n 020303-АИСа-25\n\n");
        System.Console.WriteLine("Введите количество белых и серых мышей, количество съеденных мышей, количество серых мышей, которые остались живы, количество белых мышей, которые остались живы:");
        string[] input = Console.ReadLine().Split();

        int N = int.Parse(input[0]);
        int M = int.Parse(input[1]);
        int S = int.Parse(input[2]);
        int K = int.Parse(input[3]);
        int L = int.Parse(input[4]);

        int total = N + M;
        int needLeft = K + L;

        List<int> mice = new List<int>();

        for (int i = 0; i < total; i++)
        {
            mice.Add(i);
        }

        List<int> eaten = new List<int>();

        int index = 0;

        while (mice.Count > needLeft)
        {
            index = (index + S - 1) % mice.Count;

            eaten.Add(mice[index]);

            mice.RemoveAt(index);
        }

        char[] result = new char[total];

        int grayEaten = N - K;
        int whiteEaten = M - L;
        int grayLeft = K;
        int whiteLeft = L;

        result[0] = 'G';

        if (eaten.Contains(0))
        {
            grayEaten--;
        }
        else
        {
            grayLeft--;
        }

        foreach (int pos in eaten)
        {
            if (result[pos] == '\0')
            {
                if (grayEaten > 0)
                {
                    result[pos] = 'G';
                    grayEaten--;
                }
                else
                {
                    result[pos] = 'W';
                    whiteEaten--;
                }
            }
        }

        foreach (int pos in mice)
        {
            if (result[pos] == '\0')
            {
                if (grayLeft > 0)
                {
                    result[pos] = 'G';
                    grayLeft--;
                }
                else
                {
                    result[pos] = 'W';
                    whiteLeft--;
                }
            }
        }

        Console.WriteLine("Порядок мышей по кругу:");
        Console.WriteLine(new string(result));
    }
}
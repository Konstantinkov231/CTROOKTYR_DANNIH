using System;

int n;

System.Console.WriteLine("Введите натуральное целое число >>>");
n = Convert.ToInt32(Console.ReadLine());

int sm = n;

        for (int i = 2; i * i <= n; i++)
        {
            if (n % i == 0)
            {
                sm = i;
                break;
            }
        }

        int a = n / sm;
        int b = n - a;

        Console.WriteLine($"Выполнил: Ковачёв Константин Владимирович;\n Группа: 020303-АИСа-о25;\n\n Результат: {a} {b}");
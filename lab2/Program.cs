using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

class Program
{
    const int N = 256;

    const int CblasRowMajor = 101;
    const int CblasNoTrans = 111;

    [DllImport("mkl_rt", CallingConvention = CallingConvention.Cdecl)]
    static extern void cblas_sgemm(
        int layout,
        int transa,
        int transb,
        int m,
        int n,
        int k,
        float alpha,
        float[] a,
        int lda,
        float[] b,
        int ldb,
        float beta,
        float[] c,
        int ldc
    );

    static void Main()
    {
        float[] A = new float[N * N];
        float[] B = new float[N * N];
        float[] C1 = new float[N * N];
        float[] C2 = new float[N * N];
        float[] C3 = new float[N * N];

        FillMatrix(A);
        FillMatrix(B);

        double c = 2.0 * N * N * N;

        Console.WriteLine($"Размер матриц: {N}x{N}");
        Console.WriteLine($"Количество операций: {c:F0}");
        Console.WriteLine();

        double t1 = RunTest("1 способ: обычное умножение", c, () =>
        {
            MultiplySimple(A, B, C1);
        });

        double t2 = RunTest("2 способ: BLAS cblas_sgemm", c, () =>
        {
            MultiplyBlas(A, B, C2);
        });

        double t3 = RunTest("3 способ: оптимизированное умножение", c, () =>
        {
            MultiplyOptimized(A, B, C3);
        });

        Console.WriteLine("Сравнение:");
        Console.WriteLine($"BLAS быстрее обычного способа в {t1 / t2:F2} раз");
        Console.WriteLine($"Оптимизированный способ быстрее обычного в {t1 / t3:F2} раз");

        double p2 = c / t2 * 0.000001;
        double p3 = c / t3 * 0.000001;

        Console.WriteLine($"3 способ от BLAS: {p3 / p2 * 100:F2}%");
    }

    static void FillMatrix(float[] matrix)
    {
        for (int i = 0; i < matrix.Length; i++)
        {
            matrix[i] = (i % 100) / 100.0f;
        }
    }

    static double RunTest(string name, double operations, Action method)
    {
        Stopwatch sw = Stopwatch.StartNew();

        method();

        sw.Stop();

        double time = sw.Elapsed.TotalSeconds;
        double mflops = operations / time * 0.000001;

        Console.WriteLine(name);
        Console.WriteLine($"Время: {time:F3} сек.");
        Console.WriteLine($"Производительность: {mflops:F2} MFlops");
        Console.WriteLine();

        return time;
    }

    static void MultiplySimple(float[] A, float[] B, float[] C)
    {
        for (int i = 0; i < N; i++)
        {
            for (int j = 0; j < N; j++)
            {
                float sum = 0;

                for (int k = 0; k < N; k++)
                {
                    sum += A[i * N + k] * B[k * N + j];
                }

                C[i * N + j] = sum;
            }
        }
    }

    static void MultiplyBlas(float[] A, float[] B, float[] C)
    {
        cblas_sgemm(
            CblasRowMajor,
            CblasNoTrans,
            CblasNoTrans,
            N,
            N,
            N,
            1.0f,
            A,
            N,
            B,
            N,
            0.0f,
            C,
            N
        );
    }

    static void MultiplyOptimized(float[] A, float[] B, float[] C)
    {
        int vectorSize = Vector<float>.Count;

        Parallel.For(0, N, i =>
        {
            int rowA = i * N;
            int rowC = i * N;

            for (int k = 0; k < N; k++)
            {
                float value = A[rowA + k];
                Vector<float> aVector = new Vector<float>(value);

                int rowB = k * N;

                int j = 0;

                for (; j <= N - vectorSize; j += vectorSize)
                {
                    Vector<float> bVector = new Vector<float>(B, rowB + j);
                    Vector<float> cVector = new Vector<float>(C, rowC + j);

                    cVector += aVector * bVector;

                    cVector.CopyTo(C, rowC + j);
                }

                for (; j < N; j++)
                {
                    C[rowC + j] += value * B[rowB + j];
                }
            }
        });
    }
}

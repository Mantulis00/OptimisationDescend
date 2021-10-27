﻿using System;
using System.Numerics;

namespace Optimisation
{

    public static class Optimizator
    {
        private static double DirectionA(double A, double B)
        {
            return -0.125 * B * (2 * A + B - 1);
        }
        private static double DirectionB(double A, double B)
        {
            return -0.125 * A * (2 * B + A - 1);
        }

        private static double Step(double A, double B)
        {
            return epsilon;
        }


        private static double VolumeByDistance(double A, double B, double d)
        {
            double dirA = DirectionA(A, B);
            double dirB = DirectionB(A, B);
            Vector2 dir = new Vector2((float)dirA, (float)dirB);
            float l = dir.Length();
            dir = new Vector2((float)dir.X / l, (float)dir.Y / l);

            return Volume(A + dir.X * d, B + dir.Y * d);
        }

        private static double optimisationEpsilon = 0.000000001;
        private static double StepOptimised(double A, double B)
        {
            double min = 0;
            double max =  Math.Sqrt(A * A + B * B);
            double from = max;

            double DV = Volume(A, B);
            double DVFrom = DV;

            double mid = (min + max) / 2;
            double d = 0.0001;

            double DVMax, DVMin;
            while (true)
            {
                mid = (min + max) / 2;
                DVMax = VolumeByDistance(A, B, (mid + max) / 2);
                DVMin = VolumeByDistance(A, B, (mid + min) / 2);
                if (Math.Abs(DVMax - DVMin) < optimisationEpsilon) break;

                if (DVFrom > DVMax && DVFrom > DVMin)
                {
                    max = mid;
                }
                else if (DVMax > DVMin)
                {
                    d = (mid + max) / 2;
                    min = mid;
                    if (Math.Abs(DVMax - DV) < optimisationEpsilon) break ;
                    DV = DVMax;
                }
                else
                {
                    d = (mid + min) / 2;
                    max = mid;
                    if (Math.Abs(DVMin - DV) < optimisationEpsilon) break;
                    DV = DVMin;
                }
            }
            return d;
        }



        private static double Volume (double A, double B)
        {
            return 0.125 * A * B*(1 - (A + B));
        }


        private static double epsilon = 1;
        private static bool ResultFound(double A, double B)
        {
            double dirA = DirectionA(A, B);
            double dirB = DirectionB(A, B);
            return Math.Sqrt(dirA*dirA + dirB*dirB) > epsilon/10000;
        }



        public static Vector2 Search(double A, double B)
        {
            double V=0;
            double a, b;
            int n = 0;

            Console.WriteLine(B.ToString().Replace(",", "."));
            while (ResultFound(A,B))
            {
                A = A + Step(A, B) * DirectionA(A, B);
                B = B + Step(A, B) * DirectionB(A, B);
                V = Volume(A,B);

                n++;
                Console.WriteLine(B.ToString().Replace(",", "."));
            }
            Console.WriteLine(String.Format("A = {0} B = {1} V = {2}", A, B, V));


            Console.WriteLine(n);

            return new Vector2((float)A, (float)B);
        }

        public static Vector2 SearchSteepest(double A, double B)
        {
            double V = 0;
            double a, b;
            int n = 0;

            Console.WriteLine(A.ToString().Replace(",", "."));
            while (ResultFound(A, B))
            {
                Vector2 dir = new Vector2((float)DirectionA(A, B), (float)DirectionB(A, B));
                float l = dir.Length();
                dir = new Vector2((float)DirectionA(A, B)/l, (float)DirectionB(A, B)/l);


                double d = StepOptimised(A, B) ;
                A = A + d * dir.X;
                B = B + d * dir.Y;
                V = Volume(A, B);

                n++;
                Console.WriteLine(A.ToString().Replace(",", "."));
            }
            Console.WriteLine(String.Format("A = {0} B = {1} V = {2}", A, B, V));


            Console.WriteLine(n);

            return new Vector2((float)A, (float)B);
        }
    }






    class Program
    {
        static void Main(string[] args)
        {
            Optimizator.Search(1, 1);
        }
    }
}
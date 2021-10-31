using System;
using System.Collections.Generic;
using System.Linq;
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
        private static double Volume(double A, double B)
        {
            return 0.125 * A * B * (1 - (A + B));
        }

        private static double Step(double A, double B)
        {
            return gamm;
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

        private static double optimisationEpsilon = 0.000001;
        private static double StepOptimised(double A, double B)
        {
            double min = 0;
            double max = Math.Sqrt(A * A + B * B);
            double from = max;

            double DV = Volume(A, B);
            double DVFrom = DV;

            double mid = (min + max) / 2;
            double d = 0.0001;

            double DVMax, DVMin;
            int n = 0;
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
                n++;
            }

            if (from < 1) from = 1;
            return d;
        }



        

        private static double gamm = 3;
        private static double endCondition = 0.001;
        private static bool ResultFound(double A, double B)
        {
            double dirA = DirectionA(A, B);
            double dirB = DirectionB(A, B);
            return Math.Sqrt(dirA*dirA + dirB*dirB) > endCondition;
        }

        private static bool ResultFoundS(double A, double B)
        {
            double dirA = DirectionA(A, B);
            double dirB = DirectionB(A, B);
            return Math.Sqrt(dirA * dirA + dirB * dirB) > endCondition;
        }



        public static Vector2 Search(double A, double B)
        {
            double V=0;
            int n = 0;

            Console.WriteLine(A.ToString().Replace(",", ".") + " " + B.ToString().Replace(",", "."));

            while (/*ResultFound(A,B)*/ true)
            {
                double a = A + Step(A, B) * DirectionA(A, B) ;
                double b = B + Step(A, B) * DirectionB(A, B) ;

                A = a;
                B = b;
                if (Math.Abs(V - Volume(A, B)) < endCondition) break;
                V = Volume(A,B);


                n++;
                Console.WriteLine(A.ToString().Replace(",", ".") + " " + B.ToString().Replace(",", "."));

            }
            //Console.WriteLine("V = " + Volume(A, B));
            Console.WriteLine("\n \n \n \n \n \n \n \n \n \n \n \n \n \n \n \n");

            Console.WriteLine("n = " + n);


            return new Vector2((float)A, (float)B);
        }

        public static Vector2 SearchSteepest(double A, double B)
        {
            double V = 0;
            double a, b;
            int n = 0;

            Console.WriteLine(A.ToString().Replace(",", ".") + " " + B.ToString().Replace(",", "."));

            while (n < 200 /*ResultFoundS(A, B)*/)
            {
                Vector2 dir = new Vector2((float)DirectionA(A, B), (float)DirectionB(A, B));
                float l = dir.Length();
                dir = new Vector2((float)DirectionA(A, B)/l, (float)DirectionB(A, B)/l);


                double d = StepOptimised(A, B) ;
                Console.WriteLine("d : " + d);
                A = A + d * dir.X;
                B = B + d * dir.Y;

                if (Math.Abs(V - Volume(A, B)) < endCondition/100) break;
                V = Volume(A, B);


               // Console.WriteLine(A.ToString().Replace(",", ".") + " " + B.ToString().Replace(",", "."));

                n++;
            }
             Console.WriteLine(String.Format("A = {0} B = {1} V = {2}", A, B, V));

            Console.WriteLine("\n \n \n \n \n \n \n \n \n \n \n \n \n \n \n \n");

            Console.WriteLine(n);

            return new Vector2((float)A, (float)B);
        }

        #region Simplex

        /// <summary>
        /// xh - taskas kur tikslo funkcijos reiksme didziausia
        /// xg - antra pagal dydi
        /// xl - maziausia
        /// xc - (xg+xl)/2
        /// </summary>


        private static Vector2 NewPoint(Vector2 xh, Vector2 xg, Vector2 xl)
        {
            Vector2 xsum = xg + xl;
            Vector2 xc = new Vector2(xsum.X / 2, xsum.Y / 2);

            Vector2 vector = xc - xh;
            Vector2 np = xc + new Vector2(vector.X, vector.Y);


            float lambda = GetStopSign(xh, xg, xl, np);

            return  xc + new Vector2(vector.X * lambda, vector.Y * lambda);
        }

        private static float GetStopSign(Vector2 xh, Vector2 xg, Vector2 xl, Vector2 np)
        {
            double Vh = Volume(xh.X, xh.Y);
            double Vl = Volume(xl.X, xl.Y);
            double Vg = Volume(xg.X, xg.Y);
            double Vnp = Volume(np.X, np.Y);



            if (Vg < Vnp && Vnp < Vl) return beta;
            if (Vnp > Vl) return gamma;
            if (Vnp < Vh) return fi;

            return beta;
        }

        private static List<Vector2> InitPoints(double A, double B, double size)
        {
            Vector2 dir = new Vector2((float)DirectionA(A, B), (float)DirectionB(A, B)); // faster type
            
            double l = dir.Length();
            dir = new Vector2((float)(DirectionA(A, B) / l * size), (float)(DirectionB(A, B) / l * size));

           

            Vector2 dirClock = new Vector2(dir.Y, - dir.X);
            Vector2 dirAntiClock = new Vector2(-dir.Y,dir.X);

            Vector2 p1 = new Vector2((float)A, (float)B);
            Vector2 p2 = p1 + new Vector2(dir.X + dirClock.X, dir.Y + dirClock.Y);
            Vector2 p3 = p1 + new Vector2(dir.X + dirAntiClock.X, dir.Y + dirAntiClock.Y);

            if (A == 0 && B == 0) // for finding solution where gradient is 0
            {
                p2 = p1 + new Vector2((float)(A + size), (float)B);
                p3 = p1 + new Vector2((float)(A), (float)(B + size));
            }

            return new List<Vector2>() { p1, p2, p3 };
        }



        private static float gamma = 2f, beta = 0.5f, fi = -0.5f;
        private static float alfa = 0.2f;

        public static Vector2 SearchSimplex(double A, double B)// start point
        {
            List<Vector2> Points = InitPoints(A, B, alfa);
            Points = Points.OrderBy(i => Volume(i.X, i.Y)).ToList();

            Vector2 np = Points[2];
            int n = 0;

            Console.WriteLine(A.ToString().Replace(",", ".") + " " + B.ToString().Replace(",", "."));
            while (n < 200)
            { 
                np = NewPoint(Points[0], Points[1], Points[2]);

                A = np.X;
                B = np.Y;
                Console.WriteLine(A.ToString().Replace(",", ".") +" " + B.ToString().Replace(",", "."));

                if ((np-Points[0]).Length() < endCondition) 
                    break;

                Points[0] = np;
                Points = Points.OrderBy(i => Volume(i.X, i.Y)).ToList();

                n++;
            }
            Console.Write(Volume(A, B));

            // Console.WriteLine("V = " + Volume(A, B));
            Console.WriteLine("\n \n \n \n \n \n \n \n \n \n \n \n \n \n \n \n");


            Console.WriteLine("n = " + n);

            return np;
        }


        #endregion




    }






    class Program
    {
        static void Main(string[] args)
        {
            Optimizator.SearchSimplex(0,0);
        }
    }
}

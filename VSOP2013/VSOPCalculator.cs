using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Numerics;
using System.Threading.Tasks;

namespace VSOP2013
{

    public class VSOPCalculator
    {
        PlanetData[] Data;
        //parameters
        double[] ci0, ci1;
        double[] freqpla;
        double[] gmp;
        double gmsol;
        const double dpi = 2 * Math.PI;
        const double a1000 = 365250.0d;

        public VSOPCalculator(PlanetData[] PlanetDataCollection)
        {
            //Import Planet Data
            this.Data = PlanetDataCollection;

            //Initial Constant
            Initci0();
            Initci1();
            Initfreqpla();
            Initgmp();
            Initgmsol();
        }

        private (double, double[]) SetTime(DateTime TDB)
        {
            //Convert to TDB J2000.            
            double tj = ToJulianDate2000(TDB) / a1000;
            //Iteration on Time 
            double[] t = new double[21];
            t[0] = 1.0d;
            t[1] = tj;
            for (int i = 2; i < 21; i++)
            {
                t[i] = t[1] * t[i - 1];
            }
            return (tj, t);
        }
        private double ToJulianDate2000(DateTime TDB)
        {
            //Input TDB
            double j2000 = 2451545.0d;
            //OADate + JD - j2000(JD) = TDB from J2000  
            return TDB.ToOADate() + 2415018.5d - j2000;
        }

        public PlanetEphemeris[] CalcAll(DateTime TDB)
        {
            PlanetEphemeris[] ephemerides = new PlanetEphemeris[9];

            ParallelLoopResult result = Parallel.For(0, 9, ip =>
            {
                ephemerides[ip] = CalcIP(ip, TDB);
            });
            return ephemerides;
        }

        public PlanetEphemeris CalcIP(int ip, DateTime TDB)
        {
            PlanetEphemeris Coordinate;
            Coordinate.DynamicalELL = new double[6];
            Coordinate.DynamicalXYZ = new double[6];
            Coordinate.ICRSXYZ = new double[6];
            double[] ELL = new double[6];
            ParallelLoopResult result = Parallel.For(0, 6, iv =>
            {
                ELL[iv] = CalcIV(ip, iv, TDB);
                Coordinate.DynamicalELL = ELL;
                Coordinate.DynamicalXYZ = ELLtoXYZ(ip, ELL);
                Coordinate.ICRSXYZ = DynamicaltoICRS(Coordinate.DynamicalXYZ);
            });
            return Coordinate;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ip">planet</param>
        /// <param name="iv">variable</param>
        /// <param name="TDB">time</param>
        /// <returns>Elliptic Elements</returns>
        public double CalcIV(int ip, int iv, DateTime TDB)
        {
            double tj;
            //Iteration on Time 
            double[] t;
            (tj, t) = SetTime(TDB);
            double r = 0d;
            double aa;
            double bb;
            double arg;
            double xl;
            for (int it = 0; it < 21; it++)
            {
                if (Data[ip].variables[iv].PowerTables[it].Terms == null) continue;
                for (int n = 0; n < Data[ip].variables[iv].PowerTables[it].Terms.Length; n++)
                {
                    aa = 0d;
                    bb = 0d;
                    for (int j = 0; j < 17; j++)
                    {
                        aa = aa + Data[ip].variables[iv].PowerTables[it].Terms[n].iphi[j] * ci0[j];
                        bb = bb + Data[ip].variables[iv].PowerTables[it].Terms[n].iphi[j] * ci1[j];
                    }
                    arg = aa + bb * t[1];
                    r = r + t[it]
                    * (Data[ip].variables[iv].PowerTables[it].Terms[n].ss * Math.Sin(arg)
                    + Data[ip].variables[iv].PowerTables[it].Terms[n].cc * Math.Cos(arg));
                }
            }
            if (iv == 1)
            {
                xl = r + freqpla[ip] * tj;
                xl = xl % dpi;
                if (xl < 0) xl = xl + dpi;
                r = xl;
            }
            return r;
        }

        /// <summary>
        /// This is kind of magic that I can't undersdand
        /// translate from FORTRAN code
        /// </summary>
        /// <param name="ip">planet</param>
        /// <param name="ELL">Elliptic Elements</param>
        /// <returns>Ecliptic Heliocentric Coordinates</returns>
        public double[] ELLtoXYZ(int ip, double[] ELL)
        {

            double[] w;
            Complex z, z1, z2, z3, zto, zteta;
            double rgm, xa, xl, xk, xh, xq, xp, xfi, xki;
            double u, ex, ex2, ex3, gl, gm, e, dl, rsa;
            double xcw, xsw, xm, xr, xms, xmc, xn;

            //Initialization
            rgm = Math.Sqrt(gmp[ip] + gmsol);
            w = new double[6];
            xa = ELL[0];
            xl = ELL[1];
            xk = ELL[2];
            xh = ELL[3];
            xq = ELL[4];
            xp = ELL[5];
            //Computation
            xfi = Math.Sqrt(1.0d - (xk * xk) - (xh * xh));
            xki = Math.Sqrt(1.0d - (xq * xq) - (xp * xp));
            u = 1.0d / (1.0d + xfi);
            z = new Complex(xk, xh);
            ex = z.Magnitude;
            ex2 = ex * ex;
            ex3 = ex2 * ex;
            z1 = Complex.Conjugate(z);
            gl = xl % dpi;
            gm = gl - Math.Atan2(xh, xk);
            e = gl + (ex - 0.125d * ex3) * Math.Sin(gm)
                + 0.5d * ex2 * Math.Sin(2.0d * gm)
                + 0.375d * ex3 * Math.Sin(3.0d * gm);

            while (true)
            {
                z2 = new Complex(0d, e);
                zteta = Complex.Exp(z2);
                z3 = z1 * zteta;
                dl = gl - e + z3.Imaginary;
                rsa = 1.0d - z3.Real;
                e = e + dl / rsa;
                if (Math.Abs(dl) < Math.Pow(10, -15)) break;
            }

            z1 = u * z * z3.Imaginary;
            z2 = new Complex(z1.Imaginary, -z1.Real);
            zto = (-z + zteta + z2) / rsa;
            xcw = zto.Real;
            xsw = zto.Imaginary;
            xm = xp * xcw - xq * xsw;
            xr = xa * rsa;

            w[0] = xr * (xcw - 2.0d * xp * xm);
            w[1] = xr * (xsw + 2.0d * xq * xm);
            w[2] = -2.0d * xr * xki * xm;
            xms = xa * (xh + xsw) / xfi;
            xmc = xa * (xk + xcw) / xfi;

            xn = rgm / Math.Pow(xa, 1.5d);
            w[3] = xn * ((2.0d * xp * xp - 1.0d) * xms + 2.0d * xp * xq * xmc);
            w[4] = xn * ((1.0d - 2.0d * xq * xq) * xmc - 2.0d * xp * xq * xms);
            w[5] = 2.0d * xn * xki * (xp * xms + xq * xmc);

            return w;
        }

        /// <summary>
        /// Another magic function
        /// </summary>
        /// <param name="w">Ecliptic Heliocentric Coordinates - Dynamical Frame J2000</param>
        /// <returns>Equatorial Heliocentric Coordinates - ICRS Frame J2000</returns>
        public double[] DynamicaltoICRS(double[] w)
        {
            double[] w2 = new double[6];
            //Rotation Matrix
            double[,] rot = new double[3, 3];
            double pi = Math.PI;
            double dgrad = pi / 180.0d;
            double sdrad = pi / 180.0d / 3600.0d;
            double eps = (23.0d + 26.0d / 60.0d + 21.411360d / 3600.0d) * dgrad;
            double phi = -0.051880d * sdrad;
            double ceps = Math.Cos(eps);
            double seps = Math.Sin(eps);
            double cphi = Math.Cos(phi);
            double sphi = Math.Sin(phi);
            rot[0, 0] = cphi;
            rot[0, 1] = -sphi * ceps;
            rot[0, 2] = sphi * seps;
            rot[1, 0] = sphi;
            rot[1, 1] = cphi * ceps;
            rot[1, 2] = -cphi * seps;
            rot[2, 0] = 0.0d;
            rot[2, 1] = seps;
            rot[2, 2] = ceps;

            //Computation
            for (int i = 0; i < 3; i++)
            {
                w2[i] = 0.0d;
                w2[i + 3] = 0.0d;
                for (int j = 0; j < 3; j++)
                {
                    w2[i] = w2[i] + rot[i, j] * w[j];
                    w2[i + 3] = w2[i + 3] + rot[i, j] * w[j + 3];
                }
            }
            return w2;
        }


        //Mean Longitude J2000 (radian)
        private void Initci0()
        {
            string[] data = {
            "0.4402608631669000d1",
            "0.3176134461576000d1",
            "0.1753470369433000d1",
            "0.6203500014141000d1",
            "0.4091360003050000d1",
            "0.1713740719173000d1",
            "0.5598641292287000d1",
            "0.2805136360408000d1",
            "0.2326989734620000d1",
            "0.5995461070350000d0",
            "0.8740185101070000d0",
            "0.5481225395663000d1",
            "0.5311897933164000d1",
            "0.0d0",
            "5.19846640063d0",
            "1.62790513602d0",
            "2.35555563875d0"};

            ci0 = new double[17];
            for (int i = 0; i < ci0.Length; i++)
                ci0[i] = toDouble(data[i]);

        }

        //Mean Motions in longitude (radian/cy)
        private void Initci1()
        {
            string[] data = {
            "0.2608790314068555d5",
            "0.1021328554743445d5",
            "0.6283075850353215d4",
            "0.3340612434145457d4",
            "0.1731170452721855d4",
            "0.1704450855027201d4",
            "0.1428948917844273d4",
            "0.1364756513629990d4",
            "0.1361923207632842d4",
            "0.5296909615623250d3",
            "0.2132990861084880d3",
            "0.7478165903077800d2",
            "0.3813297222612500d2",
            "0.3595362285049309d0",
            "77713.7714481804d0",
            "84334.6615717837d0",
            "83286.9142477147d0"};

            ci1 = new double[17];
            for (int i = 0; i < ci1.Length; i++)
                ci1[i] = toDouble(data[i]);

        }

        //Planetary frequency in longitude
        private void Initfreqpla()
        {
            string[] data = {
            "0.2608790314068555d5",
            "0.1021328554743445d5",
            "0.6283075850353215d4",
            "0.3340612434145457d4",
            "0.5296909615623250d3",
            "0.2132990861084880d3",
            "0.7478165903077800d2",
            "0.3813297222612500d2",
            "0.2533566020437000d2"
            };

            freqpla = new double[9];
            for (int i = 0; i < freqpla.Length; i++)
                freqpla[i] = toDouble(data[i]);

        }

        //Masses system
        private void Initgmp()
        {
            string[] data = {
            "4.9125474514508118699d-11",
            "7.2434524861627027000d-10",
            "8.9970116036316091182d-10",
            "9.5495351057792580598d-11",
            "2.8253458420837780000d-07",
            "8.4597151856806587398d-08",
            "1.2920249167819693900d-08",
            "1.5243589007842762800d-08",
            "2.1886997654259696800d-12"
            };

            gmp = new double[9];
            for (int i = 0; i < gmp.Length; i++)
            {
                gmp[i] = toDouble(data[i]);
            }
        }

        private void Initgmsol()
        {
            string data = "2.9591220836841438269d-04";
            gmsol = toDouble(data);
        }

        private double toDouble(string s)
        {
            string[] buffer = s.Split('d');
            return Convert.ToDouble(buffer[0]) * (Math.Pow(10, Convert.ToDouble(buffer[1])));
        }
    }
}

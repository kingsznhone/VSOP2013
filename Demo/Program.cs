using System;
using System.Globalization;
using VSOP2013;
namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime UTC;
            PlanetEphemeris[] ephemerides;

            //Load Data From Disk
            string Path = @"D:\VSOP2013DATA";
            DataReader dr = new DataReader(Path);
            VSOPCalculator v2013 = new VSOPCalculator(dr.ReadData());
            Console.WriteLine("Planet Data Load OK...");


            Console.WriteLine("Parse UTC string that conforms to ISO 8601:  2018-08-18T07:22:16.0000000Z");
            //Parse Time
            while (true)
            {
                Console.Write("Input Time As UTC:");
                //string inputT = Console.ReadLine();
                string inputT = "2000-01-01T12:00:00.0000000Z";
                CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US"); 
                DateTimeStyles style= DateTimeStyles.AdjustToUniversal;
                if (DateTime.TryParse(inputT,culture,style, out UTC)) break;
                else Console.WriteLine("Invalid Entry...");
            }

            //Convert UTC to TDB (Barycentric Dynamical Time)
            DateTime TDB = TimeConverter.UTCtoTDB(UTC);

            Console.WriteLine();

            Console.WriteLine("Press Enter To Start Substitution...");
            Console.ReadLine();
            
            
            #region
            //Calculate specific planet. Earth=2
            PlanetEphemeris r = v2013.CalcIP((int)Body.EMB, UTC);
            Console.WriteLine("===============================================================");
            Console.WriteLine("{0} at {1}",Enum.GetName(typeof(Body), 2), UTC.ToString());
            Console.WriteLine();
            Console.WriteLine("            Elliptic Elements - Dynamical Frame J2000");
            Console.WriteLine(String.Format("{0,-30} : {1,-30}", "semi-major axis (au)", r.DynamicalELL[0]));
            Console.WriteLine(String.Format("{0,-30} : {1,-30}", "mean longitude (rd)", r.DynamicalELL[1]));
            Console.WriteLine(String.Format("{0,-30} : {1,-30}", "k = e*cos(pi) (rd)", r.DynamicalELL[2]));
            Console.WriteLine(String.Format("{0,-30} : {1,-30}", "h = e*sin(pi) (rd)", r.DynamicalELL[3]));
            Console.WriteLine(String.Format("{0,-30} : {1,-30}", "q = sin(i/2)*cos(omega) (rd)", r.DynamicalELL[4]));
            Console.WriteLine(String.Format("{0,-30} : {1,-30}", "p = sin(i/2)*sin(omega) (rd)", r.DynamicalELL[5]));
            Console.WriteLine();
            Console.WriteLine("            Ecliptic Heliocentric Coordinates - Dynamical Frame J2000");
            Console.WriteLine(String.Format("{0,-30} : {1,-30}", "Positions  X (au)", r.DynamicalXYZ[0]));
            Console.WriteLine(String.Format("{0,-30} : {1,-30}", "Positions  Y (au)", r.DynamicalXYZ[1]));
            Console.WriteLine(String.Format("{0,-30} : {1,-30}", "Positions  Z (au)", r.DynamicalXYZ[2]));
            Console.WriteLine(String.Format("{0,-30} : {1,-30}", "Velocities X'(au/d)", r.DynamicalXYZ[3]));
            Console.WriteLine(String.Format("{0,-30} : {1,-30}", "Velocities Y'(au/d)", r.DynamicalXYZ[4]));
            Console.WriteLine(String.Format("{0,-30} : {1,-30}", "Velocities Z'(au/d)", r.DynamicalXYZ[5]));
            Console.WriteLine();
            Console.WriteLine("            Equatorial Heliocentric Coordinates - ICRS Frame J2000");
            Console.WriteLine(String.Format("{0,-30} : {1,-30}", "Positions  X (au)", r.ICRSXYZ[0]));
            Console.WriteLine(String.Format("{0,-30} : {1,-30}", "Positions  Y (au)", r.ICRSXYZ[1]));
            Console.WriteLine(String.Format("{0,-30} : {1,-30}", "Positions  Z (au)", r.ICRSXYZ[2]));
            Console.WriteLine(String.Format("{0,-30} : {1,-30}", "Velocities X'(au/d)", r.ICRSXYZ[3]));
            Console.WriteLine(String.Format("{0,-30} : {1,-30}", "Velocities Y'(au/d)", r.ICRSXYZ[4]));
            Console.WriteLine(String.Format("{0,-30} : {1,-30}", "Velocities Z'(au/d)", r.ICRSXYZ[5]));
            Console.WriteLine("===============================================================");
            ////
            #endregion

            //Calculate Each Variable 
            double R = v2013.CalcIV(2, 0, TDB);
            Console.WriteLine("===============================================================");
            Console.WriteLine(Enum.GetName(typeof(Body), 2) + " at " + UTC.ToString());
            Console.WriteLine();
            Console.WriteLine("            Elliptic Elements - Dynamical Frame J2000");
            Console.WriteLine(String.Format("{0,-30} : {1,-30}", "semi-major axis (au)", R));
            Console.WriteLine("===============================================================");
            ////
            
            //Calculate All Planet
            ephemerides = v2013.CalcAll(TDB);
            PrintResult(ephemerides, TDB);
            Console.ReadLine();
            ////

        }

        public static void PrintResult(PlanetEphemeris[] Ephemeris, DateTime time)
        {
            Console.WriteLine("===============================================================");
            Console.WriteLine("PLANETARY EPHEMERIS VSOP2013");
            Console.WriteLine("===============================================================");
            for (int i = 0; i < 9; i++)
            {
                Console.WriteLine(Enum.GetName(typeof(Body), i) + " at TDB:" + time.ToString());
                Console.WriteLine();
                Console.WriteLine("            Elliptic Elements - Dynamical Frame J2000");
                Console.WriteLine(String.Format("{0,-30} : {1,-30}", "semi-major axis (au)", Ephemeris[i].DynamicalELL[0]));
                Console.WriteLine(String.Format("{0,-30} : {1,-30}", "mean longitude (rd)", Ephemeris[i].DynamicalELL[1]));
                Console.WriteLine(String.Format("{0,-30} : {1,-30}", "k = e*cos(pi) (rd)", Ephemeris[i].DynamicalELL[2]));
                Console.WriteLine(String.Format("{0,-30} : {1,-30}", "h = e*sin(pi) (rd)", Ephemeris[i].DynamicalELL[3]));
                Console.WriteLine(String.Format("{0,-30} : {1,-30}", "q = sin(i/2)*cos(omega) (rd)", Ephemeris[i].DynamicalELL[4]));
                Console.WriteLine(String.Format("{0,-30} : {1,-30}", "p = sin(i/2)*sin(omega) (rd)", Ephemeris[i].DynamicalELL[5]));
                Console.WriteLine();
                Console.WriteLine("            Ecliptic Heliocentric Coordinates - Dynamical Frame J2000");
                Console.WriteLine(String.Format("{0,-30} : {1,-30}", "Positions  X (au)", Ephemeris[i].DynamicalXYZ[0]));
                Console.WriteLine(String.Format("{0,-30} : {1,-30}", "Positions  Y (au)", Ephemeris[i].DynamicalXYZ[1]));
                Console.WriteLine(String.Format("{0,-30} : {1,-30}", "Positions  Z (au)", Ephemeris[i].DynamicalXYZ[2]));
                Console.WriteLine(String.Format("{0,-30} : {1,-30}", "Velocities X'(au/d)", Ephemeris[i].DynamicalXYZ[3]));
                Console.WriteLine(String.Format("{0,-30} : {1,-30}", "Velocities Y'(au/d)", Ephemeris[i].DynamicalXYZ[4]));
                Console.WriteLine(String.Format("{0,-30} : {1,-30}", "Velocities Z'(au/d)", Ephemeris[i].DynamicalXYZ[5]));
                Console.WriteLine();
                Console.WriteLine("            Equatorial Heliocentric Coordinates - ICRS Frame J2000");
                Console.WriteLine(String.Format("{0,-30} : {1,-30}", "Positions  X (au)", Ephemeris[i].ICRSXYZ[0]));
                Console.WriteLine(String.Format("{0,-30} : {1,-30}", "Positions  Y (au)", Ephemeris[i].ICRSXYZ[1]));
                Console.WriteLine(String.Format("{0,-30} : {1,-30}", "Positions  Z (au)", Ephemeris[i].ICRSXYZ[2]));
                Console.WriteLine(String.Format("{0,-30} : {1,-30}", "Velocities X'(au/d)", Ephemeris[i].ICRSXYZ[3]));
                Console.WriteLine(String.Format("{0,-30} : {1,-30}", "Velocities Y'(au/d)", Ephemeris[i].ICRSXYZ[4]));
                Console.WriteLine(String.Format("{0,-30} : {1,-30}", "Velocities Z'(au/d)", Ephemeris[i].ICRSXYZ[5]));
                Console.WriteLine("===============================================================");

            }
            Console.ReadLine();
        }
    
    }
}

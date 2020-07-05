using System;
using VSOP2013;
namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime UTC;
            PlanetEphemeris[] ephemerides;

            //Load Data
            string Path = @"C:\VSOPDATA";
            DataReader dr = new DataReader(Path);
            VSOPCalculator v = new VSOPCalculator(dr.ReadData());
            Console.WriteLine("Planet Data Load OK...");


            Console.WriteLine("UTC string that conforms to ISO 8601:  2018-08-18T07:22:16.0000000Z");
            //Parse Time
            while (true)
            {
                Console.Write("Input Time As UTC:");
                //string inputT = Console.ReadLine();
                string inputT = "2000-01-01T12:00:00.0000000Z";
                //Time = DateTime.FromOADate(2411545.0d - 2415018.5d);
                if (DateTime.TryParse(inputT, out UTC)) break;
                else Console.WriteLine("Invalid Entry...");
            }
            UTC = UTC.ToUniversalTime();
            DateTime TDB = TimeConverter.UTCtoTDB(UTC);

            Console.WriteLine();

            Console.WriteLine("Press Enter To Start Substitution...");
            Console.ReadLine();

            //Calculate Each Planet. Earth=2
            PlanetEphemeris r = v.CalcIP(2, UTC);
            Console.WriteLine("===============================================================");
            Console.WriteLine(Enum.GetName(typeof(Body), 2) + " at " + UTC.ToString());
            Console.WriteLine();
            Console.WriteLine("            Elliptic Elements");
            Console.WriteLine(String.Format("{0,-30} : {1,-30}", "semi-major axis (au)", r.DynamicalELL[0]));
            Console.WriteLine(String.Format("{0,-30} : {1,-30}", "mean longitude (rd)", r.DynamicalELL[1]));
            Console.WriteLine(String.Format("{0,-30} : {1,-30}", "k = e*cos(pi) (rd)", r.DynamicalELL[2]));
            Console.WriteLine(String.Format("{0,-30} : {1,-30}", "h = e*sin(pi) (rd)", r.DynamicalELL[3]));
            Console.WriteLine(String.Format("{0,-30} : {1,-30}", "q = sin(i/2)*cos(omega) (rd)", r.DynamicalELL[4]));
            Console.WriteLine(String.Format("{0,-30} : {1,-30}", "p = sin(i/2)*sin(omega) (rd)", r.DynamicalELL[5]));
            Console.WriteLine();
            Console.WriteLine("            Ecliptic Heliocentric Coordinates");
            Console.WriteLine(String.Format("{0,-30} : {1,-30}", "Positions  X (au)", r.DynamicalXYZ[0]));
            Console.WriteLine(String.Format("{0,-30} : {1,-30}", "Positions  Y (au)", r.DynamicalXYZ[1]));
            Console.WriteLine(String.Format("{0,-30} : {1,-30}", "Positions  Z (au)", r.DynamicalXYZ[2]));
            Console.WriteLine(String.Format("{0,-30} : {1,-30}", "Velocities X'(au/d)", r.DynamicalXYZ[3]));
            Console.WriteLine(String.Format("{0,-30} : {1,-30}", "Velocities Y'(au/d)", r.DynamicalXYZ[4]));
            Console.WriteLine(String.Format("{0,-30} : {1,-30}", "Velocities Z'(au/d)", r.DynamicalXYZ[5]));
            Console.WriteLine();
            Console.WriteLine("            Equatorial Heliocentric Coordinates");
            Console.WriteLine(String.Format("{0,-30} : {1,-30}", "Positions  X (au)", r.ICRSXYZ[0]));
            Console.WriteLine(String.Format("{0,-30} : {1,-30}", "Positions  Y (au)", r.ICRSXYZ[1]));
            Console.WriteLine(String.Format("{0,-30} : {1,-30}", "Positions  Z (au)", r.ICRSXYZ[2]));
            Console.WriteLine(String.Format("{0,-30} : {1,-30}", "Velocities X'(au/d)", r.ICRSXYZ[3]));
            Console.WriteLine(String.Format("{0,-30} : {1,-30}", "Velocities Y'(au/d)", r.ICRSXYZ[4]));
            Console.WriteLine(String.Format("{0,-30} : {1,-30}", "Velocities Z'(au/d)", r.ICRSXYZ[5]));
            Console.WriteLine("===============================================================");
            ////
            
            //Calculate Each Variable 
            double R = v.CalcIV(2, 0, UTC);
            Console.WriteLine("===============================================================");
            Console.WriteLine(Enum.GetName(typeof(Body), 2) + " at " + UTC.ToString());
            Console.WriteLine();
            Console.WriteLine("            Elliptic Elements");
            Console.WriteLine(String.Format("{0,-30} : {1,-30}", "semi-major axis (au)", r.DynamicalELL[0]));
            Console.WriteLine("===============================================================");
            ////
            
            //Calculate All Planet
            ephemerides = v.CalcAll(TDB);
            PrintResult(ephemerides, TDB);
            Console.ReadLine();
            ////

        }

        public static void PrintResult(PlanetEphemeris[] PlanetStats, DateTime time)
        {
            Console.WriteLine("===============================================================");
            Console.WriteLine("PLANETARY EPHEMERIS VSOP2013");
            Console.WriteLine("===============================================================");
            for (int i = 0; i < 9; i++)
            {
                Console.WriteLine(Enum.GetName(typeof(Body), i) + " at TDB:" + time.ToString());
                Console.WriteLine();
                Console.WriteLine("            Elliptic Elements");
                Console.WriteLine(String.Format("{0,-30} : {1,-30}", "semi-major axis (au)", PlanetStats[i].DynamicalELL[0]));
                Console.WriteLine(String.Format("{0,-30} : {1,-30}", "mean longitude (rd)", PlanetStats[i].DynamicalELL[1]));
                Console.WriteLine(String.Format("{0,-30} : {1,-30}", "k = e*cos(pi) (rd)", PlanetStats[i].DynamicalELL[2]));
                Console.WriteLine(String.Format("{0,-30} : {1,-30}", "h = e*sin(pi) (rd)", PlanetStats[i].DynamicalELL[3]));
                Console.WriteLine(String.Format("{0,-30} : {1,-30}", "q = sin(i/2)*cos(omega) (rd)", PlanetStats[i].DynamicalELL[4]));
                Console.WriteLine(String.Format("{0,-30} : {1,-30}", "p = sin(i/2)*sin(omega) (rd)", PlanetStats[i].DynamicalELL[5]));
                Console.WriteLine();
                Console.WriteLine("            Ecliptic Heliocentric Coordinates");
                Console.WriteLine(String.Format("{0,-30} : {1,-30}", "Positions  X (au)", PlanetStats[i].DynamicalXYZ[0]));
                Console.WriteLine(String.Format("{0,-30} : {1,-30}", "Positions  Y (au)", PlanetStats[i].DynamicalXYZ[1]));
                Console.WriteLine(String.Format("{0,-30} : {1,-30}", "Positions  Z (au)", PlanetStats[i].DynamicalXYZ[2]));
                Console.WriteLine(String.Format("{0,-30} : {1,-30}", "Velocities X'(au/d)", PlanetStats[i].DynamicalXYZ[3]));
                Console.WriteLine(String.Format("{0,-30} : {1,-30}", "Velocities Y'(au/d)", PlanetStats[i].DynamicalXYZ[4]));
                Console.WriteLine(String.Format("{0,-30} : {1,-30}", "Velocities Z'(au/d)", PlanetStats[i].DynamicalXYZ[5]));
                Console.WriteLine();
                Console.WriteLine("            Equatorial Heliocentric Coordinates");
                Console.WriteLine(String.Format("{0,-30} : {1,-30}", "Positions  X (au)", PlanetStats[i].ICRSXYZ[0]));
                Console.WriteLine(String.Format("{0,-30} : {1,-30}", "Positions  Y (au)", PlanetStats[i].ICRSXYZ[1]));
                Console.WriteLine(String.Format("{0,-30} : {1,-30}", "Positions  Z (au)", PlanetStats[i].ICRSXYZ[2]));
                Console.WriteLine(String.Format("{0,-30} : {1,-30}", "Velocities X'(au/d)", PlanetStats[i].ICRSXYZ[3]));
                Console.WriteLine(String.Format("{0,-30} : {1,-30}", "Velocities Y'(au/d)", PlanetStats[i].ICRSXYZ[4]));
                Console.WriteLine(String.Format("{0,-30} : {1,-30}", "Velocities Z'(au/d)", PlanetStats[i].ICRSXYZ[5]));
                Console.WriteLine("===============================================================");

            }
            Console.ReadLine();
        }
    }
}

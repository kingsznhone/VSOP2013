﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VSOP2013
{

    public struct Header
    {
        public int ip;
        public int iv;
        public int it;
        public int nt;
    }
    public struct PlanetData
    {
        public Variable[] variables;
    }
    public struct Variable
    {
        public Power[] PowerTables;
    }
    public struct Power
    {
        public Term[] Terms;
    }
    public struct Term
    {
        public int rank;
        public int[] iphi;
        public double ss;
        public double cc;
    }

    public class DataReader
    {
        string Path;
        public PlanetData[] Solarsystem;
        
        public DataReader(string Path)
        {
            this.Path = Path;
            Solarsystem = new PlanetData[9];
            for (int ip = 0; ip < 9; ip++)
            {
                Solarsystem[ip].variables = new Variable[6];
                for (int iv = 0; iv < 6; iv++)
                {
                    Solarsystem[ip].variables[iv].PowerTables = new Power[21];
                }
            }
        }


        public PlanetData[] ReadData()
        {
            ParallelLoopResult result = Parallel.For(0, 9, ip => 
            {
                ReadPlanet(ip);
            });
            return Solarsystem;
        }

        private void ReadPlanet(int ip)
        {
            StreamReader sr;
            Header H = new Header();
            string line;
            {
                //  C:\VSOPDATA\VSOP2013p1.dat
                sr = new StreamReader(Path + @"\" + Enum.GetName(typeof(DataFile), ip) + ".dat");
                while ((line = sr.ReadLine()) != null)
                {
                    ReadHeader(line, ref H);
                    Term[] buffer = new Term[H.nt];
                    for (int i = 0; i < H.nt; i++)
                    {
                        line = sr.ReadLine();
                        buffer[i] = ReadTerm(line);
                    }

                    Solarsystem[H.ip].variables[H.iv].PowerTables[H.it].Terms = buffer;
                }
            }
            sr.Close();
        }
        private void ReadHeader(string line, ref Header H)
        {

            int lineptr = 9;
            H.ip = Convert.ToInt32(line.Substring(lineptr, 3).Trim()) - 1;
            lineptr += 3;
            H.iv = Convert.ToInt32(line.Substring(lineptr, 3).Trim()) - 1;
            lineptr += 3;
            H.it = Convert.ToInt32(line.Substring(lineptr, 3).Trim());
            lineptr += 3;
            H.nt = Convert.ToInt32(line.Substring(lineptr, 7).Trim());
        }
        private Term ReadTerm(string line)
        {
            int lineptr;
            Term T;
            int[] Bufferiphi = new int[17];
            int index = 0;
            double ci = 0d;

            //
            lineptr = 5;
            T.rank = Convert.ToInt32(line.Substring(0, lineptr));
            //
            lineptr++;
            //
            for (int counter = 0; counter < 4; counter++)
            {
                Bufferiphi[index] = Convert.ToInt32(line.Substring(lineptr, 3).Trim());
                index++;
                lineptr += 3;
            }
            //
            lineptr++;
            //
            for (int counter = 0; counter < 5; counter++)
            {
                Bufferiphi[index] = Convert.ToInt32(line.Substring(lineptr, 3).Trim());
                index++;
                lineptr += 3;
            }
            //
            lineptr++;
            //
            for (int counter = 0; counter < 4; counter++)
            {
                Bufferiphi[index] = Convert.ToInt32(line.Substring(lineptr, 4).Trim());
                index++;
                lineptr += 4;
            }
            //
            lineptr++;
            //
            Bufferiphi[index] = Convert.ToInt32(line.Substring(lineptr, 6).Trim());
            index++;
            lineptr += 6;
            //
            lineptr++;
            //
            for (int counter = 0; counter < 3; counter++)
            {
                Bufferiphi[index] = Convert.ToInt32(line.Substring(lineptr, 3).Trim());
                index++;
                lineptr += 3;
            }

            T.iphi = Bufferiphi;


            ci = Convert.ToDouble(line.Substring(lineptr, 20).Trim());
            lineptr += 20;
            lineptr++;
            ci = ci * Math.Pow(10, Convert.ToDouble(line.Substring(lineptr, 3).Trim()));
            lineptr += 3;
            T.ss = ci;

            ci = Convert.ToDouble(line.Substring(lineptr, 20).Trim());
            lineptr += 20;
            lineptr++;
            ci = ci * Math.Pow(10, Convert.ToDouble(line.Substring(lineptr, 3).Trim()));
            //lineptr += 3;
            T.cc = ci;

            return T;
        }

    }
}


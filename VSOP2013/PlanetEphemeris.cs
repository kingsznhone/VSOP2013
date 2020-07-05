using System;
using System.Collections.Generic;
using System.Text;

namespace VSOP2013
{
    public struct PlanetEphemeris
    {
        //Elliptic   Elements 
        //a (au), lambda (radian), k, h, q, p
        //Dynamical Frame J2000'
        public double[] DynamicalELL;

        //Ecliptic   Heliocentric Coordinates
        //X,Y,Z (au)  X'',Y'',Z'' (au/d)
        //Dynamical Frame J2000'
        public double[] DynamicalXYZ;

        //Equatorial Heliocentric Coordinates:
        //X,Y,Z (au)  X'',Y'',Z'' (au/d)
        //ICRS Frame J2000
        public double[] ICRSXYZ;
    }
}

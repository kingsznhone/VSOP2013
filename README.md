# VSOP2013 .NET Core Remastered

G. FRANCOU & J.-L. SIMON (MAY 2013) 
 
Ref: Simon J.-L., Francou G., Fienga A., Manche H., A&A 557, A49 (2013) 

## INTRODUCTION 
 
The VSOP2013 files contain the series of the elliptic elements for the 8 planets 

Mercury, Venus, Earth-Moon barycenter, Mars, Jupiter, Saturn, Uranus, Neptune and for the dwarf planet Pluto of the solution VSOP2013.  
 
List of the files: 

VSOP2013p1.dat : Mercury  

VSOP2013p2.dat : Venus  

VSOP2013p3.dat : Earth-Moon Barycenter 

VSOP2013p4.dat : Mars  

VSOP2013p5.dat : Jupiter  

VSOP2013p6.dat : Saturn  

VSOP2013p7.dat : Uranus  

VSOP2013p8.dat : Neptune  

VSOP2013p9.dat : Pluto 
 
The planetary solution VSOP2013 is fitted to the numerical integration INPOP10a built at IMCCE, Paris Observatory (http://www.imcce.fr/inpop/).   


## FILES DESCRIPTION 
 
### Each VSOP2013 file corresponds to a planet and contains trigonometric series, 

functions of time (Periodic series and Poisson series), that represent the 6 elliptic elements of the planet:  

Variable 1 : a = semi-major axis (ua)  

Variable 2 : λ = mean longitude  (radian)  

Variable 3 : k = e cos ω

Variable 4 : h = e sin ω

Variable 5 : q = sin(i/2) cos Ω

Variable 6 : p = sin(i/2) sin Ω 
 
### with: 

e : eccentricity  

ω : perihelion longitude 

i : inclination 

Ω : ascending node longitude 
 
### VSOP2013 series are characterized by 3 parameters: 

- the planet index 1-9 from Mercury to Pluto,  

- the variable index 1-6 for a, , k, h, q, p, 

- the time power α.


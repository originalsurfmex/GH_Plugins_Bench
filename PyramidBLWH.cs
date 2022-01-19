using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Plugin_1_Day_3
{
      /// <summary>
      /// Pyramid - Base, Length, Width, Height
      /// </summary>
      public class PyramidBLWH
      {
        public Plane BasePlane;
        public double Length;
        public double Width;
        public double Height;
    
        /// <summary>
        /// Pyramid - Base, Length, Width, Height
        /// </summary>
        /// <param name="basePlane"> user provided Base Plane </param>
        /// <param name="length"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public PyramidBLWH(Plane basePlane, double length, double width, double height)
        {
          BasePlane = basePlane;
          Length = length;
          Width = width;
          Height = height;
        }
        
        /// <summary>
        /// Pyramid - World XY Base Plane, Length, Width, Height
        /// </summary>
        /// <param name="length"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public PyramidBLWH(double length, double width, double height)
        {
          BasePlane = Plane.WorldXY;
          Length = length;
          Width = width;
          Height = height;
        }
        
        /// <summary>
        /// Pyramid - World XY Base Plane, Length = 1.0, Width = 1.0, Height = 1.0;
        /// </summary>
        public PyramidBLWH() // DEFAULT constructor
        {
          BasePlane = Plane.WorldXY;
          Length = 1.0;
          Width = 1.0;
          Height = 1.0;
        }
    
        /// <summary>
        /// Returns 1/3 L*W*H, this is done by multiplying 0.3333333
        /// </summary>
        /// <returns></returns>
        public double ComputeVolume()
        {
          return 0.3333333 * Length * Width * Height;
        }

        public List<LineCurve> ComputeDisplayLines()
        {
          // BASE POINTS!
          Point3d A = BasePlane.Origin + BasePlane.XAxis * Length * 0.5 + BasePlane.YAxis * Width * 0.5;
          Point3d B = BasePlane.Origin - BasePlane.XAxis * Length * 0.5 + BasePlane.YAxis * Width * 0.5;
          Point3d C = BasePlane.Origin - BasePlane.XAxis * Length * 0.5 - BasePlane.YAxis * Width * 0.5;
          Point3d D = BasePlane.Origin + BasePlane.XAxis * Length * 0.5 - BasePlane.YAxis * Width * 0.5;
          // PEAK!
          Point3d M = BasePlane.Origin + BasePlane.ZAxis * Height;

          var displayLines = new List<LineCurve>();
          // DRAW THE BASE!
          displayLines.Add(new LineCurve(A,B));
          displayLines.Add(new LineCurve(B,C));
          displayLines.Add(new LineCurve(C,D));
          displayLines.Add(new LineCurve(D,A));
          // DRAW THE CORNERS TO PEAK!
          displayLines.Add(new LineCurve(A,M));
          displayLines.Add(new LineCurve(B,M));
          displayLines.Add(new LineCurve(C,M));
          displayLines.Add(new LineCurve(D,M));

          return displayLines;
        }

        public List<Surface> ComputeSurfaces()
        {
          // BASE POINTS!
          Point3d A = BasePlane.Origin + BasePlane.XAxis * Length * 0.5 + BasePlane.YAxis * Width * 0.5;
          Point3d B = BasePlane.Origin - BasePlane.XAxis * Length * 0.5 + BasePlane.YAxis * Width * 0.5;
          Point3d C = BasePlane.Origin - BasePlane.XAxis * Length * 0.5 - BasePlane.YAxis * Width * 0.5;
          Point3d D = BasePlane.Origin + BasePlane.XAxis * Length * 0.5 - BasePlane.YAxis * Width * 0.5;
          // PEAK!
          Point3d M = BasePlane.Origin + BasePlane.ZAxis * Height;

          var displaySurf = new List<Surface>();

          displaySurf.AddRange(new List<Surface>
          {
            NurbsSurface.CreateFromCorners(A, B, C, D),
            NurbsSurface.CreateFromCorners(A, B, M),
            NurbsSurface.CreateFromCorners(B, C, M),
            NurbsSurface.CreateFromCorners(C, D, M),
            NurbsSurface.CreateFromCorners(D, A, M)
          });

          return displaySurf;
        }
        
        public List<Point3d> OutPoints()
                {
                  // BASE POINTS!
                  Point3d A = BasePlane.Origin + BasePlane.XAxis * Length * 0.5 + BasePlane.YAxis * Width * 0.5;
                  Point3d B = BasePlane.Origin - BasePlane.XAxis * Length * 0.5 + BasePlane.YAxis * Width * 0.5;
                  Point3d C = BasePlane.Origin - BasePlane.XAxis * Length * 0.5 - BasePlane.YAxis * Width * 0.5;
                  Point3d D = BasePlane.Origin + BasePlane.XAxis * Length * 0.5 - BasePlane.YAxis * Width * 0.5;
                  // PEAK!
                  Point3d M = BasePlane.Origin + BasePlane.ZAxis * Height;
        
                  var outPts = new List<Point3d>();
                  outPts.AddRange(new List<Point3d>
                  {
                    new Point3d(A),
                    new Point3d(B),
                    new Point3d(C),
                    new Point3d(D),
                    new Point3d(M)
                  });

                  return outPts;
                }
      }
}
using System.Collections.Generic;
using Rhino.Geometry;

namespace WorkBench
{
      /// <summary>
      /// Pyramid - Base, Length, Width, Height
      /// </summary>
      public class PyramidBlwh
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
        public PyramidBlwh(Plane basePlane, double length, double width, double height)
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
        public PyramidBlwh(double length, double width, double height)
        {
          BasePlane = Plane.WorldXY;
          Length = length;
          Width = width;
          Height = height;
        }
        
        /// <summary>
        /// Pyramid - World XY Base Plane, Length = 1.0, Width = 1.0, Height = 1.0;
        /// </summary>
        public PyramidBlwh() // DEFAULT constructor
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
          Point3d a = BasePlane.Origin + BasePlane.XAxis * Length * 0.5 + BasePlane.YAxis * Width * 0.5;
          Point3d b = BasePlane.Origin - BasePlane.XAxis * Length * 0.5 + BasePlane.YAxis * Width * 0.5;
          Point3d c = BasePlane.Origin - BasePlane.XAxis * Length * 0.5 - BasePlane.YAxis * Width * 0.5;
          Point3d d = BasePlane.Origin + BasePlane.XAxis * Length * 0.5 - BasePlane.YAxis * Width * 0.5;
          // PEAK!
          Point3d m = BasePlane.Origin + BasePlane.ZAxis * Height;

          var displayLines = new List<LineCurve>();
          // DRAW THE BASE!
          displayLines.Add(new LineCurve(a,b));
          displayLines.Add(new LineCurve(b,c));
          displayLines.Add(new LineCurve(c,d));
          displayLines.Add(new LineCurve(d,a));
          // DRAW THE CORNERS TO PEAK!
          displayLines.Add(new LineCurve(a,m));
          displayLines.Add(new LineCurve(b,m));
          displayLines.Add(new LineCurve(c,m));
          displayLines.Add(new LineCurve(d,m));

          return displayLines;
        }

        public List<Surface> ComputeSurfaces()
        {
          // BASE POINTS!
          Point3d a = BasePlane.Origin + BasePlane.XAxis * Length * 0.5 + BasePlane.YAxis * Width * 0.5;
          Point3d b = BasePlane.Origin - BasePlane.XAxis * Length * 0.5 + BasePlane.YAxis * Width * 0.5;
          Point3d c = BasePlane.Origin - BasePlane.XAxis * Length * 0.5 - BasePlane.YAxis * Width * 0.5;
          Point3d d = BasePlane.Origin + BasePlane.XAxis * Length * 0.5 - BasePlane.YAxis * Width * 0.5;
          // PEAK!
          Point3d m = BasePlane.Origin + BasePlane.ZAxis * Height;

          var displaySurf = new List<Surface>();

          displaySurf.AddRange(new List<Surface>
          {
            NurbsSurface.CreateFromCorners(a, b, c, d),
            NurbsSurface.CreateFromCorners(a, b, m),
            NurbsSurface.CreateFromCorners(b, c, m),
            NurbsSurface.CreateFromCorners(c, d, m),
            NurbsSurface.CreateFromCorners(d, a, m)
          });

          return displaySurf;
        }
        
        public List<Point3d> OutPoints()
                {
                  // BASE POINTS!
                  Point3d a = BasePlane.Origin + BasePlane.XAxis * Length * 0.5 + BasePlane.YAxis * Width * 0.5;
                  Point3d b = BasePlane.Origin - BasePlane.XAxis * Length * 0.5 + BasePlane.YAxis * Width * 0.5;
                  Point3d c = BasePlane.Origin - BasePlane.XAxis * Length * 0.5 - BasePlane.YAxis * Width * 0.5;
                  Point3d d = BasePlane.Origin + BasePlane.XAxis * Length * 0.5 - BasePlane.YAxis * Width * 0.5;
                  // PEAK!
                  Point3d m = BasePlane.Origin + BasePlane.ZAxis * Height;
        
                  var outPts = new List<Point3d>();
                  outPts.AddRange(new List<Point3d>
                  {
                    new Point3d(a),
                    new Point3d(b),
                    new Point3d(c),
                    new Point3d(d),
                    new Point3d(m)
                  });

                  return outPts;
                }
      }
}
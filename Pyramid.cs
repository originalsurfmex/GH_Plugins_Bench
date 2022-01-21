using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace WorkBench
{
  public partial class Pyramid : GH_Component
  {
    /// <summary>
    /// Each implementation of GH_Component must provide a public 
    /// constructor without any arguments.
    /// Category represents the Tab in which the component will appear, 
    /// Subcategory the panel. If you use non-existing tab or panel names, 
    /// new tabs/panels will automatically be created.
    /// </summary>
    public Pyramid()
      : base("Pyramid", "Computes a Pyramid",
        "WorkBench Test Component - Pyramid",
        "WorkBench", "Utilities")
    {
    }

    /// <summary>
    /// Registers all the input parameters for this component.
    /// </summary>
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddPlaneParameter("Base Plane", "Base Plane", "Base Plane", GH_ParamAccess.item);
      pManager.AddNumberParameter("Length", "Length", "Length", GH_ParamAccess.item);
      pManager.AddNumberParameter("Width", "Width", "Width", GH_ParamAccess.item);
      pManager.AddNumberParameter("Height", "Height", "Height", GH_ParamAccess.item);
      //pManager.AddPointParameter("Points", "Points", "Points", GH_ParamAccess.list);
    }

    /// <summary>
    /// Registers all the output parameters for this component.
    /// </summary>
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddCurveParameter("Display Lines", "Display Lines", "Display Lines", GH_ParamAccess.item);
      pManager.AddNumberParameter("Volume", "Volume", "Volume", GH_ParamAccess.item);
      pManager.AddSurfaceParameter("Surfaces", "Surfaces", "Surfaces", GH_ParamAccess.list);
      pManager.AddPointParameter("Points", "Points", "Points", GH_ParamAccess.list);
    }

    /// <summary>
    /// This is the method that actually does the work.
    /// </summary>
    /// <param name="da">The DA object can be used to retrieve data from input parameters and 
    /// to store data in output parameters.</param>
    protected override void SolveInstance(IGH_DataAccess da)
    {
      Plane iBasePlane = Plane.WorldXY;
      double iLen = 1.0;
      double iWid = 1.0;
      double iHt = 1.0;
      da.GetData("Base Plane", ref iBasePlane);
      da.GetData("Length", ref iLen);
      da.GetData("Width", ref iWid);
      da.GetData("Height", ref iHt);

      var dasPyr = new PyramidBlwh(iBasePlane, iWid, iLen, iHt);
      // curves
      List <LineCurve> displayLines = dasPyr.ComputeDisplayLines();
      da.SetDataList("Display Lines", displayLines);
      // volume
      double volume = dasPyr.ComputeVolume();
      da.SetData("Volume", volume);
      // surfaces
      List<Surface> srf = dasPyr.ComputeSurfaces();
      da.SetDataList("Surfaces", srf);

      List<Point3d> pts = dasPyr.OutPoints();
      da.SetDataList("Points", pts);
    }

    /// <summary>
    /// Provides an Icon for every component that will be visible in the User Interface.
    /// Icons need to be 24x24 pixels.
    /// You can add image files to your project resources and access them like this:
    /// return Resources.IconForThisComponent;
    /// </summary>
    //protected override System.Drawing.Bitmap Icon => null;
    protected override Bitmap Icon
    {
      get
      {
        var assembly = System.Reflection.Assembly.GetExecutingAssembly();
        {
          var resourceName = assembly.GetManifestResourceNames().Single(n => n.EndsWith("PyramidIcon.png"));
          var stream = assembly.GetManifestResourceStream(resourceName);
          if (stream != null) return new Bitmap(stream);
        }
        return null;
      }
    }

    /// <summary>
    /// Each component must have a unique Guid to identify it. 
    /// It is vital this Guid doesn't change otherwise old ghx files 
    /// that use the old ID will partially fail during loading.
    /// </summary>
    public override Guid ComponentGuid => new Guid("73060FCF-221C-4E0A-A7D2-BEF135672CDA");
  }


}
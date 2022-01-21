using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace WorkBench
{
  public class Centroid : GH_Component
  {
    /// <summary>
    /// Each implementation of GH_Component must provide a public 
    /// constructor without any arguments.
    /// Category represents the Tab in which the component will appear, 
    /// Subcategory the panel. If you use non-existing tab or panel names, 
    /// new tabs/panels will automatically be created.
    /// </summary>
    public Centroid()
      : base("Centroid", "Computes Centroid",
        "WorkBench Test Component - Centroid",
        "WorkBench", "Utilities")
    {
    }

    /// <summary>
    /// Registers all the input parameters for this component.
    /// </summary>
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddPointParameter("Points", "Points", "Points", GH_ParamAccess.list);
    }

    /// <summary>
    /// Registers all the output parameters for this component.
    /// </summary>
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddPointParameter("Centroid", "Centroid", "Centroid", GH_ParamAccess.item);
      pManager.AddNumberParameter("Distances", "Distances", "Distances", GH_ParamAccess.list);
    }

    /// <summary>
    /// This is the method that actually does the work.
    /// </summary>
    /// <param name="da">The DA object can be used to retrieve data from input parameters and 
    /// to store data in output parameters.</param>
    protected override void SolveInstance(IGH_DataAccess da)
    {
      var iPoints = new List<Point3d>();
      da.GetDataList("Points", iPoints);

      // mass add up all the centroid points into one "mega"-point
      Point3d centroid = new Point3d(0.0, 0.0, 0.0);
      foreach (var pt in iPoints)
        centroid += pt;
      centroid /= iPoints.Count; // i guess calc'ing a centroid is remarkably easy.
      // CENTROIDS //
      da.SetData("Centroid", centroid);

      var distances = new List<double>();
      foreach (var pt in iPoints)
        distances.Add(centroid.DistanceTo(pt));
      // DISTANCES //
      da.SetDataList("Distances", distances);
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
          var resourceName = assembly.GetManifestResourceNames().Single(n => n.EndsWith("CentroidIcon.png"));
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
    public override Guid ComponentGuid => new Guid("8DB64619-6321-47A4-9226-504A5ABD0959");
      
  }
}
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
  public class MovingParticle : GH_Component
  {
    /// <summary>
    /// Each implementation of GH_Component must provide a public 
    /// constructor without any arguments.
    /// Category represents the Tab in which the component will appear, 
    /// Subcategory the panel. If you use non-existing tab or panel names, 
    /// new tabs/panels will automatically be created.
    /// </summary>
    public MovingParticle()
      : base("Moving Particle", "Computes a Moving Particle",
        "Workshop Test Component - Moving Particle.  The point data is persistent and this component requires a timed trigger.",
        "Workshop", "Utilities")
    {
    }

    /// <summary>
    /// Registers all the input parameters for this component.
    /// </summary>
    protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
    {
      pManager.AddBooleanParameter("Reset", "Reset", "Reset", GH_ParamAccess.item);
      pManager.AddVectorParameter("Velocity", "Velocity", "Velocity", GH_ParamAccess.item);
    }

    /// <summary>
    /// Registers all the output parameters for this component.
    /// </summary>
    protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
    {
      pManager.AddPointParameter("Particle", "Particle", "Particle", GH_ParamAccess.item);
    }

    //persistent data
    private Point3d currentPosition;

    /// <summary>
    /// This is the method that actually does the work.
    /// </summary>
    /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
    /// to store data in output parameters.</param>
    protected override void SolveInstance(IGH_DataAccess DA)
    {
      bool iReset = false;
      DA.GetData("Reset", ref iReset);

      if (iReset)
        currentPosition = new Point3d(0.0, 0.0, 0.0);
      else
      {
        Vector3d iVelocity = new Vector3d(0.0, 0.0, 0.0);
        DA.GetData("Velocity", ref iVelocity);
        currentPosition += iVelocity;
      }

      DA.SetData("Particle", currentPosition);
    }

    /// <summary>
    /// Provides an Icon for every component that will be visible in the User Interface.
    /// Icons need to be 24x24 pixels.
    /// You can add image files to your project resources and access them like this:
    /// return Resources.IconForThisComponent;
    /// </summary>
    //protected override System.Drawing.Bitmap Icon => null;
    protected override System.Drawing.Bitmap Icon
    {
      get
      {
        var assembly = System.Reflection.Assembly.GetExecutingAssembly();
        {
          var resourceName = assembly.GetManifestResourceNames().Single(n => n.EndsWith("MovingParticleIcon.png"));
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
    public override Guid ComponentGuid => new Guid("5988494E-5F34-4DE7-B3AD-B476482F33E9");
      
  }
}
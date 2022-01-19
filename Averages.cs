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
  public class Averages : GH_Component
  {
    /// <summary>
    /// Each implementation of GH_Component must provide a public 
    /// constructor without any arguments.
    /// Category represents the Tab in which the component will appear, 
    /// Subcategory the panel. If you use non-existing tab or panel names, 
    /// new tabs/panels will automatically be created.
    /// </summary>
    public Averages()
      : base("Averages", "Computes Averages",
        "Workshop Test Component - Averages",
        "Workshop", "Utilities")
    {
    }

    /// <summary>
    /// Registers all the input parameters for this component.
    /// </summary>
    protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
    {
      pManager.AddNumberParameter("first num", "first num", "the first num", GH_ParamAccess.item, 0.0);
      pManager.AddNumberParameter("second num", "second num", "the second num", GH_ParamAccess.item, 0.0);
    }

    /// <summary>
    /// Registers all the output parameters for this component.
    /// </summary>
    protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
    {
      pManager.AddNumberParameter("Average", "Average", "average of inputs", GH_ParamAccess.item);
    }

    /// <summary>
    /// This is the method that actually does the work.
    /// </summary>
    /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
    /// to store data in output parameters.</param>
    protected override void SolveInstance(IGH_DataAccess DA)
    {
      double a = double.NaN;
      double b = double.NaN;
      
      bool checkA = DA.GetData(0, ref a);
      bool checkB = DA.GetData(1, ref b);

      if (checkA || checkB)
      {
        double average = 0.5 * (a + b);
        DA.SetData(0, average);       
      }
      else
      {
        AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "c'mon");
        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "nope");
        AddRuntimeMessage(GH_RuntimeMessageLevel.Blank, "tic tic tic");
      }

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
          var resourceName = assembly.GetManifestResourceNames().Single(n => n.EndsWith("AverageIcon.png"));
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
    public override Guid ComponentGuid => new Guid("6CBE441F-4367-4146-9755-BF7296D4BFE1");
  }
}
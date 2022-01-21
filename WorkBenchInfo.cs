using System;
using System.Drawing;
using System.Linq;
using Grasshopper;
using Grasshopper.Kernel;

namespace WorkBench
{
  public class WorkBenchInfo : GH_AssemblyInfo
  {
    public override string Name => "WorkBench";

    //Return a 24x24 pixel bitmap to represent this GHA library.
    //public override Bitmap Icon => null;
    ///*
    public override Bitmap Icon
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
    //*/

    //Return a short string describing the purpose of this GHA library.
    public override string Description => "WorkBench";

    public override Guid Id => new Guid("F692A962-AEE5-4D01-853C-4CC14EB4E520");

    //Return a string identifying you or your company.
    public override string AuthorName => "rd3";

    //Return a string representing your preferred contact details.
    public override string AuthorContact => "da moon";
 }

  public class CategoryIcon : GH_AssemblyPriority
  {
    public override GH_LoadingInstruction PriorityLoad()
    {
      var assembly = System.Reflection.Assembly.GetExecutingAssembly();
      var resourceName = assembly.GetManifestResourceNames().Single(n => n.EndsWith("AverageIcon.png"));
      var stream = assembly.GetManifestResourceStream(resourceName);
      if (stream != null)
      {
          Bitmap dasIcon = new Bitmap(stream);
          Instances.ComponentServer.AddCategoryIcon("WorkBench", dasIcon);
      }

      Instances.ComponentServer.AddCategorySymbolName("WorkBench", 'w');
      return GH_LoadingInstruction.Proceed;
    }
  }
}
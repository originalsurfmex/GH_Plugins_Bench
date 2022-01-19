using System;
using System.Drawing;
using System.Linq;
using Grasshopper;
using Grasshopper.Kernel;

namespace Plugin_1_Day_3
{
  public class Plugin_1_Day_3Info : GH_AssemblyInfo
  {
    public override string Name => "Plugin_1_Day_3 Info";

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
    public override string Description => "Workshop";

    public override Guid Id => new Guid("F692A962-AEE5-4D01-853C-4CC14EB4E520");

    //Return a string identifying you or your company.
    public override string AuthorName => "rd3";

    //Return a string representing your preferred contact details.
    public override string AuthorContact => "da moon";
  }

  public class CategoryIcon : Grasshopper.Kernel.GH_AssemblyPriority
  {
    public override Grasshopper.Kernel.GH_LoadingInstruction PriorityLoad()
    {
      var assembly = System.Reflection.Assembly.GetExecutingAssembly();
      var resourceName = assembly.GetManifestResourceNames().Single(n => n.EndsWith("AverageIcon.png"));
      var stream = assembly.GetManifestResourceStream(resourceName);
      Bitmap dasIcon = new Bitmap(stream);
      Grasshopper.Instances.ComponentServer.AddCategoryIcon("Workshop", dasIcon);
      Grasshopper.Instances.ComponentServer.AddCategorySymbolName("Workshop", 'w');
      return Grasshopper.Kernel.GH_LoadingInstruction.Proceed;
    }
  }
}
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Grasshopper.Kernel;
using Rhino.Geometry;


namespace WorkBench
{
    public class MeshGrowth : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MeshGrowth class.
        /// </summary>

        // persistent variable within the class
        private MeshGrowthSystem _myMeshGrowthSystem;

        public MeshGrowth()
          : base("MeshGrowth", "MeshGrowth",
              "Expand a mesh based on subdivision and avoiding self-collision",
              "WorkBench", "MeshGrowth")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Reset", "Reset", "Reset", GH_ParamAccess.item);
            pManager.AddMeshParameter("Starting Mesh", "Starting Mesh", "Starting Mesh", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Sub-iteration Count", "Sub-iteration Count", "Sub-iteration Count",
                GH_ParamAccess.item);
            pManager.AddBooleanParameter("Grow", "Grow", "Grow", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Max Vertex Count", "Max Vertex Count", "Max Vertex Count",
                GH_ParamAccess.item);
            pManager.AddNumberParameter("Edge Length Constraint Weight", "Edge Length Constraint Weight",
                "Edge Length Constraint Weight", GH_ParamAccess.item);
            pManager.AddNumberParameter("Collision Distance", "Collision Distance", "Collision Distance",
                GH_ParamAccess.item);
            pManager.AddNumberParameter("Collision Weight", "Collision Weight", "Collision Weight",
                GH_ParamAccess.item);
            pManager.AddNumberParameter("Bending Resistance Weight", "Bending Resistance Weight",
                "Bending Resistance Weight", GH_ParamAccess.item);
            pManager.AddBooleanParameter("R-Tree", "R-Tree", "R-Tree", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "Mesh", "Mesh", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="da">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess da)
        {
            bool iReset = true;
            da.GetData("Reset", ref iReset);
            Mesh iStartingMesh = null;
            da.GetData("Starting Mesh", ref iStartingMesh);
            int iSubIterationCount = 0;
            da.GetData("Sub-iteration Count", ref iSubIterationCount);
            bool iGrow = false;
            da.GetData("Grow", ref iGrow);
            int iMaxVertexCount = 0;
            da.GetData("Max Vertex Count", ref iMaxVertexCount);
            double iEdgeLengthConstrainWeight = 0.0;
            da.GetData("Edge Length Constraint Weight", ref iEdgeLengthConstrainWeight);
            double iCollisionDistance = 0.0;
            da.GetData("Collision Distance", ref iCollisionDistance);
            double iCollisionWeight = 0.0;
            da.GetData("Collision Weight", ref iCollisionWeight);
            double iBendingResistanceWeight = 0.0;
            da.GetData("Bending Resistance Weight", ref iBendingResistanceWeight);
            bool iRTree = false;
            da.GetData("R-Tree", ref iRTree);


            //=====================================================================================================

            // if this gets reset then restart with a fresh mesh
            // the null part is there because otherwise it requires a reset on the first go
            if (iReset || _myMeshGrowthSystem == null)
                _myMeshGrowthSystem = new MeshGrowthSystem(iStartingMesh);

            _myMeshGrowthSystem.Grow = iGrow; // connect the boolean to the system - grow/no grow
            _myMeshGrowthSystem.MaxVertexCount = iMaxVertexCount;
            _myMeshGrowthSystem.EdgeLengthConstrainWeight = iEdgeLengthConstrainWeight;
            _myMeshGrowthSystem.CollisionWeight = iCollisionWeight;
            _myMeshGrowthSystem.BendingResistanceWeight = iBendingResistanceWeight;
            _myMeshGrowthSystem.UseRTree = iRTree;
            _myMeshGrowthSystem.CollisionDistance = iCollisionDistance;

            // this does the work every time the component runs this runs once
            // this also allows a slider to control the iterations instead of a timer/trigger
            // if a timer/trigger is used then the slider should be set to however many iterations per trigger (typ 1)
            for (int i = 0; i < iSubIterationCount; i++)
                _myMeshGrowthSystem.Update();

            // output a rhino mesh after all the plankton work is done
            da.SetData("Mesh", _myMeshGrowthSystem.GetRhinoMesh());
        }

    /// <summary>
    /// Provides an Icon for the component.
    /// </summary>
    protected override Bitmap Icon
    {
      get
      {
        var assembly = System.Reflection.Assembly.GetExecutingAssembly();
        {
          var resourceName = assembly.GetManifestResourceNames().Single(n => n.EndsWith("MeshGrowth.png"));
          var stream = assembly.GetManifestResourceStream(resourceName);
          if (stream != null) return new Bitmap(stream);
        }
        return null;
      }
    }
        

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("D86CE874-B820-454A-AAAD-1290E96C87CF"); }
        }
    }
}
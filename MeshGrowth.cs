using System;
using Grasshopper.Kernel;
using PlanktonGh;
using Rhino.Geometry;

namespace WorkBench
{
    public class MeshGrowth : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MeshGrowth class.
        /// </summary>

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
            Mesh iStartingMesh = null;
            int iSubIterationCount = 0;
            bool iGrow = false;
            int iMaxVertexCount = 0;
            double iEdgeLengthConstrainWeight = 0.0;
            double iCollisionDistance = 0.0;
            double iCollisionWeight = 0.0;
            double iBendingResistanceWeight = 0.0;
            bool iRTree = false;

            //=====================================================================================================

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (iReset || _myMeshGrowthSystem == null)
                // ReSharper disable once ExpressionIsAlwaysNull
                _myMeshGrowthSystem = new MeshGrowthSystem(iStartingMesh.ToPlanktonMesh());

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            _myMeshGrowthSystem.Grow = iGrow;
            _myMeshGrowthSystem.MaxVertexCount = iMaxVertexCount;
            _myMeshGrowthSystem.EdgeLengthConstrainWeight = iEdgeLengthConstrainWeight;
            _myMeshGrowthSystem.CollisionDistance = iCollisionDistance;
            _myMeshGrowthSystem.CollisionWeight = iCollisionWeight;
            _myMeshGrowthSystem.BendingResistanceWeight = iBendingResistanceWeight;
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            _myMeshGrowthSystem.UseRTree = iRTree;

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            for (int i = 0; i < iSubIterationCount; i++)
                _myMeshGrowthSystem.Update();

            da.SetData("Mesh", _myMeshGrowthSystem.Mesh.ToRhinoMesh());
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
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
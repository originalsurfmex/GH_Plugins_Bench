using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using Eto.Forms;
using Plankton;
using PlanktonGh;
using Rhino.DocObjects;
using Rhino.Geometry;

namespace WorkBench
{
    public class MeshGrowthSystem
    {
        // starting plankton mesh to work with
        private PlanktonMesh _ptMesh; //private so nobody can mess with it

        public bool Grow = false;
        public int MaxVertexCount;

        // speed faster, slow with this as false
        public bool UseRTree;

        public double EdgeLengthConstrainWeight; // strength in comparison to relaxation or bending strength
        public double CollisionDistance; // basically the size of the sphere, should be 2 * radius of sphere
        public double CollisionWeight; // how strongly the sphere can "push"
        public double BendingResistanceWeight;

        private List<Vector3d> totalWeightedMoves;
        private List<double> totalWeights; // each vertex has a total weight

        public MeshGrowthSystem(Mesh startingMesh) //takes in a Rhino Mesh and converts to a plankton mesh
        {
            _ptMesh = startingMesh.ToPlanktonMesh();
        }

        public Mesh GetRhinoMesh()
        {
            return _ptMesh.ToRhinoMesh();
        }

        public void Update()
        {
            if (Grow) SplitAllLongEdges();

            totalWeightedMoves = new List<Vector3d>();
            totalWeights = new List<double>();

            foreach (var unused in _ptMesh.Vertices)
            {
                totalWeightedMoves.Add(new Vector3d(0, 0, 0));
                totalWeights.Add(0.0);
            }

            //ProcessCollision();
            if (UseRTree) ProcessCollisionWithRTree();
            else ProcessCollision();

            ProcessBendingResistance();
            ProcessEdgeLengthConstraint();
            UpdateVertexPositions();
        }

        private void SplitAllLongEdges()
        {
            int halfEdgeCount = _ptMesh.Halfedges.Count;

            // every 2 edges because how the half edge data structure works (still unclear on this)
            for (int i = 0; i < halfEdgeCount; i += 2)
            {
                if (_ptMesh.Vertices.Count < MaxVertexCount &&
                    _ptMesh.Halfedges.GetLength(i) > 0.99 * CollisionDistance)
                {
                    SplitEdge(i);
                }
            }
        }

        /// <summary>
        /// Move the vertices each time the process updates, using the idea of collision spheres
        /// </summary>
        private void ProcessCollision()
        {
            int vertexCount = _ptMesh.Vertices.Count;
            for (int i = 0; i < vertexCount; i++)
                for (int j = i + 1; j < vertexCount; j++)
                {
                    Vector3d move = _ptMesh.Vertices[j].ToPoint3d() - _ptMesh.Vertices[i].ToPoint3d();
                    double currentDistance = move.Length;

                    if (currentDistance > CollisionDistance) continue;

                    // if the current distance is less than the collision distance it keeps nudging
                    // 0.5 makes it nudge out into a sphere, other amounts change the shape/proportion
                    move *= 0.25 * (currentDistance - CollisionDistance) / currentDistance;

                    /*
                    move the vertices away from each other along the vector
                    the bending resistance and the collision effects can be manually adjusted
                    a 'collision weight' slider allows the collisions power/importance to be controlled
                    this is why we multiply by a 'collision weight' 
                    the new INCREMENT VALUE used for averages is now the 'collision weight'
                    this is more elegant than just taking all the weights and averaging them
                    */
                    totalWeightedMoves[i] += CollisionWeight * move;
                    totalWeightedMoves[j] -= CollisionWeight * move;
                    // if we didn't use a collision weighting system then this would just be 1.0
                    totalWeights[i] += CollisionWeight;
                    totalWeights[j] += CollisionWeight;
                }
        }

        private void ProcessCollisionWithRTree()
        {
            // build the R-Tree and populate with points
            RTree rTree = new RTree();
            for (int i = 0; i < _ptMesh.Vertices.Count; i++)
                rTree.Insert(_ptMesh.Vertices[i].ToPoint3d(), i);

            // get the search sphere going
            for (int i = 0; i < _ptMesh.Vertices.Count; i++)
            {
                var vI = _ptMesh.Vertices[i].ToPoint3d();
                var searchSphere = new Sphere(vI, CollisionDistance);

                var collisionIndices = new List<int>();

                // wild little callback function whaaat????
                // goal here is to get a list of collision indices from the rtree, and only calculate those
                // rtree is logarithmic improvement, so more vertices means bigger improvements
                rTree.Search(searchSphere,
                    (sender, args) => { if (i < args.Id) collisionIndices.Add(args.Id); });

                // run the collision program, it no longer needs to check for distance here cuz of sphere + rtree
                foreach (var j in collisionIndices)
                {
                    Vector3d move = _ptMesh.Vertices[j].ToPoint3d() - _ptMesh.Vertices[i].ToPoint3d();
                    double currentDistance = move.Length;

                    // if the current distance is less than the collision distance it keeps nudging
                    // 0.5 makes it nudge out into a sphere, other amounts change the shape/proportion
                    move *= 0.5 * (currentDistance - CollisionDistance) / currentDistance;

                    totalWeightedMoves[i] += CollisionWeight * move;
                    totalWeightedMoves[j] -= CollisionWeight * move;
                    // if we didn't use a collision weighting system then this would just be 1.0
                    totalWeights[i] += CollisionWeight;
                    totalWeights[j] += CollisionWeight;

                }
            }

        }

        // this gets into winding around the half edge mesh structure too, it could use a diagram or something
        // the diagram for this is on the PPT for Long Nguyen's workshop.
        private void ProcessBendingResistance()
        {
            int halfEdgeCount = _ptMesh.Halfedges.Count;
            for (int i = 0; i < halfEdgeCount; i += 2)
            {
                // index the points using the half edge data structure
                int j = _ptMesh.Halfedges[i].StartVertex;
                int k = _ptMesh.Halfedges[i + 1].StartVertex;
                int p = _ptMesh.Halfedges[_ptMesh.Halfedges[i].PrevHalfedge].StartVertex;
                int q = _ptMesh.Halfedges[_ptMesh.Halfedges[i + 1].PrevHalfedge].StartVertex;

                // get the point from the point index, see half edge triangle diagram
                Point3d vJ = _ptMesh.Vertices[j].ToPoint3d();
                Point3d vK = _ptMesh.Vertices[k].ToPoint3d();
                Point3d vP = _ptMesh.Vertices[p].ToPoint3d();
                Point3d vQ = _ptMesh.Vertices[q].ToPoint3d();

                // compute the plane origin and the normal vector, using half edge structure logic (see diagrams)
                Vector3d nP = Vector3d.CrossProduct(vK - vJ, vP - vJ); // the triangle here is K-J-P
                Vector3d nQ = Vector3d.CrossProduct(vQ - vJ, vK - vJ); // the triangle here is Q-J-K
                //Vector3d nQ = Vector3d.CrossProduct(vK - vJ, vQ - vJ); // the triangle here is K-J-Q, infinite weird cool

                // get the normal vector and origin point, the rhino plane constructor will AUTOMATICALLY UNITIZE THIS
                Vector3d planeNormal = nP + nQ;
                Point3d planeOrigin = 0.25 * (vJ + vK + vP + vQ); // just hard coded the equivalent of divide by 4
                Plane plane = new Plane(planeOrigin, planeNormal);

                // get the vector between the current point and the closest point, add it into the summed list per point
                // similar to the process collisions, a slider gives control so that bending resistance can be
                // manually weighted and adjusted.  see explanation in comments of collision process function
                totalWeightedMoves[j] += BendingResistanceWeight * (plane.ClosestPoint(vJ) - vJ);
                totalWeightedMoves[k] += BendingResistanceWeight * (plane.ClosestPoint(vK) - vK);
                totalWeightedMoves[p] += BendingResistanceWeight * (plane.ClosestPoint(vP) - vP);
                totalWeightedMoves[q] += BendingResistanceWeight * (plane.ClosestPoint(vQ) - vQ);

                // increment the total quantity of vectors so that it may be averaged later
                // if we didn't use a bending resistance weighting system then this would just be 1.0
                totalWeights[j] += BendingResistanceWeight; 
                totalWeights[k] += BendingResistanceWeight; 
                totalWeights[p] += BendingResistanceWeight; 
                totalWeights[q] += BendingResistanceWeight; 
            }
        }

        // this prevents the edges from getting to long and creating disproportionate triangles
        private void ProcessEdgeLengthConstraint()
        {
            for (int i = 0; i < _ptMesh.Halfedges.Count; i += 2) // increment by 2 due to half edge structure
            {
                // assign the vertices to be processed the correct index number
                int j = _ptMesh.Halfedges[i].StartVertex;
                int k = _ptMesh.Halfedges[i + 1].StartVertex; // sibling vertex

                // use the index numbers to get the correct vertex points
                Point3d vJ = _ptMesh.Vertices[j].ToPoint3d();
                Point3d vK = _ptMesh.Vertices[k].ToPoint3d();

                // if the points are already less than the collision distance then don't move it
                if (vJ.DistanceTo(vK) < CollisionDistance) continue;

                // get the vector between each subsequent set of points
                // and move half way between them
                //Vector3d move = vJ - vK; // this does a weird flatter version of the algorithm
                Vector3d move = vK - vJ;
                move *= (move.Length - CollisionDistance) * 0.5 / move.Length;

                // see comments on bending resistance and process collision to explain the weighting concept
                // move the vectors in opposite directions
                totalWeightedMoves[j] += move * EdgeLengthConstrainWeight;
                totalWeightedMoves[k] -= move * EdgeLengthConstrainWeight;

                // used to get the averages without the weighting system this would just be 1.0;
                totalWeights[j] += EdgeLengthConstrainWeight;
                totalWeights[k] += EdgeLengthConstrainWeight;
            }
        }

        private void UpdateVertexPositions()
        {
            for (int i = 0; i < _ptMesh.Vertices.Count; i++)
            {
                if (totalWeights[i] == 0.0) continue;

                // gather all the weighted moves and average them into one vector
                Vector3d move = totalWeightedMoves[i] / totalWeights[i];
                Point3d newPosition = _ptMesh.Vertices[i].ToPoint3d() + move;
                _ptMesh.Vertices.SetVertex(i, newPosition.X, newPosition.Y, newPosition.Z);
            }
        }


        /*
        private List<Vector3d> totalWeightedMoves;
        private List<double> totalWeights;

        public MeshGrowthSystem(PlanktonMesh startingPtMesh)
        {
            ptMesh = startingPtMesh;
        }

        public void Update()
        {
            if (Grow) SplitAllLongEdges();

            totalWeightedMoves = new List<Vector3d>();
            totalWeights.Add(0.0);

            for (int i = 0; i < ptMesh.Vertices.Count; i++)
            {
                totalWeightedMoves.Add(Vector3d.Zero);
                totalWeights.Add(0.0);
            }

            ProcessEdgeLengthConstraint();
            ProcessCollision();
            UpdateVertexPositionsAndVelocities();
        }
        */

        private void SplitEdge(int edgeIndex)
        {
            int newHalfEdgeIndex = _ptMesh.Halfedges.SplitEdge(edgeIndex);

            _ptMesh.Vertices.SetVertex(
                _ptMesh.Vertices.Count - 1,
                0.5 * (_ptMesh.Vertices[_ptMesh.Halfedges[edgeIndex].StartVertex].ToPoint3d() +
                       _ptMesh.Vertices[_ptMesh.Halfedges[edgeIndex + 1].StartVertex].ToPoint3d()));

            if (_ptMesh.Halfedges[edgeIndex].AdjacentFace >= 0)
                _ptMesh.Faces.SplitFace(newHalfEdgeIndex, _ptMesh.Halfedges[edgeIndex].PrevHalfedge);

            if (_ptMesh.Halfedges[edgeIndex + 1].AdjacentFace >= 0)
                _ptMesh.Faces.SplitFace(edgeIndex + 1,
                    _ptMesh.Halfedges[_ptMesh.Halfedges[edgeIndex + 1].NextHalfedge].NextHalfedge);
        }

        /*
        private void SplitAllLongEdges()
        {
            int halfedgeCount = ptMesh.Halfedges.Count;

            for (int k = 0; k < halfedgeCount; k += 2)
                if (ptMesh.Vertices.Count < MaxVertexCount &&
                    ptMesh.Halfedges.GetLength(k) > 0.99 * CollisionDistance)
                {
                    SplitEdge(k);
                }
        }

        private void ProcessEdgeLengthConstraint()
        {
            int halfEdgeCount = ptMesh.Halfedges.Count;

            for (int k = 0; k < halfEdgeCount; k += 2)
            {
                PlanktonHalfedge halfedge = ptMesh.Halfedges[k];
                int i = halfedge.StartVertex;
                int j = ptMesh.Halfedges[halfedge.NextHalfedge].StartVertex;

                Vector3d d = ptMesh.Vertices[j].ToPoint3d() - ptMesh.Vertices[i].ToPoint3d();
                if (d.Length > CollisionDistance)
                {
                    Vector3d move = EdgeLengthConstrainWeight * 0.5 * (d);
                    totalWeightedMoves[i] += move;
                    totalWeightedMoves[j] -= move;
                    totalWeights[i] += EdgeLengthConstrainWeight;
                    totalWeights[j] += EdgeLengthConstrainWeight;

                }
            }
        }

        private void ProcessCollision()
        {
            for (int i = 0; i < ptMesh.Vertices.Count; i++)
            for (int j = i + 1; j < ptMesh.Vertices.Count; j++)
            {
                Vector3d move = ptMesh.Vertices[j].ToPoint3d() - ptMesh.Vertices[i].ToPoint3d();
                double currentDistance = move.Length;
                if (currentDistance > CollisionDistance) continue;

                move *= CollisionWeight * 0.5 * (currentDistance - CollisionDistance) / currentDistance;
                totalWeightedMoves[i] += move;
                totalWeightedMoves[j] -= move;
                totalWeights[i] += CollisionWeight;
                totalWeights[j] += CollisionWeight;
            }
        }

        private void UpdateVertexPositionsAndVelocities()
        {
            for (int i = 0; i < ptMesh.Vertices.Count; i++)
            {
                if (totalWeights[i] == 0) continue;

                PlanktonVertex vertex = ptMesh.Vertices[i];
                Vector3d move = totalWeightedMoves[i] / totalWeights[i];
                ptMesh.Vertices.SetVertex(i, vertex.X + move.X, vertex.Y + move.Y, vertex.Z + move.Z);
            }
        }
        */
    }
}

using System;
using System.Collections;
using System.Collections.Generic;

using Rhino;
using Rhino.Geometry;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

// Non-default includes.
using Rhino.DocObjects;
using Rhino.Collections;
using GH_IO;
using GH_IO.Serialization;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;

/// Unique namespace, so visual studio won't throw any errors about duplicate definitions.
namespace ns11fd2
{
    /// <summary>
    /// This class will be instantiated on demand by the Script component.
    /// </summary>
    public class Script_Instance : GH_ScriptInstance
    {
        /// This method is added to prevent compiler errors when opening this file in visual studio (code) or rider.
        public override void InvokeRunScript(IGH_Component owner, object rhinoDocument, int iteration, List<object> inputs, IGH_DataAccess DA)
        {
            throw new NotImplementedException();
        }

        #region Utility functions
        /// <summary>Print a String to the [Out] Parameter of the Script component.</summary>
        /// <param name="text">String to print.</param>
        private void Print(string text) { /* Implementation hidden. */ }
        /// <summary>Print a formatted String to the [Out] Parameter of the Script component.</summary>
        /// <param name="format">String format.</param>
        /// <param name="args">Formatting parameters.</param>
        private void Print(string format, params object[] args) { /* Implementation hidden. */ }
        /// <summary>Print useful information about an object instance to the [Out] Parameter of the Script component. </summary>
        /// <param name="obj">Object instance to parse.</param>
        private void Reflect(object obj) { /* Implementation hidden. */ }
        /// <summary>Print the signatures of all the overloads of a specific method to the [Out] Parameter of the Script component. </summary>
        /// <param name="obj">Object instance to parse.</param>
        private void Reflect(object obj, string method_name) { /* Implementation hidden. */ }
        #endregion
        #region Members
        /// <summary>Gets the current Rhino document.</summary>
        private readonly RhinoDoc RhinoDocument;
        /// <summary>Gets the Grasshopper document that owns this script.</summary>
        private readonly GH_Document GrasshopperDocument;
        /// <summary>Gets the Grasshopper script component that owns this script.</summary>
        private readonly IGH_Component Component;
        /// <summary>
        /// Gets the current iteration count. The first call to RunScript() is associated with Iteration==0.
        /// Any subsequent call within the same solution will increment the Iteration count.
        /// </summary>
        private readonly int Iteration;
        #endregion
        /// <summary>
        /// This procedure contains the user code. Input parameters are provided as regular arguments,
        /// Output parameters as ref arguments. You don't have to assign output parameters,
        /// they will have a default value.
        /// </summary>
        #region Runscript
        private void RunScript(Plane Plane, double Height, double InnerRadius, double OuterRadius, int Steps, double Turns, ref object Treads, ref object Raisers)
        {
            var treads = new List<NurbsSurface>();
            var raisers = new List<NurbsSurface>();

            double stepHeight = Height / (Steps + 1);
            double stepAngle = Turns * Math.PI * 2 / Steps; // Angle in radians

            for (int i = 0; i <= Steps; i++)
            {
                var innerPoint = GetSpiralPoint(Plane, stepHeight * (i + 1), stepAngle * i, InnerRadius);
                var outerPoint = GetSpiralPoint(Plane, stepHeight * (i + 1), stepAngle * i, OuterRadius);

                var innerLowerPoint = GetSpiralPoint(Plane, stepHeight * (i + 1), stepAngle * (i + 1), InnerRadius);
                var outterLowerPoint = GetSpiralPoint(Plane, stepHeight * (i + 1), stepAngle * (i + 1), OuterRadius);

                var innerRaiserPoint = GetSpiralPoint(Plane, stepHeight * i, stepAngle * i, InnerRadius);
                var outterRaiserPoint = GetSpiralPoint(Plane, stepHeight * i, stepAngle * i, OuterRadius);

                var tread = NurbsSurface.CreateFromCorners(innerPoint, innerLowerPoint, outterLowerPoint, outerPoint);
                treads.Add(tread);

                var raiser = NurbsSurface.CreateFromCorners(innerPoint, outerPoint, outterRaiserPoint, innerRaiserPoint);
                raisers.Add(raiser);
            }
            Treads = treads;
            Raisers = raisers;
        }
        #endregion

        #region Additional
        public Point3d GetSpiralPoint(Plane plane, double height, double angle, double radius)
        {
            double x = radius * Math.Cos(angle);
            double y = radius * Math.Sin(angle);
            double z = height;
            return plane.Origin + plane.XAxis * x + plane.YAxis * y + plane.ZAxis * z;
        }
        #endregion
    }
}

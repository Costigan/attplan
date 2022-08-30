using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using attplan.spice;

namespace attplan.Viz
{
    public class MoonWithMetadata : MoonDEM
    {
        public bool IsMetadataVisible = false;

        public Vector3 LadeePositionMe;
        public Vector3 TelescopeBoresightMe;
        public Vector3 TelescopeLimbPointMe;
        public Vector3 TelescopeGrazingPointMe;

        public Vector3 EndOfBoresight;
        public LineSegment TelescopeBoresight;
        public List<LineSegment> DropLines;
        public List<Vector3> GrazingPointsForActivity;

        public override void Draw(bool near, Vector3d eye)
        {
            if (!Visible) return;
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();
            Transform(near, eye); // Pushed one level
            Paint(); // Must be stack even
            GL.PopMatrix(); // Pop the matrix that was pushed in Transform.
        }

        public void Copy(ref Vector3 a, ref double[] b)
        {
            a.X = (float)b[0];
            a.Y = (float)b[1];
            a.Z = (float)b[2];
        }

        public void Copy(ref Vector3d a, ref double[] b)
        {
            a.X = b[0];
            a.Y = b[1];
            a.Z = b[2];
        }


    }
}

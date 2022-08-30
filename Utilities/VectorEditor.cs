using System.Windows.Forms;
using LadeeViz.Viz;

namespace LadeeViz.Utilities
{
    public partial class VectorEditor : Form
    {       
        public VectorEditor()
        {
            InitializeComponent();
        }

        public VectorEditor(OpenGLControlWrapper wrapper, VectorShape vector)
        {
            InitializeComponent();
            var holder = new VectorHolder(wrapper, vector, pgVector);
            pgVector.SelectedObject = holder;
        }
    }

    public class VectorHolder
    {
        public OpenGLControlWrapper Wrapper;
        public VectorShape Vector;
        public PropertyGrid Grid;
        public VectorHolder(OpenGLControlWrapper wrapper, VectorShape vector, PropertyGrid grid)
        {
            Wrapper = wrapper;
            Vector = vector;
            Grid = grid;
        }
        public void Update()
        {
            var len = Vector.Vector.Length;
            if (len < double.Epsilon) return;
            Vector.Vector[0] /= len;
            Vector.Vector[1] /= len;
            Vector.Vector[2] /= len;
            Wrapper.Invalidate();
            Grid.Invalidate();            
        }
        public double X { get { return Vector.Vector[0]; } set { Vector.Vector[0] = value; Update(); } }
        public double Y { get { return Vector.Vector[1]; } set { Vector.Vector[1] = value; Update(); } }
        public double Z { get { return Vector.Vector[2]; } set { Vector.Vector[2] = value; Update(); } }
    }
}

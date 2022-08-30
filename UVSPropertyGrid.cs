using System.Windows.Forms;

namespace LadeeViz
{
    public partial class UVSPropertyGrid : Form
    {
        public UVSPropertyGrid()
        {
            InitializeComponent();
        }

        public PropertyGrid Grid => pgUVS;
    }
}

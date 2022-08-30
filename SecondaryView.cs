using System;
using System.Drawing;
using System.Windows.Forms;
using LadeeViz.Viz;
using OpenTK.Graphics.OpenGL;

namespace LadeeViz
{
    public partial class SecondaryView : Form
    {
        public SecondaryView(World theWorld)
        {
            InitializeComponent();
            Wrapper.TheWorld = theWorld;
        }

        private void openGLControlWrapper1_MouseClick(object sender, MouseEventArgs e)
        {
            Wrapper.Invalidate();
        }

        private void Wrapper_Load(object sender, EventArgs e)
        {
            Wrapper.Loaded = true;
            Wrapper.MakeCurrent();
            Wrapper.TheWorld.Wrappers.Add(Wrapper);
            GL.ClearColor(Color.Gray);
            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.Light0);
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest); //??
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                            (int) TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                            (int) TextureMagFilter.Linear);
            GL.ShadeModel(ShadingModel.Smooth);
            // Enable Light 0 and set its parameters.
            //GL.Light(LightName.Light0, LightParameter.Position, SunPosition);
            GL.Light(LightName.Light0, LightParameter.Ambient, new[] {0.1f, 0.1f, 0.1f, 1.0f});
            //GL.Light(LightName.Light0, LightParameter.Ambient, new[] { 0.6f, 0.6f, 0.6f, 1.0f });
            GL.Light(LightName.Light0, LightParameter.Diffuse, new[] {1.0f, 1.0f, 1.0f, 1.0f});
            GL.Light(LightName.Light0, LightParameter.Specular, new[] {1f, 1f, 1f, 1.0f});
            GL.Light(LightName.Light0, LightParameter.SpotExponent, new[] {1.0f, 1.0f, 1.0f, 1.0f});
            GL.LightModel(LightModelParameter.LightModelAmbient, new[] {0f, 0f, 0f, 1.0f});
            GL.LightModel(LightModelParameter.LightModelLocalViewer, 0);
            GL.LightModel(LightModelParameter.LightModelTwoSide, 0);

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.ColorMaterial); // lets me use colors rather than changing materials
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.Normalize); // Do I need this?  (this make a difference, although I don't know why)
            Wrapper.TheWorld.Tick();
        }

        private void Wrapper_Paint(object sender, PaintEventArgs e)
        {
            if (!Wrapper.Loaded) return;
            Wrapper.PaintScene();
        }

        private void SecondaryView_FormClosing(object sender, FormClosingEventArgs e)
        {
            Wrapper.TheWorld.Wrappers.Remove(Wrapper);
        }

        private void uVSTelescopeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Wrapper.CameraMode = new InstrumentView(Wrapper, Wrapper.TheWorld.LADEE);
            Wrapper.TheWorld.LADEE.ShowAxes = true;
            Wrapper.TheWorld.Tick();
        }

        private void uVSSolarViewerToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }
    }
}
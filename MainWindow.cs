using attplan.Viz;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using attplan.util;
using attplan.spice;

namespace attplan
{
    public partial class MainWindow : Form
    {
        public enum Camera { BioSentinel, Earth, Moon };
        public const float FarUnit = 1000f; // 1000 km
        public const float NearUnit = 1f; // 1 m
        public const double Meters = 0.000001d;  // Also defined in Shape
        public const double Kilometers = 0.001d;
        public const long Minutes = 65536L * 60;
        public const long Seconds = 65536L;
        public const long Hours = 65536L * 60 * 60;
        public const long Days = 65536L * 60 * 60 * 24;

        public const string SpiceDir = @"kernels\";

        public static long FirstTimestamp = 0L;
        public static long LastTimestamp = long.MaxValue;

        public ShaderProgram EarthShader;
        public ShaderProgram MoonShader;
        public PhongRejection1 MoonShaderPhongRejection1;
        public ShaderProgram MoonShaderTexturedPhong;
        public World TheWorld;

        OpenGLControlWrapper glControl1;
        bool _scenarioInhibit, _scenarioUpdated;
        DateTime _scenarioEnd, _scenarioStart;

        public MainWindow()
        {
            InitializeComponent();
            CreateOpenGLControl();

            TheWorld = new World(@"kernels");
            TheWorld.Wrappers.Add(glControl1);
            RbBioSentinel_CheckedChanged(null, null);
        }

        void CreateOpenGLControl()
        {
            // 
            // glControl1
            // 
            glControl1 = new OpenGLControlWrapper();
            Controls.Add(glControl1);
            glControl1.BackColor = Color.Black;
            glControl1.Dock = DockStyle.Fill;
            glControl1.Location = new Point(219, 24);
            glControl1.Name = "glControl1";
            glControl1.Size = new Size(857, 779);
            glControl1.TabIndex = 0;
            glControl1.VSync = false;
            glControl1.Load += new EventHandler(glControl1_Load);
            glControl1.Paint += new PaintEventHandler(glControl1_Paint);
            glControl1.Resize += new EventHandler(glControl1_Resize);
        }

        private void glControl1_Load(object sender, EventArgs e)
        {
            glControl1.Loaded = true;
            glControl1.MakeCurrent();

            CreateShaders();

            //WavefrontShape.MakeCone(20);
            STKModel.MakeCone(20);

            glControl1.TheWorld = TheWorld;

            LoadObjects();

            GL.ClearColor(Color.Black);
            //SetupViewport();

            GL.Enable(EnableCap.Lighting); // Turn off lighting to get color
            GL.Enable(EnableCap.Light0);

            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest); //??
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                            (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                            (int)TextureMagFilter.Linear);

            GL.ShadeModel(ShadingModel.Smooth);

            // Enable Light 0 and set its parameters.
            //GL.Light(LightName.Light0, LightParameter.Position, SunPosition);

            const float ambient = 0.35f;
            const float diffuse = 1f;

            GL.Light(LightName.Light0, LightParameter.Ambient, new[] { ambient, ambient, ambient, 1.0f });
            //GL.Light(LightName.Light0, LightParameter.Ambient, new[] { 0.6f, 0.6f, 0.6f, 1.0f });
            GL.Light(LightName.Light0, LightParameter.Diffuse, new[] { diffuse, diffuse, diffuse, 1.0f });
            GL.Light(LightName.Light0, LightParameter.Specular, new[] { 1f, 1f, 1f, 1.0f });
            GL.Light(LightName.Light0, LightParameter.SpotExponent, new[] { 1.0f, 1.0f, 1.0f, 1.0f });
            GL.LightModel(LightModelParameter.LightModelAmbient, new[] { 0f, 0f, 0f, 1.0f });
            GL.LightModel(LightModelParameter.LightModelLocalViewer, 0);
            GL.LightModel(LightModelParameter.LightModelTwoSide, 0);

            //GL.Material(MaterialFace.Front, MaterialParameter.Ambient, new float[] { 0.3f, 0.3f, 0.3f, 1.0f });
            //GL.Material(MaterialFace.Front, MaterialParameter.Diffuse, new float[] { 1.0f, 1.0f, 1.0f, 1.0f });
            //GL.Material(MaterialFace.Front, MaterialParameter.Specular, new float[] { 0.5f, 0.5f, 0.5f, 1.0f });
            //GL.Material(MaterialFace.Front, MaterialParameter.Emission, new float[] { 0.0f, 0.0f, 0.0f, 1.0f });

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.ColorMaterial); // lets me use colors rather than changing materials
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.Normalize); // Do I need this?  (this make a difference, although I don't know why)

            GL.PointSize(5f);
            GL.Enable(EnableCap.PointSmooth);
            GL.Hint(HintTarget.PointSmoothHint, HintMode.Nicest);

            DateTime start = TheWorld.Fetcher.SpiceStartDate.AddDays(0);
            SetScenarioTimes(LadeeStateFetcher.StateFrame.EarthCenteredJ2000, new DateTime(2020, 6, 27, 21, 9, 14), new DateTime(2021, 4, 16, 1, 57, 13));

            long t = TimeUtilities.DateTimeToTime42(start);
            UpdateToTime(t);

            glControl1.CameraMode = new ArcBall(glControl1, TheWorld.BioSentinel)
            {
                RelativePosition = new Vector3d(0d, 100 * Meters, 0d)
            };
            TheWorld.Tick();
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            if (!glControl1.Loaded) return;
            glControl1.PaintScene();
        }

        private void glControl1_Resize(object sender, EventArgs e)
        {
            if (!glControl1.Loaded) return;
            glControl1.SetupViewport();
            if (MoonShaderPhongRejection1 != null)
            {
                MoonShaderPhongRejection1.CenterX = glControl1.Width / 2f;
                MoonShaderPhongRejection1.CenterY = glControl1.Height / 2f;
                MoonShaderPhongRejection1.AngleFactor = 1 / 50f;
            }
        }

        private void LoadObjects()
        {
            TheWorld.Trajectory = new TrajectoryShape(TheWorld);

            TheWorld.BioSentinel = STKModel.Load(@"Resources\ladee_new.mdl");
            TheWorld.BioSentinel.ShowAxes = true;
            TheWorld.NearShapes.Add(TheWorld.BioSentinel);

            TheWorld.BioSentinel.SetThrusterVisibility(0); // 16 | 1

            /*            if (false)
                        {
                            for (double azimuth = 0d; azimuth < MathHelper.TwoPi; azimuth += MathHelper.DegreesToRadians(5))
                                for (double elevation = MathHelper.DegreesToRadians(-60d);
                                     elevation < MathHelper.DegreesToRadians(60d);
                                     elevation += MathHelper.DegreesToRadians(5))
                                {
                                    const double d = 100000d;
                                    double w = Math.Cos(elevation)*d;
                                    double z = Math.Sin(elevation)*d;
                                    double x = Math.Cos(azimuth)*w;
                                    double y = Math.Sin(azimuth)*w;
                                    var cube = new CubeShape() {Scale = 0.1f, Position = new Vector3d(x, y, z)};
                                    TheWorld.FarShapes.Add(cube);
                                }
                        }*/

            //var cube = new CubeShape() { Scale = 1.5f, Position = new Vector3d(1000d,0d,0d) };
            //TheWorld.FarShapes.Add(cube);

            /*
            _ladee = new WavefrontShape
                {
                    Name = "LADEE",
                    WavefrontFilename = "0a-003-ladee_simplified2.obj",
                    Position = new Vector3d(386400 * Kilometers, 0000 * Kilometers, 0d),
                    ShowAxes = true,
                    AxisScale = 40f,
                    ShowModel = true
                };
            */

            TheWorld.FarShapes.Add(TheWorld.LookaroundPoint);

            TheWorld.Moon = new MoonWithMetadata
            {
                Name = "Moon",
                Position = new Vector3d(384400 * Kilometers, 0d, 0d),
                TextureFilename = @"Resources\moon_8k_color_brim16.jpg",
                ShowAxes = false,
                AxisScale = 4f,
                Shininess = 1f,
                Shader = MoonShader
            };
            TheWorld.Moon.Load();
            TheWorld.FarShapes.Add(TheWorld.Moon);

            // earth_800x400.jpg
            // land_shallow_topo_2011_8192.jpg
            TheWorld.Earth = new Earth
            {
                Name = "Earth",
                Position = new Vector3d(0d, 0d, 0d),
                TextureFilename = @"Resources\earth_800x400.jpg",
                NightFilename = @"Resources\earth_night_800x400.jpg",
                Radius = (float)(6371 * Kilometers),
                XSize = 48,
                YSize = 24,
                ShowAxes = false,
                AxisScale = 10f,
                Specularity = new[] { 1f, 1f, 1f },
                Shininess = 10f,
                Shader = EarthShader
            };
            TheWorld.Earth.Load();
            TheWorld.Earth.LoadTexture();
            TheWorld.FarShapes.Add(TheWorld.Earth);

            // earth_800x400.jpg
            // land_shallow_topo_2011_8192.jpg
            TheWorld.Sun = new TexturedBall
            {
                Name = "Sun",
                Position = new Vector3d(0d, 0d, 0d),
                TextureFilename = @"Resources\sun.png",
                Color = Color.Yellow,
                Radius = (float)(695500 * Kilometers),
                XSize = 32,
                YSize = 16,
                ShowAxes = false,
                AxisScale = 10f
            };
            TheWorld.Sun.Load();
            TheWorld.Sun.LoadTexture();

            TheWorld.Stars = new StarBackground();
            TheWorld.Stars.Load();
        }



        private void CreateShaders()
        {
            EarthShader = new EarthShaderProgram(@"Resources\earth_vs_120.glsl", @"Resources\earth_fs_120.glsl");
            //MoonShader = new ShaderProgram("moon1_vs_120.glsl", "moon1_fs_120.glsl");
            MoonShaderPhongRejection1 = new PhongRejection1(@"Resources\textured_phong_vs_120.glsl", @"Resources\phong_rejection1_fs_120.glsl");
            MoonShaderTexturedPhong = new ShaderProgram(@"Resources\textured_phong_vs_120.glsl", @"Resources\textured_phong_fs_120.glsl");
            MoonShader = null;
        }


        private void RbBioSentinel_CheckedChanged(object sender, EventArgs e) => SetCamera(Camera.BioSentinel);

        private void RbEarth_CheckedChanged(object sender, EventArgs e) => SetCamera(Camera.Earth);

        private void RbMoon_CheckedChanged(object sender, EventArgs e) => SetCamera(Camera.Moon);

        void SetCamera(Camera c)
        {
            switch (c)
            {
                case Camera.BioSentinel:
                    glControl1.CameraMode = new ZoomTo(glControl1, TheWorld.BioSentinel, 10 * Meters, 2 * Meters, new ArcBall(glControl1, TheWorld.BioSentinel));
                    break;
                case Camera.Earth:
                    glControl1.CameraMode = new ZoomTo(glControl1, TheWorld.Earth, 60000d, 1d, new ArcBall(glControl1, TheWorld.Earth));
                    break;
                default:
                    glControl1.CameraMode = new ZoomTo(glControl1, TheWorld.Moon, 20000d, 1d, new ArcBall(glControl1, TheWorld.Moon));
                    break;
            }
            TheWorld.Tick();
        }

        #region Time Handling

        public void SetScenarioTimes(LadeeStateFetcher.StateFrame frame, DateTime start, DateTime stop)
        {
            _scenarioInhibit = true;

            if (start > stop)
            {
                var temp = start;
                start = stop;
                stop = temp;
            }

            TheWorld.Frame = frame;
            _scenarioStart = start;
            _scenarioEnd = stop;
            var start42 = TimeUtilities.DateTimeToTime42(_scenarioStart);
            var stop42 = TimeUtilities.DateTimeToTime42(_scenarioEnd);
            lbIntervalStart.Text = TimeUtilities.Time42ToSTK(start42);
            lbIntervalStop.Text = TimeUtilities.Time42ToSTK(stop42);

            TheWorld.Trajectory.Frame = frame;
            TheWorld.Trajectory.DrawStart = start42;
            TheWorld.Trajectory.DrawStop = stop42;

            _scenarioUpdated = true;
            _scenarioInhibit = false;
        }

        public void UpdateCurrentTime(long time)
        {
            //time = Math.Min(MBLibData.LastTimestamp, Math.Max(MBLibData.FirstTimestamp, time));

            _scenarioInhibit = true;
            tbCurrentTime.Text = TimeUtilities.Time42ToSTK(time);
            _scenarioInhibit = false;

            UpdateToTime(time);
        }

        public void UpdateToTime(long t) => TheWorld.Update(t);

        private void tbCurrentTime_TextChanged(object sender, EventArgs e)
        {
            if (_scenarioInhibit) return;
            DateTime d;
            if (DateTime.TryParse(tbCurrentTime.Text, out d))
            {
                UpdateToTime(TimeUtilities.DateTimeToTime42(d));
                tbCurrentTime.ForeColor = Color.Gray;
            }
            else
            {
                tbCurrentTime.ForeColor = Color.Red;
            }
        }

        #endregion

        // Unused for now
        private void udAmbient_ValueChanged(object sender, EventArgs e)
        {
            //float ambient = ((float)udAmbient.Value) / 100f;
            //GL.Light(LightName.Light0, LightParameter.Ambient, new[] { ambient, ambient, ambient, 1.0f });
            //glControl1.Invalidate();
        }

    }
}

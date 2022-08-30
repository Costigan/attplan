﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace attplan.Viz
{
    public class MoonHeightField : TexturedShape
    {
        public float Radius = 1.7374f;
        public bool ShowTerminator = false;
        public int XSize = 256; // 64
        public int YSize = 128; // 32

        public void Load()
        {
            var vertices = new Vertex[XSize*YSize];
            if (vertices.Length > 65536)
                throw new Exception("Mesh too large");
            float xf = 1f/(XSize - 1);
            float yf = 1f/(YSize - 1);
            for (int y = 0; y < YSize; y++)
            {
                double ya = (yf*y - 0.5d)*Math.PI;
                //Console.WriteLine(yf * y - 0.5d);
                var t = (float) Math.Cos(ya);
                var pz = (float) Math.Sin(ya);
                for (int x = 0; x < XSize; x++)
                {
                    int idx = x*YSize + y;
                    vertices[idx].TexCoord.X = x*xf;
                    vertices[idx].TexCoord.Y = 1f - y*yf;

                    double xa = (x*xf*2f*Math.PI);
                    //double xa = (x * xf * 2f * Math.PI);
                    float px = t*(float) Math.Cos(xa);
                    float py = t*(float) Math.Sin(xa);
                    //Console.WriteLine(@"{0}, {1}, {2}", x * xf, px, py);

                    px = -px; // Helps get the textures aligned to the frame
                    py = -py;

                    vertices[idx].Normal.X = px;
                    vertices[idx].Normal.Y = py;
                    vertices[idx].Normal.Z = pz;

                    vertices[idx].Position.X = px*Radius;
                    vertices[idx].Position.Y = py*Radius;
                    vertices[idx].Position.Z = pz*Radius;

                    //Console.WriteLine(@"idx={0}, [x={1}, y={2}] -> [{3}, {4}, {5}]", idx, x, y, px, py, pz);
                }
            }

            // Define a mesh
            var elements = new ushort[(XSize - 1)*(YSize - 1)*6];
            int ptr = 0;
            int xMax = XSize - 1;
            int yMax = YSize - 1;
            for (int x = 0; x < xMax; x++)
                for (int y = 0; y < yMax; y++)
                {
                    int v = x*YSize + y;

                    //Console.WriteLine(@"v={0}", v);

                    elements[ptr++] = (ushort) (v + YSize); // a
                    elements[ptr++] = (ushort) (v + 1); // b
                    elements[ptr++] = (ushort) v; // c

                    //Console.WriteLine(@"tri [{0}, {1}, {2}]", x + YSize, v + 1, v);

                    elements[ptr++] = (ushort) (v + YSize); // a
                    elements[ptr++] = (ushort) (v + YSize + 1); // b
                    elements[ptr++] = (ushort) (v + 1); // c

                    //Console.WriteLine(@"tri [{0}, {1}, {2}]", v + YSize, v + YSize + 1, v + 1);
                }
            Buffer = LoadVBO(vertices, elements, InterleavedArrayFormat.T2fN3fV3f);

            BoundingSphereRadius = Radius;
            BoundingSphereDefined = true;
        }

        public override void Paint()
        {
            GL.Color3(1f, 1f, 1f);
            base.Paint();
        }
    }

    public class MoonDEM : Shape
    {
        public const double Radius = 1.7374d;
        public static bool ShowTexture = true;
        public Color Color = Color.White;

        public uint[] Elements;
        protected int ElementsHandle;

        public List<DEMFragment> Fragments = new List<DEMFragment>();

        public NormalAverager[] NormalBuffer = new NormalAverager[0];
        public int NumElements;
        public bool SurfaceLines = false;
        public string TextureFilename = null;
        protected int TextureHandle;
        public InterleavedArrayFormat VertexFormat;
        private TerrainManager _terrain;
        private bool _useDEMs = true;

        public MoonDEM()
        {
            VertexFormat = InterleavedArrayFormat.T2fN3fV3f;
        }

        public TerrainManager Terrain
        {
            get { return _terrain ?? (_terrain = TerrainManager.GetTerrrainManager()); }
        }

        public void Load()
        {
            LoadTexture();
            LoadElements();
            LoadDEMs();

            BoundingSphereRadius = Radius + 0.3d;
            BoundingSphereDefined = true;
        }

        public void LoadTexture()
        {
            if (TextureFilename == null)
                return;
            GL.GenTextures(1, out TextureHandle);
            GL.BindTexture(TextureTarget.Texture2D, TextureHandle);
            var bitmap = new Bitmap(TextureFilename);

            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                                              ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (float) TextureEnvMode.Modulate);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                            (float) TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                            (float) TextureMagFilter.Linear);

            // tell OpenGL to build mipmaps out of the bitmap data
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.GenerateMipmap, 1.0f);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                          OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

            bitmap.UnlockBits(data);
        }

        public void LoadElements()
        {
            const int xSize = DEMFragment.Width;
            const int ySize = DEMFragment.Height;

            // Define a mesh
            int ptr = 0;
            const int xMax = xSize - 1;
            const int yMax = ySize - 1;
            var elements = new uint[xMax*yMax*6];
            for (int x = 0; x < xMax; x++)
                for (int y = 0; y < yMax; y++)
                {
                    int v = y*xSize + x;

                    elements[ptr++] = (uint) v; // c
                    elements[ptr++] = (uint) (v + 1); // b
                    elements[ptr++] = (uint) (v + xSize); // a

                    elements[ptr++] = (uint) (v + 1); // c
                    elements[ptr++] = (uint) (v + xSize + 1); // b
                    elements[ptr++] = (uint) (v + xSize); // a
                }

            int size;
            GL.GenBuffers(1, out ElementsHandle);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementsHandle);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr) (elements.Length*sizeof (uint)), elements,
                          BufferUsageHint.StaticDraw);
            GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out size);
            if (elements.Length*sizeof (uint) != size)
                throw new ApplicationException("Element data not uploaded correctly");

            NumElements = elements.Length;
            Elements = elements;
        }

        public void LoadDEMs()
        {
            //fragments = CreateFragments(4, 2, 0, 360, 90, -90);  // breaks due to lat=-90

            const int depth = 1; // 5 is as far as the dataset allows.  4 runs out of memory.
            Fragments = CreateFragments(4, 2, -180, 180, 45, -45, depth);

            Fragments.Add(new DEMFragment(this, -180, 180, 90, 45, 0));
            Fragments.Add(new DEMFragment(this, -180, 180, -45, -89, 0));

            foreach (var f in Fragments)
            {
                f.LoadGPU();
                f.UnloadRam();
            }
        }

        public bool UseDEMs
        {
            get { return _useDEMs; }
            set {
                if (value == _useDEMs) return;
                _useDEMs = value;
                foreach (var f in Fragments)
                    f.UnloadSurfaceAndChildren();
                DEMFragment.UseDEM = _useDEMs;
                foreach (var f in Fragments)
                {
                    f.LoadGPU();
                    f.UnloadRam();
                }
            }
        }

        public List<DEMFragment> CreateFragments(int xfragments, int yfragments, double west, double east, double north,
                                                 double south, int depth)
        {
            var result = new List<DEMFragment>();
            double xstep = (east - west)/xfragments;
            double ystep = (north - south)/yfragments;
            for (int y = 0; y < yfragments; y++)
                for (int x = 0; x < xfragments; x++)
                {
                    double w = west + x*xstep;
                    double e = w + xstep;
                    double s = south + y*ystep;
                    double n = s + ystep;
                    Console.WriteLine(@"Creating fragment [{0}, {1}, {2}, {3}]", w, e, n, s);
                    var f = new DEMFragment(this, w, e, n, s, depth) {Color = Color.White};
                    result.Add(f);
                }
            return result;
        }

        public override void Paint()
        {
            if (ShowAxes)
                PaintAxes(AxisScale);

            if (Shader != null)
                Shader.UseProgram();

            GL.Color3(Color);
            GL.Material(MaterialFace.Front, MaterialParameter.Specular, Specularity);
            GL.Material(MaterialFace.Front, MaterialParameter.Shininess, Shininess); //Shininess
            GL.ShadeModel(ShadingModel.Smooth);

            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, TextureHandle);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementsHandle);

            for (int i = 0; i < Fragments.Count; i++)
            {
                DEMFragment t = Fragments[i];
                t.Paint(ShowTexture, VertexFormat, NumElements);
            }

            // testing
            //var t = Fragments[0];
            //t.Paint(ShowTexture, VertexFormat, NumElements);


            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.DisableClientState(ArrayCap.VertexArray);
            GL.DisableClientState(ArrayCap.NormalArray);
            GL.DisableClientState(ArrayCap.IndexArray);
            GL.Disable(EnableCap.Texture2D);

            if (Shader != null)
                Shader.StopUsingProgram();
        }

        public void SetLevelOfDetail(double lat, double lon)
        {
            for (int i = 0; i < Fragments.Count; i++)
                Fragments[i].SetLevelOfDetail(lat, lon);
        }

        public NormalAverager[] GetNormalAverager(int count)
        {
            if (NormalBuffer != null && NormalBuffer.Length >= count)
                return NormalBuffer;
            NormalBuffer = new NormalAverager[count];
            return NormalBuffer;
        }
    }

    public class DEMFragment
    {
        public const double Radius = 1.7374d;
        public const int XSteps = 256; // 256
        public const int YSteps = 128; // 128
        public const int Width = XSteps + 1; // 257
        public const int Height = YSteps + 1; // 129
        public const int VertexCount = Width*Height; // 33153
        public const int VertexByteCount = VertexCount*32; // 1060896   ... sizeof(Vertex)=32;
        public const int TriangleCount = XSteps*YSteps*2; // 65536
        public const int TriangleByteCount = TriangleCount*6; // 393216
        private readonly bool _regenerate;
        public List<DEMFragment> Children;
        public Color Color = Color.White;
        public Vertex[] CornerVertices = new Vertex[4];

        public MoonDEM DEM;
        public int Depth;
        public double East;
        private string _filenameBody;

        public bool InRam = false;
        public double North;
        public bool OnGPU = false;
        public double South;

        public int VertexHandle;
        public byte[] Vertices;
        public double West;

        public static bool UseDEM = true;

        public DEMFragment(MoonDEM dem, double west, double east, double north, double south, int depth,
                           bool regenerate = false)
        {
            DrawChildren = false;
            DEM = dem;
            West = west;
            East = east;
            North = north;
            South = south;
            Depth = depth;
            InRam = false;
            OnGPU = false;
            if (_filenameBody == null)
                _filenameBody = tos(west) + "w_" + tos(east) + "e_" + tos(north) + "n_" + tos(south) + "s.dem";
            _regenerate = regenerate;

            if (Depth > 0)
                CreateChildren(Depth);
        }

        public string Filename => MBLibData.Combine((UseDEM ? "DEM_cache\\dem_" : "DEM_cache\\flat_dem_") + _filenameBody);

        public bool DrawChildren { get; set; }

        private string tos(double v)
        {
            return v < 0d ? (-v).ToString(@"-000.00000") : v.ToString(@"000.00000");
        }

        public void LoadRam()
        {
            if (Vertices != null)
                return;
            if (!_regenerate && File.Exists(Filename))
            {
                Vertices = File.ReadAllBytes(Filename);
                return;
            }
            Console.WriteLine(@"Defining {0}", Filename);
            var v = new Vertex[VertexCount];
            TerrainManager m = DEM.Terrain;
            double xstep = (East - West)/XSteps;
            double ystep = (North - South)/YSteps;
            for (int ilat = 0; ilat < Height; ilat++)
            {
                double lat = South + ilat*ystep;
                double alat = lat*Math.PI/180d;
                double textureY = 0.5d + (lat/-180d);
                double sinlat = Math.Sin(alat);
                double coslat = Math.Cos(alat);
                for (int ilon = 0; ilon < Width; ilon++)
                {
                    double lon = West + ilon*xstep;
                    var alt = UseDEM ? (double)m.ReadTerrainHeight(lat, lon < 0d ? lon + 360d : lon) : 0d;

                    //alt *= 2d; // Testing

                    double rad = Radius + alt/1000000f;

                    double alon = lon*Math.PI/180d;
                    double sinlon = Math.Sin(alon);
                    double coslon = Math.Cos(alon);

                    double z = sinlat;
                    double w = coslat;
                    double x = w*coslon;
                    double y = w*sinlon;

                    int idx = ilat*Width + ilon;
                    //int idx = ilon * Height + ilat;

                    double textureX = 0.5d + lon/360d;

                    //if (textureX > 0.999d)
                    //    Console.WriteLine(@"here");

                    //if (textureX > 1d || textureY > 1d || textureX < 0d || textureY < 0d)
                    //    throw new Exception(@"texture out-of-bounds");

                    v[idx].TexCoord.X = (float) textureX;
                    v[idx].TexCoord.Y = (float) textureY; // ((lat + 90d) / 180d);

                    v[idx].Position.X = (float) (rad*x);
                    v[idx].Position.Y = (float) (rad*y);
                    v[idx].Position.Z = (float) (rad*z);

                    v[idx].Normal.X = (float) x;
                    v[idx].Normal.Y = (float) y;
                    v[idx].Normal.Z = (float) z;
                }
            }

            if (true)
            {
                // Average the normals
                NormalAverager[] buf = DEM.GetNormalAverager(VertexCount);
                for (int i = 0; i < VertexCount; i++)
                    buf[i].Reset();

                uint[] elements = DEM.Elements;
                int triCount = elements.Length/3;
                int ptr = 0;
                for (int tri = 0; tri < triCount; tri++)
                {
                    uint p1 = elements[ptr++];
                    uint p2 = elements[ptr++];
                    uint p3 = elements[ptr++];
                    Vector3 v1 = v[p1].Position;
                    Vector3 v2 = v[p2].Position;
                    Vector3 v3 = v[p3].Position;

                    //Console.WriteLine(@"textureCoords=[{0},{1}], [{2},{3}], [{4},{5}]",
                    //    v[p1].TexCoord.X, v[p1].TexCoord.Y,
                    //    v[p2].TexCoord.X, v[p2].TexCoord.Y,
                    //    v[p3].TexCoord.X, v[p3].TexCoord.Y);

                    Vector3 n;
                    Shape.FindNormal(ref v1, ref v2, ref v3, out n);

                    if (float.IsNaN(n.X))
                        throw new Exception(@"Got NaN when creating a mesh normal");

                    buf[p1].Add(n);
                    buf[p2].Add(n);
                    buf[p3].Add(n);

                    /*
                    if (tri == 0)
                    {
                        Console.WriteLine(@"First tri");
                        Console.WriteLine(@"  p1={0} p2={1} p3={2}", p1, p2, p3);
                        Console.WriteLine(@"  v1={0} v2={1} v3={2}", v1, v2, v3);
                        Console.WriteLine(@"  n1={0} n2={1} n3={2}", v[p1].Normal, v[p2].Normal, v[p3].Normal);
                        Console.WriteLine(@"  calculated normal={0}", n);
                        Console.WriteLine();
                    }
                    */

                    /*
                    buf[p1].Add(v[p1].Normal);
                    buf[p2].Add(v[p1].Normal);
                    buf[p3].Add(v[p2].Normal);
                    */
                }
                for (int i = 0; i < VertexCount; i++)
                {
                    Vector3 norm = buf[i].Normal/buf[i].Count;
                    norm.Normalize();

                    /*
                    if (i == 0 || i == 1 || i == 257)
                    {
                        Console.WriteLine(@"point[{0}].Normal (before)={1}", i, v[i].Normal);
                        Console.WriteLine(@"new normal={0}", norm);
                    }
                    */

                    v[i].Normal = norm;
                }
            }

            //CornerVertices[x] = v[ilat * Width + ilon]
            CornerVertices[0] = v[0*Width + 0];
            CornerVertices[1] = v[0*Width + (Width - 1)];
            CornerVertices[2] = v[(Height - 1)*Width + 0];
            CornerVertices[3] = v[(Height - 1)*Width + (Width - 1)];

            // Write to file
            using (var fs = new FileStream(Filename, FileMode.Create, FileAccess.Write))
            using (var bw = new BinaryWriter(fs))
            {
                for (int i = 0; i < v.Length; i++)
                {
                    bw.Write(v[i].TexCoord.X);
                    bw.Write(v[i].TexCoord.Y);
                    bw.Write(v[i].Normal.X);
                    bw.Write(v[i].Normal.Y);
                    bw.Write(v[i].Normal.Z);
                    bw.Write(v[i].Position.X);
                    bw.Write(v[i].Position.Y);
                    bw.Write(v[i].Position.Z);
                }
            }
            // Read the file back
            Vertices = File.ReadAllBytes(Filename);
        }

        public void LoadGPU()
        {
            if (VertexHandle > 0)
                return;
            LoadRam();
            GL.GenBuffers(1, out VertexHandle);
            int size;
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexHandle);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr) (Vertices.Length*BlittableValueType.StrideOf(Vertices)),
                          Vertices, BufferUsageHint.StaticCopy);
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out size);
            if (Vertices.Length*BlittableValueType.StrideOf(Vertices) != size)
                throw new ApplicationException("Vertex data not uploaded correctly");
        }

        public void UnloadGPU()
        {
            if (VertexHandle < 1)
                return;
            GL.DeleteBuffer(VertexHandle);
            VertexHandle = 0;
        }

        public void UnloadSurfaceAndChildren()
        {
            UnloadGPU();
            UnloadRam();
            if (Children != null)
                foreach (var c in Children)
                    c.UnloadSurfaceAndChildren();
        }

        public void UnloadRam()
        {
            Vertices = null;
        }

        private void CreateChildren(int depth)
        {
            int childDepth = depth - 1;
            double centerLat = (North + South)/2d;
            double centerLon = (West + East)/2d;
            Children = new List<DEMFragment>
                {
                    new DEMFragment(DEM, West, centerLon, North, centerLat, childDepth),// {Color = Color.LightGoldenrodYellow},
                    new DEMFragment(DEM, West, centerLon, centerLat, South, childDepth),// {Color = Color.LightBlue},
                    new DEMFragment(DEM, centerLon, East, North, centerLat, childDepth),// {Color = Color.LightCoral},
                    new DEMFragment(DEM, centerLon, East, centerLat, South, childDepth),// {Color = Color.LightGreen}
                };
        }

        public void SetLevelOfDetail(double lat, double lon)
        {
            //Console.WriteLine(@"SetLevelOfDetail: [lat={4}, lon={5}] for [{0}, {1}, {2}, {3}]", West, East, North, South, lat, lon);
            LoadGPU();
            DrawChildren = lon >= West && lon <= East && lat <= North && lat >= South;
            if (DrawChildren && Depth > 0)
                for (int i = 0; i < 4; i++)
                    Children[i].SetLevelOfDetail(lat, lon);
        }

        public void Paint(bool showTexture, InterleavedArrayFormat vertexFormat, int numElements)
        {
            if (DrawChildren && Depth > 0)
            {
                for (int i = 0; i < 4; i++)
                    Children[i].Paint(showTexture, vertexFormat, numElements);
            }
            else if (VertexHandle > 0)
            {
                GL.Color3(Color);
                GL.BindBuffer(BufferTarget.ArrayBuffer, VertexHandle);
                GL.InterleavedArrays(vertexFormat, 0, IntPtr.Zero); // Can this go outside the loop?

                if (!DEM.SurfaceLines)
                {
                    GL.DrawElements(showTexture ? BeginMode.Triangles : BeginMode.LineStrip,
                                    numElements, DrawElementsType.UnsignedInt, IntPtr.Zero);
                }
                else
                {
                    GL.DrawElements(BeginMode.Triangles, numElements, DrawElementsType.UnsignedInt, IntPtr.Zero);
                    GL.DepthFunc(DepthFunction.Lequal);

                    GL.Disable(EnableCap.Lighting);
                    GL.Color3(1f, 0f, 0f);
                    GL.DrawElements(BeginMode.LineStrip, numElements, DrawElementsType.UnsignedInt, IntPtr.Zero);
                    GL.DepthFunc(DepthFunction.Less);
                    GL.Enable(EnableCap.Lighting);
                }
            }
            else
            {
                Console.WriteLine(@"Paint: no texture for [{0}, {1}, {2}, {3}]", West, East, North, South);
            }
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct NormalAverager
    {
        public int Count;
        public Vector3 Normal;

        public void Reset()
        {
            Count = 0;
            Normal.X = Normal.Y = Normal.Z = 0f;
        }

        public void Add(Vector3 v)
        {
            Count++;
            Normal.X += v.X;
            Normal.Y += v.Y;
            Normal.Z += v.Z;
        }
    }
}
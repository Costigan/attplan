using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace attplan.spice
{
    public class SpiceManager
    {
        public const int SpacecraftId = -12;
        public const double MoonRadius = 1737.1d;
        public const double DegToRad = 3.141592653589793238D/180d;
        public const double RadToDeg = 180d/3.141592653589793238D;
        public static string SpiceKernelDirectory = @"Kernels\";

        public static string StandardKernelFiles =
            @"de421.bsp, moon_pa_de421_1900-2050.bpc, moon_080317.tf, moon_assoc_me.tf, pck00010.tpc, naif0010.tls";

        //private static readonly string Pictur = "MON DD,YYYY  HR:MN:SC.####";

        /// <summary>
        ///     Load an ephemeris file for use by the readers.  Return that file's handle, to be used by other SPK routines to refer to the file.
        /// </summary>
        /// <param name="filename">Name of the file to be loaded.</param>
        /// <param name="handle">Loaded file's handle. (output)</param>
        [DllImport("cspice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void spklef_c(string filename, ref int handle);

        //private static extern void spklef_c([MarshalAs(UnmanagedType.LPStr)]string filename, ref int handle);

        /// <summary>
        ///     Return the state (position and velocity) of a target body relative to an observing body, optionally corrected for light time (planetary aberration) and stellar aberration.
        /// </summary>
        /// <param name="targ">Target body name.</param>
        /// <param name="et">Observer epoch.</param>
        /// <param name="refer">Reference frame of output state vector.</param>
        /// <param name="abcorr">Aberration correction flag.</param>
        /// <param name="obs">Observing body name.</param>
        /// <param name="starg">State of target.</param>
        /// <param name="lt">One way light time between observer and target.</param>
        [DllImport("cspice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void spkezr_c([MarshalAs(UnmanagedType.LPStr)] string targ,
                                           double et,
                                           string refer,
                                           string abcorr,
                                           string obs,
                                           double[] starg,
                                           ref double lt);

        /// <summary>
        ///     Load one or more SPICE kernels into a program.
        /// </summary>
        /// <param name="filename">Name of SPICE kernel file (text or binary).</param>
        [DllImport("cspice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void furnsh_c([MarshalAs(UnmanagedType.LPStr)] string filename);

        /// <summary>
        ///     Convert a string representing an epoch to a double precision value representing the number of TDB seconds past the J2000 epoch corresponding to the input epoch.
        /// </summary>
        /// <param name="pictur">A string representing an epoch.</param>
        /// <param name="et">The equivalent value in seconds past J2000, TDB. (output)</param>
        [DllImport("cspice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void str2et_c([MarshalAs(UnmanagedType.LPStr)] string pictur, ref double et);

        /// <summary>
        ///     Convert from rectangular coordinates to latitudinal coordinates.
        /// </summary>
        /// <param name="rectan">Rectangular coordinates of a point.</param>
        /// <param name="radius">Distance of the point from the origin.</param>
        /// <param name="longitude">Longitude of the point in radians.</param>
        /// <param name="latitude">Latitude of the point in radians.</param>
        [DllImport("cspice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void reclat_c(double[] rectan,
                                           ref double radius,
                                           ref double longitude,
                                           ref double latitude);

        /// <summary>
        ///     Return the position of a target body relative to an observing body, optionally corrected for light time (planetary aberration) and stellar aberration.
        /// </summary>
        /// <param name="targ">Target body name.</param>
        /// <param name="et">Observer epoch.</param>
        /// <param name="refFrame">Reference frame of output position vector.</param>
        /// <param name="abcorr">Aberration correction flag.</param>
        /// <param name="obs">Observing body name.</param>
        /// <param name="ptarg">Position of target. (output)</param>
        /// <param name="lt">One way light time between observer and target. (output)</param>
        [DllImport("cspice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void spkpos_c([MarshalAs(UnmanagedType.LPStr)] string targ,
                                           double et,
                                           [MarshalAs(UnmanagedType.LPStr)] string refFrame,
                                           [MarshalAs(UnmanagedType.LPStr)] string abcorr,
                                           [MarshalAs(UnmanagedType.LPStr)] string obs,
                                           double[] ptarg,
                                           ref double lt);

        /// <summary>
        ///     Return the position of a target body relative to an observing body, optionally corrected for light time (planetary aberration) and stellar aberration.
        /// </summary>
        /// <param name="targ">Target body NAIF ID code.</param>
        /// <param name="et">Observer epoch.</param>
        /// <param name="refFrame">Reference frame of output position vector.</param>
        /// <param name="abcorr">Aberration correction flag.</param>
        /// <param name="obs">Observing body NAIF ID code.</param>
        /// <param name="ptarg">Position of target.</param>
        /// <param name="lt">One way light time between observer and target.</param>
        [DllImport("cspice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void spkezp_c(int targ,
                                           double et,
                                           [MarshalAs(UnmanagedType.LPStr)] string refFrame,
                                           [MarshalAs(UnmanagedType.LPStr)] string abcorr,
                                           int obs,
                                           double[] ptarg,
                                           ref double lt);

        /// <summary>
        ///     Compute the geometric position of a target body relative to an observing body.
        /// </summary>
        /// <param name="targ">Target body.</param>
        /// <param name="et">Target epoch.</param>
        /// <param name="refFrame">Target reference frame.</param>
        /// <param name="obs">Observing body.</param>
        /// <param name="pos">Position of target. (output)</param>
        /// <param name="lt">Light time. (output)</param>
        [DllImport("cspice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void spkgps_c(int targ,
                                           double et,
                                           [MarshalAs(UnmanagedType.LPStr)] string refFrame,
                                           int obs,
                                           double[] pos,
                                           ref double lt);


        /// <summary>
        ///     Compute the geometric state (position and velocity) of a target body relative to an observing body.
        /// </summary>
        /// <param name="targ">Target body.</param>
        /// <param name="et">Target epoch.</param>
        /// <param name="refFrame">Target reference frame.</param>
        /// <param name="obs">Observing body.</param>
        /// <param name="state">State of target. (output)</param>
        /// <param name="lt">Light time. (output)</param>
        [DllImport("cspice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void spkgeo_c(int targ,
                                           double et,
                                           [MarshalAs(UnmanagedType.LPStr)] string refFrame,
                                           int obs,
                                           double[] state,
                                           ref double lt);

        /// <summary>
        ///     Convert rectangular coordinates to planetographic coordinates.
        /// </summary>
        /// <param name="body">Body with which coordinate system is associated.</param>
        /// <param name="rectan">Rectangular coordinates of a point.</param>
        /// <param name="re">Equatorial radius of the reference spheroid.</param>
        /// <param name="f">Flattening coefficient.</param>
        /// <param name="lon">Planetographic longitude of the point (radians). (output)</param>
        /// <param name="lat">Planetographic latitude of the point (radians). (output)</param>
        /// <param name="alt">Altitude of the point above reference spheroid. (output)</param>
        [DllImport("cspice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void recpgr_c([MarshalAs(UnmanagedType.LPStr)] string body,
                                           double[] rectan,
                                           double re,
                                           double f,
                                           ref double lon,
                                           ref double lat,
                                           ref double alt);

        /// <summary>
        ///     Convert planetographic coordinates to rectangular coordinates.
        /// </summary>
        /// <param name="body">Body with which coordinate system is associated.</param>
        /// <param name="lon">Planetographic longitude of a point (radians).</param>
        /// <param name="lat">Planetographic latitude of a point (radians).</param>
        /// <param name="alt">Altitude of a point above reference spheroid.</param>
        /// <param name="re">Equatorial radius of the reference spheroid.</param>
        /// <param name="f">Flattening coefficient.</param>
        /// <param name="rectan">Rectangular coordinates of the point.</param>
        [DllImport("cspice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void pgrrec_c([MarshalAs(UnmanagedType.LPStr)] string body,
                                           double lon,
                                           double lat,
                                           double alt,
                                           double re,
                                           double f,
                                           double[] rectan);

        //[3]

        /// <summary>
        ///     Convert an input time from ephemeris seconds past J2000 to Calendar, Day-of-Year, or Julian Date format, UTC.
        /// </summary>
        /// <param name="et">is the input epoch, ephemeris seconds past J2000.</param>
        /// <param name="format">is the format of the output time string.</param>
        /// <param name="prec">is the number of digits of precision to which fractional seconds (for Calendar and Day-of-Year formats) or days (for Julian Date format) are to be computed. If PREC is zero or smaller, no decimal point is appended to the output string. If PREC is greater than 14, it is treated as 14.</param>
        /// <param name="lenout">The allowed length of the output string.  This length must large enough to hold the output string plus the null terminator.  If the output string is expected to have x characters, lenout must be x + 1.</param>
        /// <param name="utcstr">is the output time string equivalent to the input epoch, in the specified format.</param>
        [DllImport("cspice.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void et2utc_c(double et,
                                            [MarshalAs(UnmanagedType.LPStr)] string format,
                                            int prec,
                                            int lenout,
                                            [MarshalAs(UnmanagedType.LPStr)] StringBuilder utcstr);

        /// <summary>
        ///     Find nearest point on a triaxial ellipsoid to a specified line,
        ///     and the distance from the ellipsoid to the line. If the line
        ///     intersects the ellipsoid, dist is zero.
        /// </summary>
        /// <param name="a">Length of ellipsoid's semi-axis in the x direction</param>
        /// <param name="b">Length of ellipsoid's semi-axis in the y direction</param>
        /// <param name="c">Length of ellipsoid's semi-axis in the z direction</param>
        /// <param name="linept">Point on line</param>
        /// <param name="linedr">Direction vector of line</param>
        /// <param name="pnear">Nearest point on ellipsoid to line (output)</param>
        /// <param name="dist">Distance of ellipsoid from line (output)</param>
        [DllImport("cspice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void npedln_c(double a,
                                           double b,
                                           double c,
                                           double[] linept,
                                           double[] linedr,
                                           double[] pnear,
                                           ref double dist);

        /// <summary>
        ///     Find the limb of a triaxial ellipsoid, viewed from a specified point.
        /// </summary>
        /// <param name="a">Length of ellipsoid semi-axis lying on the x-axis.</param>
        /// <param name="b">Length of ellipsoid semi-axis lying on the y-axis.</param>
        /// <param name="c">Length of ellipsoid semi-axis lying on the z-axis.</param>
        /// <param name="viewpt">Location of viewing point.</param>
        /// <param name="limb">Limb of ellipsoid as seen from viewing point. (output)</param>
        [DllImport("cspice.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void edlimb_c(double a,
                                            double b,
                                            double c,
                                            double[] viewpt,
                                            ref SpiceEllipse limb);

        /// <summary>
        ///     Convert a CSPICE ellipse to a center vector and two generating vectors.  The selected generating vectors are semi-axes of the ellipse.
        /// </summary>
        /// <param name="ellipse">A CSPICE ellipse.</param>
        /// <param name="center">Center of the ellipse (output)</param>
        /// <param name="smajor">semi-major axis of the ellipse (output)</param>
        /// <param name="sminor">semi-minor axis of the ellipse (output)</param>
        [DllImport("cspice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void el2cgv_c(ref SpiceEllipse ellipse,
                                           double[] center,
                                           double[] smajor,
                                           double[] sminor);

        public static string et2utc_c_temp(double et, string format, int prec, int lenout)
        {
            var buf = new StringBuilder(' ', lenout);
            et2utc_c(et, format, prec, lenout, buf);
            return buf.ToString();
        }

        [DllImport("cspice.dll", CallingConvention = CallingConvention.Cdecl)]
        // private static extern void timout_c(ref double et, [MarshalAs(UnmanagedType.LPStr)]string pictur, ref int lenout, char[] buf);
        public static extern void timout_c(double et,
                                           [MarshalAs(UnmanagedType.LPStr)] string pictur,
                                           int lenout,
                                           char[] buf);

        [DllImport("cspice.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void psv2pl_c(double[] point,
                                            double[] span1,
                                            double[] span2,
                                            ref SpicePlane plane);

        /// <summary>
        ///     Find the intersection of a ray and a plane.
        /// </summary>
        /// <param name="vertex">Vertex of a ray</param>
        /// <param name="dir">Direction vector of a ray</param>
        /// <param name="plane">A CSPICE plane.</param>
        /// <param name="nxpts">Number of intersection points of ray and plane (output)</param>
        /// <param name="xpt">Intersection point, if nxpts = 1 (output)</param>
        [DllImport("cspice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void inrypl_c(double[] vertex,
                                           double[] dir,
                                           ref SpicePlane plane,
                                           ref int nxpts,
                                           double[] xpt);

        /// <summary>
        ///     Find the nearest point on an ellipse to a specified point, both
        ///     in three-dimensional space, and find the distance between the
        ///     ellipse and the point.
        /// </summary>
        /// <param name="point">Point whose distance to an ellipse is to be found.</param>
        /// <param name="ellips">A CSPICE ellipse.</param>
        /// <param name="pnear">Nearest point on ellipse to input point.</param>
        /// <param name="dist">Distance of input point to ellipse.</param>
        [DllImport("cspice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void npelpt_c(double[] point,
                                           ref SpiceEllipse ellips,
                                           double[] pnear,
                                           ref double dist);

        /// <summary>
        ///     Find the intersection of an ellipse and a plane.
        /// </summary>
        /// <param name="ellips">A CSPICE ellipse.</param>
        /// <param name="plane">A CSPICE plane.</param>
        /// <param name="nxpts">Number of intersection points of plane and ellipse. (output)</param>
        /// <param name="xpt1">Intersection point (output)</param>
        /// <param name="xpt2">Intersection point (output)</param>
        [DllImport("cspice.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void inelpl_c(ref SpiceEllipse ellips,
                                            ref SpicePlane plane,
                                            ref int nxpts,
                                            double[] xpt1,
                                            double[] xpt2);

        /// <summary>
        ///     Negate a 3D vector
        /// </summary>
        /// <param name="v1">input vector</param>
        /// <param name="vout">output vector</param>
        [DllImport("cspice.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void vminus_c(double[] v1,
                                            double[] vout);


        /// <summary>
        ///     Compute the normalized cross product of two 3-vectors.
        /// </summary>
        /// <param name="v1">Left vector for cross product.</param>
        /// <param name="v2">Right vector for cross product.</param>
        /// <param name="vout">Normalized cross product (v1xv2) / |v1xv2|. (output)</param>
        [DllImport("cspice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ucrss_c(double[] v1,
                                          double[] v2,
                                          double[] vout);

        public static double Distance3(double[] a, double[] b)
        {
            return Math.Sqrt(Math.Pow(a[0] - b[0], 2d) +
                             Math.Pow(a[1] - b[1], 2d) +
                             Math.Pow(a[2] - b[2], 2d));
        }

        /// <summary>
        ///     Open a new CK file, returning the handle of the opened file.
        /// </summary>
        /// <param name="fname">The name of the CK file to be opened.</param>
        /// <param name="ifname">The internal filename for the CK.</param>
        /// <param name="ncomch">The number of characters to reserve for comments.</param>
        /// <param name="handle">The handle of the opened CK file.</param>
        [DllImport("cspice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ckopn_c([MarshalAs(UnmanagedType.LPStr)] string fname,
                                          [MarshalAs(UnmanagedType.LPStr)] string ifname,
                                          int ncomch,
                                          ref int handle);

        /// <summary>
        ///     Close an open CK file.
        /// </summary>
        /// <param name="handle">Handle of the CK file to be closed.</param>
        [DllImport("cspice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ckcls_c(int handle);

        /// <summary>
        ///     Add a type 3 segment to a C-kernel.
        /// </summary>
        /// <param name="handle">Handle of an open CK file.</param>
        /// <param name="begtim">The beginning encoded SCLK of the segment.</param>
        /// <param name="endtim">The ending encoded SCLK of the segment.</param>
        /// <param name="inst">The NAIF instrument ID code.</param>
        /// <param name="refFrame">The reference frame of the segment.</param>
        /// <param name="avflag">True if the segment will contain angular velocity.</param>
        /// <param name="segid">Segment identifier.</param>
        /// <param name="nrec">Number of pointing records.</param>
        /// <param name="sclkdp">Encoded SCLK times.</param>
        /// <param name="quats">Quaternions representing instrument pointing.</param>
        /// <param name="avvs">Angular velocity vectors.</param>
        /// <param name="nints">Number of intervals.</param>
        /// <param name="starts">Encoded SCLK interval start times.</param>
        [DllImport("cspice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ckw03_c(int handle,
                                          double begtim,
                                          double endtim,
                                          int inst,
                                          [MarshalAs(UnmanagedType.LPStr)] string refFrame,
                                          int avflag, // SpiceBoolean
                                          [MarshalAs(UnmanagedType.LPStr)] string segid,
                                          int nrec,
                                          double[] sclkdp,
                                          double[] quats, //[][4],
                                          double[] avvs, //[][3],
                                          int nints,
                                          double[] starts);

//           handle     is the handle of the CK file to which the segment will 
//              be written. The file must have been opened with write 
//              access. 
// 
//   begtim     is the beginning encoded SCLK time of the segment. This 
//              value should be less than or equal to the first time in 
//              the segment. 
// 
//   endtim     is the encoded SCLK time at which the segment ends. 
//              This value should be greater than or equal to the last 
//              time in the segment. 
// 
//   inst       is the NAIF integer ID code for the instrument. 
// 
//   ref        is a character string which specifies the  
//              reference frame of the segment. This should be one of 
//              the frames supported by the SPICELIB routine NAMFRM 
//              which is an entry point of FRAMEX. 
// 
//              The rotation matrices represented by the quaternions
//              that are to be written to the segment transform the
//              components of vectors from the inertial reference frame
//              specified by ref to components in the instrument fixed
//              frame. Also, the components of the angular velocity
//              vectors to be written to the segment should be given
//              with respect to ref.
//
//              ref should be the name of one of the frames supported
//              by the SPICELIB routine NAMFRM.
//
//
//   avflag     is a boolean flag which indicates whether or not the 
//              segment will contain angular velocity. 
// 
//   segid      is the segment identifier.  A CK segment identifier may 
//              contain up to 40 characters, excluding the terminating
//              null.
// 
//   nrec       is the number of pointing instances in the segment. 
// 
//   sclkdp     are the encoded spacecraft clock times associated with 
//              each pointing instance. These times must be strictly 
//              increasing. 
// 
//   quats      is an array of SPICE-style quaternions representing a
//              sequence of C-matrices. See the discussion of "Quaternion
//              Styles" in the Particulars section below.
//
//              The C-matrix represented by the ith quaternion in
//              quats is a rotation matrix that transforms the
//              components of a vector expressed in the inertial
//              frame specified by ref to components expressed in
//              the instrument fixed frame at the time sclkdp[i].
//
//              Thus, if a vector V has components x, y, z in the
//              inertial frame, then V has components x', y', z' in
//              the instrument fixed frame where:
//
//                   [ x' ]     [          ] [ x ]
//                   | y' |  =  |   cmat   | | y |
//                   [ z' ]     [          ] [ z ]
//
//   avvs       are the angular velocity vectors ( optional ).
//
//              The ith vector in avvs gives the angular velocity of
//              the instrument fixed frame at time sclkdp[i]. The
//              components of the angular velocity vectors should
//              be given with respect to the inertial reference frame
//              specified by ref.
//
//              The direction of an angular velocity vector gives
//              the right-handed axis about which the instrument fixed
//              reference frame is rotating. The magnitude of the
//              vector is the magnitude of the instantaneous velocity
//              of the rotation, in radians per second.
//
//              If avflag is FALSE then this array is ignored by the
//              routine; however it still must be supplied as part of
//              the calling sequence.
//
//   nints      is the number of intervals that the pointing instances
//              are partitioned into.
//
//   starts     are the start times of each of the interpolation
//              intervals. These times must be strictly increasing
//              and must coincide with times for which the segment
//              contains pointing.

        //public static string SpiceKernelDirectory = @"C:\UVS\svn\src\TestData\Kernels\";

        //, naif0010.tls.pc

        public void LoadStandardKernels()
        {
            LoadStandardKernels(StandardKernelFiles);
        }

        public void LoadStandardKernels(string kernels)
        {
            string[] filenames = kernels.Split(',').Select(name => name.Trim()).ToArray();
            LoadStandardKernels(filenames);
        }

        public void LoadStandardKernels(string[] filenames)
        {
            foreach (string filename in filenames)
            {
                string fullname = Path.GetFullPath(Path.Combine(SpiceKernelDirectory, filename));
                Console.WriteLine(@"Furnishing {0}", fullname);
                furnsh_c(fullname);
            }
        }

        public void LoadEphemeris(string filename)
        {
            furnsh_c(filename);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SpiceEllipse
        {
            public double centerX;
            public double centerY;
            public double centerZ;
            public double semiMajorX;
            public double semiMajorY;
            public double semiMajorZ;
            public double semiMinorX;
            public double semiMinorY;
            public double semiMinorZ;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SpicePlane
        {
            public double normal0;
            public double normal1;
            public double normal2;
            public double constant;
        }
    }
}
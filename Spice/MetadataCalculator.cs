using attplan.util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace attplan.spice
{
    public class MetadataCalculator
    {
        public const double DegToRad = Math.PI / 180d;
        public const double RadToDeg = 180d / Math.PI;
        public const double LunarRadiusD = 1737.4d;
        public const float LunarRadius = 1737.4f;
        public const float MinTerrain = -6.6695f + LunarRadius;
        public const float MaxTerrain = 10.774f + LunarRadius;

        public static MetadataCalculator Calculator;

        public const string LadeeBodyFrame = "LADEE_SC_PROP";
        public const string TelescopeFrame = "LADEE_UVSTEL";
        public const string SolarViewerFrame = "LADEE_UVSSOL";

        public CSpice Spice;

        public string SpiceRoot;
        public long LatestSpiceTime42;

        //public AttitudeSupplier AttitudeSupplier;
        public const long QuaternionTimeout = 65536L * 3600L;  // 1 hour: If the quaternion returned from the supplier is out of date more than this, then ignore it

        public double[] OriginVector = new[] { 0d, 0d, 0d };

        public const double FOVHalfAngle = 0.5d*Math.PI/180d;  // Half angle for both telescope and solar viewer

        public MetadataCalculator(CSpice spice)
        {
            Spice = spice;
        }

        public void FurnishSpiceKernels(string spiceKernelRoot)
        {
            FurnishSclk(spiceKernelRoot);
            FurnishFk(spiceKernelRoot);
            FurnishTrajectories(spiceKernelRoot);
            FurnishAttitudes(spiceKernelRoot);
        }

        public void FurnishFk(string root)
        {
            var clockRoot = Path.Combine(root, @"fk\");
            var di = new DirectoryInfo(clockRoot);
            var files = di.GetFiles("*.tf");
            files = files.OrderBy(a => a.Name).ToArray();
            if (files.Length < 1)
            {
                Console.WriteLine(@"Cannot find the frames kernel (.tf)");
                return;
            }
            Furnish(files[0].FullName);
        }

        public void FurnishSclk(string root)
        {
            var clockRoot = Path.Combine(root, @"sclk\");
            var di = new DirectoryInfo(clockRoot);
            var files = di.GetFiles("*.tsc");
            files = files.OrderBy(a => a.Name).ToArray();
            if (files.Length < 1)
            {
                Console.WriteLine(@"Cannot find the clock correction file (.tsc)");
                return;
            }
            Furnish(files[files.Length - 1].FullName);
        }

        public void FurnishTrajectories(string root)
        {
            var trajectoryRoot = Path.Combine(root, @"spk\definitive\");

            var di = new DirectoryInfo(trajectoryRoot);
            var files = di.GetFiles("*.bsp");
            files = files.OrderBy(a => a.Name).Where(a => !a.Name.Equals("ladee_r_13290_13321_v01.bsp")).ToArray();
            if (files.Length < 1)
            {
                Console.WriteLine(@"Cannot find the preliminary definitive ephemeris.");
                return;
            }
            var currentDefinitive = files[files.Length - 1].FullName;
            var definitiveEnd = GetEndDay(currentDefinitive);

            var predictedRoot = Path.Combine(root, @"spk\predicted\");
            var pdi = new DirectoryInfo(predictedRoot);
            var pfiles = pdi.GetFiles("*.bsp").Select(p => p.FullName).OrderByDescending(a => a).ToArray();
            var loadPredicts = new List<string>();
            int beginDay = 15000;
            foreach (var pf in pfiles)
            {
                var en = GetEndDay(pf);
                if (en < definitiveEnd) continue;
                var bg = GetBeginDay(pf);         
                if (bg < beginDay)
                {
                    loadPredicts.Add(pf);
                    beginDay = bg;
                }
                if (beginDay < definitiveEnd)
                    break;
            }

            loadPredicts.Reverse();
            foreach (var p in loadPredicts)
                FurnishTrajectory(p);

            FurnishTrajectory(Path.Combine(trajectoryRoot, "ladee_r_13250_13279_v01.bsp"));
            FurnishTrajectory(Path.Combine(trajectoryRoot, "ladee_r_13279_13286_v01.bsp"));
            FurnishTrajectory(Path.Combine(trajectoryRoot, "ladee_r_13286_13293_v01.bsp"));
            FurnishTrajectory(currentDefinitive);
        }

        private static int GetBeginDay(string path)
        {
            var n = Path.GetFileName(path);
            if (n == null) return 15000;   // Past the end of the mission
            if (n.Length < 20) return 15000;
            var s = n.Substring(8, 5);
            int r;
            return !int.TryParse(s, out r) ? 15000 : r;
        }

        private static int GetEndDay(string path)
        {
            var n = Path.GetFileName(path);
            if (n == null) return 15000;   // Past the end of the mission
            if (n.Length < 20) return 15000;
            var s = n.Substring(14, 5);
            int r;
            return !int.TryParse(s, out r) ? 15000 : r;
        }

        public void FurnishTrajectory(string path)
        {
            if (File.Exists(path))
                Furnish(path);
            else
                Console.Error.WriteLine("Tried to furnish non-existant kernel: {0}", path);
        }

        public static void Furnish(string path)
        {
            Console.WriteLine(@"Furnishing {0}", path);
            CSpice.furnsh_c(path);
        }

        /// <summary>
        /// Furnish attitudes from earliest to latest, ignoring previous versions of files
        /// </summary>
        /// <param name="root"></param>
        private void FurnishAttitudes(string root)
        {
            var attitudeRoot = Path.Combine(root, @"ck\definitive\");
            var di = new DirectoryInfo(attitudeRoot);
            var fileArray = di.GetFiles("*.bc");
            fileArray = fileArray.OrderBy(a => a.Name).Reverse().ToArray();
            var files = new List<FileInfo>();
            var previousFile = "";  // This is the previous filename without the version spec (_vxx)
            foreach (var fi in fileArray)
            {
                var withoutVersion = fi.Name.Substring(0, 20);
                if (previousFile.Equals(withoutVersion, StringComparison.InvariantCultureIgnoreCase))
                    continue;
                previousFile = withoutVersion;
                files.Add(fi);
            }

            // Work out the bounds of definitive attitude

            var earliest = long.MaxValue;
            var latest = long.MinValue;
            foreach (var fname in files)
            {
                long b;
                long e;
                CSpice.CkGetCoverage(fname.FullName, out b, out e);
                earliest = Math.Min(earliest, b);
                latest = Math.Max(latest, e);
            }

            // Furnish the attitude files

            files.Reverse();  // Furnish them from the earliest to the latest
            foreach (var fi in files)
                Furnish(fi.FullName);

            LatestSpiceTime42 = latest;
            
            // Load latest attitudes from telemetry
            Console.WriteLine(@"Latest attitude in spice CK kernels: {0}", TimeUtilities.Time42ToString(LatestSpiceTime42));
        }

        /*
        public void LoadAttitudeFromTelemetry(string uvsRoot)
        {
            if (Calculator == null || Calculator.AttitudeSupplier != null)
                return;
            var supplier = new TelemetryAttitudeSupplier();
            Calculator.AttitudeSupplier = supplier;

            // Load telemetry from before the SOC's CK files
            // Now covered by new CKs from Cory (not checked in, though)
            //LoadTelemetryForDay(new DateTime(2013, 9, 14), uvsRoot);  // doy 258
            //LoadTelemetryForDay(new DateTime(2013, 9, 15), uvsRoot);  // doy 258
            //LoadTelemetryForDay(new DateTime(2013, 9, 17), uvsRoot);  // doy 260
            //LoadTelemetryForDay(new DateTime(2013, 9, 18), uvsRoot);  // doy 261

            // Load telemetry after the SOC's CK files
            var l = TimeUtilities.Time42ToDateTime(Calculator.LatestSpiceTime42);
            var day = new DateTime(l.Year, l.Month, l.Day);
            // Load a day's worth of attitude telemetry until there is no more
            var flag = true;
            while (flag)
            {
                flag = LoadTelemetryForDay(day, uvsRoot);
                day = day.AddDays(1);
            }

            supplier.Sort();

            const string filename = "temporary.ck";
            Console.WriteLine(@"Writing {0} quaternions to temporary ck file {1}", supplier.Quaternions.Count, filename);
            AttitudeSupplier.WriteQuaternionsToFile(supplier.Quaternions, filename);
            CSpice.furnsh_c(filename);
        }

        private bool LoadTelemetryForDay(DateTime day, string uvsRoot)
        {
            Console.WriteLine(@"Loading telemetry for day {0}, {1}", day, TimeUtilities.DateTimeToString(day));
            var flag = false;
            var yearDay = string.Format(@"{0:D4}_{1:D3}", day.Year, day.DayOfYear);
            var di = new DirectoryInfo(Path.Combine(uvsRoot, yearDay));
            foreach (var childDi in di.EnumerateDirectories("*", SearchOption.AllDirectories))
            {
                foreach (var fi in childDi.EnumerateFiles("SC_HK_*.HK.0"))
                {
                    flag = true;

                    var c = Calculator.AttitudeSupplier as TelemetryAttitudeSupplier;
                    //Console.WriteLine(@"before Span is [{0}, {1}]", TimeUtilities.Time42ToString(c.BeginSpan), TimeUtilities.Time42ToString(c.EndSpan));

                    Calculator.AttitudeSupplier.Load(fi.FullName);


                    Console.WriteLine(@"Loading attitude from {0}.", fi.FullName);
                    if (c == null)
                        Console.WriteLine(@"  Span is unknown");
                    else
                        Console.WriteLine(@"  Span is now [{0}, {1}]", TimeUtilities.Time42ToString(c.BeginSpan), TimeUtilities.Time42ToString(c.EndSpan));
                }
            }
            return flag;
        }

        public bool AddGeometricMetadata(long rawTime42, BaseSpectrum m)
        {
            var qstk = StkAttitudeAtTime(rawTime42);
            if (qstk == null)
                return false;
            Calculator.AddGeometricMetadata(rawTime42, m, qstk);
            return true;
        }

        public double[] StkAttitudeAtTime(long rawTime42)
        {
            var qstk = new double[4];
            if (StkAttitudeAtTime(rawTime42, qstk))
                return qstk;
            if (AttitudeSupplier == null)
                return null;
            var qd = AttitudeSupplier.QuaternionAt(rawTime42);
            if (Math.Abs(rawTime42 - qd.Timestamp) > QuaternionTimeout)
                return null;
            qstk[0] = qd.X;
            qstk[1] = qd.Y;
            qstk[2] = qd.Z;
            qstk[3] = qd.W;
            //Console.WriteLine("  Got quaternion from supplier");
            return qstk;
        }
        */

        public bool StkAttitudeAtTime(long rawTime42, double[] qstk)
        {
            /*  This worked.
            var et = TimeUtilities.Time42ToET(rawTime42);

            const int sc = CSpice.LadeeId;
            const int sc1 = -12000;

            var sclkdp = 0d;  // Encoded spacecraft time

            CSpice.sce2c_c(sc, et, ref sclkdp);

            var cmat = new double[3, 3];
            var clkout = 0d;
            var found = 0;
            CSpice.ckgp_c(sc1, sclkdp, 0d, "J2000", cmat, ref clkout, ref found);
             * */

            var sclkdp = (double)rawTime42;
            const int sc1 = -12000;
            var cmat = new double[3, 3];
            var clkout = 0d;
            var found = 0;
            CSpice.ckgp_c(sc1, sclkdp, 0d, "J2000", cmat, ref clkout, ref found);

            if (found == 0)
            {
                var s = TimeUtilities.Time42ToString((long) sclkdp);
                //Console.Error.WriteLine(@"Couldn't find an attitude at time {0}, {1}", sclkdp, s);
                return false;
            }

            var qspice = new double[4];
            CSpice.m2q_c(cmat, qspice);
            Calculator.Spice2StkQuat(qspice, qstk);
            return true;
        }

        private double AltOfPoint(double[] v)
        {
            return Math.Sqrt(v[0]*v[0] + v[1]*v[1] + v[2]*v[2]);
        }

        // Validated against Boris' fortran example
        public void Spice2StkQuat(double[] qspice, double[] qstk)
        {
            qstk[0] = -qspice[1];
            qstk[1] = -qspice[2];
            qstk[2] = -qspice[3];
            qstk[3] = qspice[0];
        }

        public void Stk2SpiceQuat(double[] qstk, double[] qspice)
        {
            qspice[0] = qstk[3];
            qspice[1] = -qstk[0];
            qspice[2] = -qstk[1];
            qspice[3] = -qstk[2];
        }

    }
}

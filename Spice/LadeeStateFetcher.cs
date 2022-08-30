using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using attplan.util;
using OpenTK;

namespace attplan.spice
{
    public class LadeeStateFetcher
    {
        public enum StateFrame
        {
            EarthFixed,
            MoonFixed,
            EarthCenteredJ2000,
            EarthCenteredMoonFollowing,
            SelenocentricSolarEcliptic,
            LadeeSolarEcliptic,
            LadeeLVLH
        }

        public const int LadeeAttitudeId = -12000;
        public const int LadeeId = -12;
        public const int EarthId = 399;
        public const int MoonId = 301;
        public const int SunId = 10;
        public const double DegToRad = Math.PI/180d;
        public const double RadToDeg = 180d/Math.PI;
        public const double LunarRadiusD = 1737.4d;
        public const float LunarRadius = 1737.4f;
        public const float MinTerrain = -6.6695f + LunarRadius;
        public const float MaxTerrain = 10.774f + LunarRadius;

        public const string J2000Frame = "J2000";
        public const string MoonFrame = "MOON_ME";
        public const string LadeeBodyFrame = "LADEE_SC_PROP";
        public const string TelescopeFrame = "LADEE_UVSTEL";
        public const string SolarViewerFrame = "LADEE_UVSSOL";
        public const double FOVHalfAngle = 0.5d*Math.PI/180d; // Half angle for both telescope and solar viewer

        public static LadeeStateFetcher StateFetcher;

        public long LatestSpiceTime42;
        public double[] OriginVector = new[] {0d, 0d, 0d};
        public CSpice Spice;
        public string SpiceRoot;
        public static string DataTarget { get; set; }

        public DateTime SpiceStartDate = new DateTime(2013, 9, 8, 0, 0, 0);
        public DateTime SpiceStopDate = new DateTime(2014, 2, 9, 17, 0, 0);
        private StateFrame _frame = StateFrame.EarthFixed;
        private string _spiceFrame = J2000Frame;
        private int _spiceObserver = EarthId;

        private SpiceState _tempState = new SpiceState();

        #region Initialization

        public LadeeStateFetcher(string spiceRoot)
        {
            Spice = new CSpice();
            //Spice.LoadStandardKernels();
            FurnishSpiceKernels(spiceRoot, "metakernel.txt");
            SpiceRoot = spiceRoot;
        }

        public static LadeeStateFetcher GetFetcher(string spiceRoot)
        {
            if (StateFetcher != null)
                return StateFetcher;
            StateFetcher = new LadeeStateFetcher(spiceRoot);
            return StateFetcher;
        }

        public static LadeeStateFetcher GetFetcher()
        {
            return StateFetcher;
        }

        // I used to use a spice metakernel, but that didn't interact well with my relative path
        // infrastructure.  So, I've implemented a simple version of its functionality
        public void FurnishSpiceKernels(string spiceKernelRoot, string rootFile)
        {
            var metakernelPath = Path.Combine(spiceKernelRoot, rootFile);
            foreach (var line in File.ReadAllLines(metakernelPath).Where(line => !string.IsNullOrEmpty(line) && line[0] != ' '))
                Furnish(Path.Combine(spiceKernelRoot, CanonicalizeDirectorySeparators(line)));
        }

        public static string Combine(string subpath)
        {
            return Path.Combine(DataTarget, subpath);
        }

        public static string CanonicalizeDirectorySeparators(string path)
        {
            path = path.Replace('/', Path.DirectorySeparatorChar);
            path = path.Replace('\\', Path.DirectorySeparatorChar);
            return path;
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

        #endregion Initialization

        #region Frames

        public StateFrame Frame
        {
            get { return _frame; }
            set
            {
                _frame = value;
                switch (_frame)
                {
                    case StateFrame.EarthCenteredJ2000:
                    case StateFrame.EarthFixed:
                        _spiceFrame = J2000Frame;
                        _spiceObserver = EarthId;
                        break;
                    case StateFrame.SelenocentricSolarEcliptic:
                        _spiceFrame = "LADEE_SSE";
                        _spiceObserver = MoonId;
                        break;
                    case StateFrame.EarthCenteredMoonFollowing:
                        _spiceFrame = "LADEE_MF";
                        _spiceObserver = MoonId;
                        break;
                    case StateFrame.LadeeLVLH:
                        _spiceFrame = "LADEE_LVLH";
                        _spiceObserver = MoonId;
                        break;
                    case StateFrame.MoonFixed:
                    default:
                        _spiceFrame = MoonFrame;
                        _spiceObserver = MoonId;
                        break;
                }
            }
        }

        #endregion Frames

        public bool SpiceAttitudeAtTime(long rawTime42, double[] qspice)
        {
            var sclkdp = (double) rawTime42;
            const int sc1 = -12000;
            var cmat = new double[3,3];
            double clkout = 0d;
            int found = 0;
            CSpice.ckgp_c(sc1, sclkdp, 0d, J2000Frame, cmat, ref clkout, ref found);

            if (found == 0)
            {
                //var s = TimeUtilities.Time42ToString((long) sclkdp);
                //Console.Error.WriteLine(@"Couldn't find an attitude at time {0}, {1}", sclkdp, s);
                return false;
            }

            CSpice.m2q_c(cmat, qspice);
            return true;
        }

        public void Fetch(long rawTime42, Vector3d position, Quaternion attitude)
        {
            if (rawTime42 == 0)
                Console.Error.WriteLine("Time is 0.");
            const int scId = CSpice.LadeeId;
            var sclkdp = (double) rawTime42;
            var etCorrected = 0d;
            CSpice.sct2e_c(scId, sclkdp, ref etCorrected);

            long trueTime42 = TimeUtilities.ETToTime42(etCorrected);
            var qspice = new double[4];
            SpiceAttitudeAtTime(trueTime42, qspice);

            attitude.W = (float) qspice[0];
            attitude.X = (float) qspice[1];
            attitude.Y = (float) qspice[2];
            attitude.Z = (float) qspice[3];

            // Ladee State and Position in ME frame
            var ladeeState = new double[6];
            double lt = 0d;

            // The LADEE's state in ME frame
            CSpice.spkgeo_c(CSpice.LadeeId, etCorrected, _spiceFrame, _spiceObserver, ladeeState, ref lt);
            position.X = ladeeState[0];
            position.Y = ladeeState[1];
            position.Z = ladeeState[2];
        }

        public void FetchPosition(int id, long rawTime42, ref Vector3d position)
        {
            var sclkdp = (double) rawTime42;
            var etCorrected = 0d;
            CSpice.sct2e_c(CSpice.LadeeId, sclkdp, ref etCorrected);

            var ladeeState = new double[6];
            var lt = 0d;
            CSpice.spkgeo_c(id, etCorrected, _spiceFrame, _spiceObserver, ladeeState, ref lt);


            //var sb = new StringBuilder();
            //CSpice.et2utc_c(etCorrected, "C", 14, 40, sb);
            //Console.WriteLine(@"FetchPosition etCorrected={0}", sb);
            //CSpice.spkpos_c("-12", etCorrected, _spiceFrame, "NONE", "MOON", ladeeState, ref lt);


            position.X = ladeeState[0];
            position.Y = ladeeState[1];
            position.Z = ladeeState[2];
        }

        public bool FetchPositionAndAttitude(int id, long rawTime42, ref Vector3d position, ref Quaternion q)
        {
            if (rawTime42 > MainWindow.LastTimestamp)
            {
                rawTime42 = MainWindow.LastTimestamp;
                //Console.WriteLine(@"Clipped rawTime42={0}", TimeUtilities.Time42ToDateTimeString(rawTime42));
            }
            if (rawTime42 < MainWindow.FirstTimestamp)
                rawTime42 = MainWindow.FirstTimestamp;

            //var dt = TimeUtilities.Time42ToDateTime(rawTime42);
            //Console.WriteLine(@"Fetch pos and att at {0}", dt);
            //if (dt.Year == 2013)
            //    Console.WriteLine(@"here");

            var sclkdp = (double) rawTime42;
            var etCorrected = 0d;
            CSpice.sct2e_c(CSpice.LadeeId, sclkdp, ref etCorrected);

            var ladeeState = new double[6];
            var lt = 0d;
            CSpice.spkgeo_c(id, etCorrected, _spiceFrame, _spiceObserver, ladeeState, ref lt);
            position.X = ladeeState[0];
            position.Y = ladeeState[1];
            position.Z = ladeeState[2];

            var cmat = new double[3,3];
            if (id == LadeeId)
            {
                const int attId = -12000;
                var clkout = 0d;
                var found = 0;
                CSpice.ckgp_c(attId, sclkdp, 0d, _spiceFrame, cmat, ref clkout, ref found);
                if (found == 0)
                    return false;
            }
            else
            {
                if (id == MoonId)
                    CSpice.pxform_c(_spiceFrame, "MOON_ME", etCorrected, cmat);
                else if (id == EarthId)
                    CSpice.pxform_c(_spiceFrame, "IAU_EARTH", etCorrected, cmat);
                else if (id == SunId)
                    CSpice.pxform_c(_spiceFrame, "IAU_SUN", etCorrected, cmat);
            }

            var qspice = new double[4];
            CSpice.m2q_c(cmat, qspice);

            q.W = (float) qspice[0];
            q.X = (float) qspice[1];
            q.Y = (float) qspice[2];
            q.Z = (float) qspice[3];
            return true;
        }

        public bool FetchLADEEPositionVelocityAndAttitude(long rawTime42, ref Vector3d position, ref Quaternion q, ref Vector3d velocity)
        {
            if (rawTime42 > MainWindow.LastTimestamp)
            {
                rawTime42 = MainWindow.LastTimestamp;
                //Console.WriteLine(@"Clipped rawTime42={0}", TimeUtilities.Time42ToDateTimeString(rawTime42));
            }
            if (rawTime42 < MainWindow.FirstTimestamp)
                rawTime42 = MainWindow.FirstTimestamp;

            //var dt = TimeUtilities.Time42ToDateTime(rawTime42);
            //Console.WriteLine(@"Fetch pos and att at {0}", dt);
            //if (dt.Year == 2013)
            //    Console.WriteLine(@"here");

            var sclkdp = (double)rawTime42;
            var etCorrected = 0d;
            CSpice.sct2e_c(CSpice.LadeeId, sclkdp, ref etCorrected);

            var ladeeState = new double[6];
            var lt = 0d;
            CSpice.spkgeo_c(LadeeStateFetcher.LadeeId, etCorrected, _spiceFrame, _spiceObserver, ladeeState, ref lt);
            position.X = ladeeState[0];
            position.Y = ladeeState[1];
            position.Z = ladeeState[2];

            velocity.X = ladeeState[3];
            velocity.Y = ladeeState[4];
            velocity.Z = ladeeState[5];

            //Console.WriteLine(@"--Vel=[{0},{1},{2}]", ladeeState[3], ladeeState[4], ladeeState[5]);
            velocity.NormalizeFast();
            //Console.WriteLine(@"  Vel=[{0},{1},{2}]", velocity.X, velocity.Y, velocity.Z);

            var cmat = new double[3, 3];

                const int attId = -12000;
                var clkout = 0d;
                var found = 0;
                CSpice.ckgp_c(attId, sclkdp, 0d, _spiceFrame, cmat, ref clkout, ref found);
                if (found == 0)
                    return false;

            var qspice = new double[4];
            CSpice.m2q_c(cmat, qspice);

            q.W = (float)qspice[0];
            q.X = (float)qspice[1];
            q.Y = (float)qspice[2];
            q.Z = (float)qspice[3];
            return true;
        }

        public bool FetchPositionAndAttitudeInMoonMe(int id, long rawTime42, ref Vector3d position,
                                                     ref Vector3d velocity, ref Quaternion q)
        {
            var sclkdp = (double) rawTime42;
            double etCorrected = 0d;
            CSpice.sct2e_c(CSpice.LadeeId, sclkdp, ref etCorrected);

            var ladeeState = new double[6];
            double lt = 0d;
            CSpice.spkgeo_c(id, etCorrected, MoonFrame, MoonId, ladeeState, ref lt);
            position.X = ladeeState[0];
            position.Y = ladeeState[1];
            position.Z = ladeeState[2];
            velocity.X = ladeeState[3];
            velocity.Y = ladeeState[4];
            velocity.Z = ladeeState[5];

            //
            var cmat = new double[3,3];

            if (id == LadeeId)
            {
                const int attId = -12000;
                double clkout = 0d;
                int found = 0;
                CSpice.ckgp_c(attId, sclkdp, 0d, MoonFrame, cmat, ref clkout, ref found);
                if (found == 0)
                    return false;
            }
            else
            {
                if (id == MoonId)
                    CSpice.pxform_c(MoonFrame, "MOON_ME", etCorrected, cmat);
                else if (id == EarthId)
                    CSpice.pxform_c(MoonFrame, "IAU_EARTH", etCorrected, cmat);
                else if (id == SunId)
                    CSpice.pxform_c(MoonFrame, "IAU_SUN", etCorrected, cmat);
            }

            var qspice = new double[4];
            CSpice.m2q_c(cmat, qspice);

            q.W = (float) qspice[0];
            q.X = (float) qspice[1];
            q.Y = (float) qspice[2];
            q.Z = (float) qspice[3];
            return true;
        }

        public bool FetchJ200Attitude(long rawTime42, ref Quaternion q)
        {
            if (rawTime42 > MainWindow.LastTimestamp)
                rawTime42 = MainWindow.LastTimestamp;
            if (rawTime42 < MainWindow.FirstTimestamp)
                rawTime42 = MainWindow.FirstTimestamp;

            var sclkdp = (double)rawTime42;
            var etCorrected = 0d;
            CSpice.sct2e_c(CSpice.LadeeId, sclkdp, ref etCorrected);

            var cmat = new double[3, 3];
            CSpice.pxform_c(_spiceFrame, "J2000", etCorrected, cmat);

            var qspice = new double[4];
            CSpice.m2q_c(cmat, qspice);

            q.W = (float)qspice[0];
            q.X = (float)qspice[1];
            q.Y = (float)qspice[2];
            q.Z = (float)qspice[3];
            return true;
        }


        // Validated against Boris' fortran example
        public void Spice2StkQuat(double[] qspice, double[] qstk)
        {
            qstk[0] = -qspice[1]; // x
            qstk[1] = -qspice[2]; // y
            qstk[2] = -qspice[3]; // z
            qstk[3] = qspice[0]; // w
        }

        public void Stk2SpiceQuat(double[] qstk, double[] qspice)
        {
            qspice[0] = qstk[3]; // w
            qspice[1] = -qstk[0]; // x
            qspice[2] = -qstk[1]; // y
            qspice[3] = -qstk[2]; // z
        }
    }

    public class SpiceState
    {
        public static List<SpiceState> States = new List<SpiceState>();
        public static Stack<int> FreeIndexes = new Stack<int>();
        private readonly double[] _fixedSpiceQuaternion = new double[4]; // spice format

        // Moon ME fixed frame
        private readonly double[] _fixedState = new double[6];
        private double _etCorrected;
        private readonly LadeeStateFetcher _fetcher;
        private double _altitude;

        private long _fixedLatLonTimestamp;
        private double _fixedLatitude;
        private double _fixedLongitude;
        private long _fixedSpiceQuaternionTimestamp;
        private long _fixedStateTimestamp;

        private readonly double[] _moonJ2000FixedState = new double[6];
        private readonly double[] _oscelt = new double[8];
        private long _osceltTimestamp;

        // Selenocentric solar fixed frame

        // Frame Independent
        private long _independentSolarAzimuthElevationTimestamp;
        private double _solarAzimuth;
        private double _solarElevation;
        private long _solarLatLonTimestamp;
        private double _solarLatitude;
        private double _solarLongitude;
        private double[] _solarState = new double[6];
        private long _solarStateTimestamp;

        private double _terrainHeight;
        private long _terrainTimestamp;

        public SpiceState()
        {
            _fetcher = LadeeStateFetcher.GetFetcher();
        }

        public double[] FixedState(long time)
        {
            if (time == _fixedStateTimestamp)
                return _fixedState;

            _fixedStateTimestamp = time;

            const int scId = CSpice.LadeeId;
            var sclkdp = (double) time;
            var etCorrected = 0d;
            CSpice.sct2e_c(scId, sclkdp, ref etCorrected);
            _etCorrected = etCorrected;

            var lt = 0d;
            var a = _fixedState;
            CSpice.spkgeo_c(CSpice.LadeeId, etCorrected, LadeeStateFetcher.MoonFrame, LadeeStateFetcher.MoonId, a, ref lt);
            var d = Math.Sqrt(a[0]*a[0] + a[1]*a[1] + a[2]*a[2]);
            _altitude = d - LadeeStateFetcher.LunarRadiusD;
            return _fixedState;
        }

        public double Altitude(long time)
        {
            FixedState(time);
            return _altitude;
        }

        public double FixedVelocity(long time)
        {
            FixedState(time);
            double[] s = _fixedState;
            return Math.Sqrt(s[3]*s[3] + s[4]*s[4] + s[5]*s[5]);
        }

        public double[] FixedSpiceQuaternion(long time)
        {
            if (_fixedSpiceQuaternionTimestamp == time)
                return _fixedSpiceQuaternion;
            _fixedSpiceQuaternionTimestamp = time;
            _fetcher.SpiceAttitudeAtTime(time, _fixedSpiceQuaternion);
            return _fixedSpiceQuaternion;
        }

        public double FixedLatitude(long time)
        {
            if (_fixedLatLonTimestamp == time)
                return _fixedLatitude;
            _fixedLatLonTimestamp = time;

            double[] state = FixedState(time);

            double ladeeLon = 0d, ladeeLat = 0d, ladeeAlt = 0d;
            CSpice.recpgr_c("MOON", state, CSpice.MoonRadius, 0d, ref ladeeLon, ref ladeeLat, ref ladeeAlt);

            _fixedLongitude = ladeeLon*180d/3.141592653589d;
            _fixedLatitude = ladeeLat*180d/3.141592653589d;
            return _fixedLatitude;
        }

        public double FixedLongitude(long time)
        {
            FixedLatitude(time);
            return _fixedLongitude;
        }

        public double Apoapsis(long time)
        {
            Oscelt(time, 0);
            var a = _oscelt[0] / (1d - _oscelt[1]);
            return a * (1d + _oscelt[1]) - LadeeStateFetcher.LunarRadiusD;
        }

        public double Periapsis(long time)
        {
            return Oscelt(time, 0) - LadeeStateFetcher.LunarRadiusD;
        }

        public double Eccentricity(long time)
        {
            return Oscelt(time, 1);
        }

        public double Inclination(long time)
        {
            return Oscelt(time, 2) * 180d/Math.PI;
        }

        public double LongitudeOfAscendingNode(long time)
        {
            return Oscelt(time, 3);
        }

        public double ArgumentOfPeriapsis(long time)
        {
            return Oscelt(time, 4);
        }

        public double MeanAnomalyAtEpoch(long time)
        {
            return Oscelt(time, 5);
        }

        public double Epoch(long time)
        {
            return Oscelt(time, 6);
        }

        public double GravitationalParameter(long time)
        {
            return Oscelt(time, 7);
        }

        public double Oscelt(long time, int index)
        {
            if (_osceltTimestamp != time)
            {
                const int scId = CSpice.LadeeId;
                var sclkdp = (double)time;
                var etCorrected = 0d;
                CSpice.sct2e_c(scId, sclkdp, ref etCorrected);
                _etCorrected = etCorrected;

                var lt = 0d;
                var state = _moonJ2000FixedState;
                CSpice.spkgeo_c(CSpice.LadeeId, etCorrected, "MOON_J2000", LadeeStateFetcher.MoonId, state, ref lt);

                // This returned the same value as the constant below
                //var muary = new double[1];
                //int size=0;
                //CSpice.bodvrd_c("MOON", "GM", 1, ref size, muary);
                //Console.WriteLine(@"mu={0}", muary[0]);

                const double muMoon = 4902.8000661638d;
                CSpice.oscelt_c(state, _etCorrected, muMoon, _oscelt);
                _osceltTimestamp = time;
            }
            return _oscelt[index];
        }

        #region resource

        public static int AllocateIndex()
        {
            if (FreeIndexes.Count > 0)
                return FreeIndexes.Pop();
            States.Add(new SpiceState());
            return States.Count - 1;
        }

        public static void ReturnIndex(int index)
        {
            FreeIndexes.Push(index);
        }

        public static SpiceState Get(int index)
        {
            return States[index];
        }

        #endregion
    }
}
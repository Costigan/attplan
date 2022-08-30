using System;
using System.IO;

namespace attplan.util
{
    public class ThrusterTree
    {
        private byte[] _thrusters;
        private long[] _timestamps;
        private long threshold = 6554L; // 0.1 sec

        public ThrusterTree(string filename)
        {
            Load(filename);
        }

        public unsafe void Load(string filename)
        {
            byte[] bytes = File.ReadAllBytes(filename);
            int count = bytes.Length / 9;
            _timestamps = new long[count];
            _thrusters = new byte[count];
            fixed (byte* pData = bytes)
            {
                byte* p = pData;
                for (int i = 0; i < count; i++)
                {
                    _timestamps[i] = *((long*)p);
                    p += 8;
                }
            }
            Array.Copy(bytes, count * 8, _thrusters, 0, count);
        }

        public byte Thrusters(long timestamp)
        {
            int i = Array.BinarySearch(_timestamps, timestamp);
            if (i < 0) i = ~i;
            if (i >= _thrusters.Length) return 0;
            long t = _timestamps[i];
            long delta = t - timestamp;
            if (delta > threshold || -delta > threshold) return 0;
            return _thrusters[i];
        }
    }
}
using System;

namespace attplan.util
{
    public interface IUpdateTimeListener
    {
        void UpdateCurrentTime(Int64 time);
        void UpdateTimeIntervals(Int64 start, Int64 end);
        void UpdateShowInterval(bool newValue);
    }
}
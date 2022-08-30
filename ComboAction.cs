using System;

namespace LadeeViz
{
    internal class ComboAction
    {
        internal Action Func;
        internal string Name;

        public override string ToString()
        {
            return Name;
        }
    }
}
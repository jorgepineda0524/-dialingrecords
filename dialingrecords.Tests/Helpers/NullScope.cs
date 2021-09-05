using System;

namespace dialingrecords.Tests.Helpers
{
    public class NullScope : IDisposable
    {
        public static NullScope Instance { get; } = new NullScope();
        public void Dispose() { }

        private NullScope() { }
    }
}
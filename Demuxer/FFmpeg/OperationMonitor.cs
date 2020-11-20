using System;
using System.Threading;

namespace Demuxer.FFmpeg
{
    public class OperationMonitor
    {
        private CancellationToken _token = CancellationToken.None;
        private bool _isRunning = false;
        private int _startTicks = int.MaxValue;
        private const int TickTimeOut = 1000 * 5; // timeout in ms. 5s.

        public void Start()
        {
            _token = CancellationToken.None;
            _startTicks = Environment.TickCount;
            _isRunning = true;
        }

        public void Start(CancellationToken token, bool noTimeout=false)
        {
            _token = token;
            _startTicks = noTimeout?int.MaxValue:Environment.TickCount;
            _isRunning = true;
        }

        public void End()
        {
            _isRunning = false;
            var currentToken = _token;
            bool throwTimeout = Environment.TickCount - _startTicks > TickTimeOut;
            _token = CancellationToken.None;
            _startTicks = int.MaxValue;

            currentToken.ThrowIfCancellationRequested();
            if (throwTimeout)
                throw new TimeoutException("Demuxer operation timeout");
        }

        public int CancellOrTimeout()
        {
            // Does not matter what value is returned. > 0 result in EXIT code being retured.
            return _isRunning && (_token.IsCancellationRequested || Environment.TickCount - _startTicks > TickTimeOut ) ? 1 : 0;
        }

        public static bool IsAborted(int result)
        {
            // -541478725  'EOF ' code = ~' FOE'
            // -1414092869 'EXIT' code = ~'TIXE'
            return result == -1414092869 || result == -541478725;
        }
    }
}

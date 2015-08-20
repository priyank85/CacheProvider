using System;
using System.Threading;
using Provider.Exceptions;

namespace Provider
{
    public class Lock : IDisposable
    {
        private int _locked;
        private readonly int _timeout;
        private readonly object _lock;

        public static Lock GetLock(object o)
        {
            return GetLock(o, 15000, 1);
        }

        public static Lock GetLock(object o, int theTimeout)
        {
            return GetLock(o, theTimeout, 1);
        }

        public static Lock GetLock(object o, int theTimeout, int theFrame)
        {
            // try the monitor
            if (!Monitor.TryEnter(o, theTimeout))
                throw new TimeoutException();

            try
            {
                // for debugging
                return new Lock(o, theTimeout);
            }
            catch
            {
                // we make very sure the monitor is exited if a problem occurred
                Monitor.Exit(o);

                // and we rethrow the error
                throw;
            }
        }

        private Lock(object o, int theTimeout)
        {
            // by this point the lock is held
            _lock = o;
            _locked = 1;
            _timeout = theTimeout;
        }

        public void Unlock()
        {
            // compare the locked value to ONE and if the lock is held change to ZERO
            if (Interlocked.CompareExchange(ref _locked, 0, 1) == 0)
                throw new UnlockCallWithoutHeldLock();

            try
            {
                Monitor.Exit(_lock);
            }
            catch (Exception)
            {
                // we threw an exception during the EXIT, so reset the locked to 1 to indicate that we should still have the lock
                Interlocked.Exchange(ref _locked, 1);
                throw;
            }
        }

        public void Relock()
        {
            // try to renter the lock
            if (Interlocked.CompareExchange(ref _locked, 1, 0) == 0)
            {
                if (!Monitor.TryEnter(_lock, _timeout))
                {
                    // failed to relock, so reset our locked variable
                    Interlocked.Exchange(ref _locked, 0);
                    throw new TimeoutException();
                }
            }
        }

        public void Dispose()
        {
            // lock is held at this point
            if (Interlocked.Exchange(ref _locked, 0) == 1)
            {
                // release the monitor lock, and clear the variable so that we don't exit it again
                Monitor.Exit(_lock);
            }
        }
    }
}

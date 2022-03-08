using System;

namespace DFSyclusStatusLogger.Helper
{
    public sealed class LockHolder<T> : IDisposable where T : class
	{
		private readonly T handle;
		private bool holdsLock;

		public LockHolder(T handle, int milliSecondTimeout)
		{
			this.handle = handle;
			holdsLock = System.Threading.Monitor.TryEnter(handle, milliSecondTimeout);
		}

		public bool LockSuccessful
		{
			get { return holdsLock; }
		}

		#region IDisposable Members
		public void Dispose()
		{
			if (holdsLock)
			{
				System.Threading.Monitor.Exit(handle);
			}
			// Don’t unlock twice
			holdsLock = false;
		}
		#endregion
	}
}

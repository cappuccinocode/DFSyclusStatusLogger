using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DFSyclusStatusLogger.Threading
{
	class ThreadHolder
	{
		public Thread thread { get; set; }
		public ThisThread thisThread { get; set; }
		public bool resuming { get; set; }

		public ThreadHolder()
		{
			resuming = false;
		}

		public void Process()
		{
			thisThread.Process(resuming);
		}
	}
}

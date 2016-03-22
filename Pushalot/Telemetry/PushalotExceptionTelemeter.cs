using System;
using TIKSN.Analytics.Telemetry;

namespace TIKSN.Pushalot.Analytics.Telemetry
{
	public class PushalotExceptionTelemeter : TelemeterBase, IExceptionTelemeter
	{
		public void TrackException(Exception exception)
		{
			throw new NotImplementedException();
		}
	}
}

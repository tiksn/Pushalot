using System;
using TIKSN.Analytics.Telemetry;

namespace TIKSN.Pushalot.Analytics.Telemetry
{
	public class PushalotEventTelemeter : TelemeterBase, IEventTelemeter
	{
		public void TrackEvent(string name)
		{
			throw new NotImplementedException();
		}
	}
}

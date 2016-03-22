using System;
using TIKSN.Analytics.Telemetry;

namespace TIKSN.Pushalot.Analytics.Telemetry
{
	public class PushalotMetricTelemeter : TelemeterBase, IMetricTelemeter
	{
		public void TrackMetric(string metricName, double metricValue)
		{
			throw new NotImplementedException();
		}
	}
}

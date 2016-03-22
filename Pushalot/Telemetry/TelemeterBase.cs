using System;
using System.Collections.Generic;

namespace TIKSN.Pushalot.Analytics.Telemetry
{
	public abstract class TelemeterBase
	{
		protected readonly PushalotClient client;
		protected readonly IPushalotConfiguration pushalotConfiguration;

		public TelemeterBase(IPushalotConfiguration pushalotConfiguration)
		{
			var authorizationTokens = GetAuthorizationTokens(pushalotConfiguration);

			foreach (var authorizationToken in authorizationTokens)
			{
			}
		}

		protected abstract IEnumerable<string> GetAuthorizationTokens(IPushalotConfiguration pushalotConfiguration);

		protected void SendMessage(IEnumerable<string> authorizationTokens, string telemetryName, string content)
		{
			var mbuilder = new MessageBuilder();
			mbuilder.MessageLinkTitle = telemetryName;
			mbuilder.MessageBody = content;

			var message = mbuilder.Build();


		}
	}
}

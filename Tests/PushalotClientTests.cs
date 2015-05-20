
namespace TIKSN.Pushalot.Tests
{
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
    public class PushalotClientTests
    {
        //[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        //[Microsoft.VisualStudio.TestTools.UnitTesting.ExpectedException(typeof(System.ArgumentNullException))]
        //public void PushalotClient_WithNullToken()
        //{
        //    var client = new PushalotClient(null);
        //}

        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public async System.Threading.Tasks.Task SendMessage_001()
        {
            var token = new AuthorizationToken(Properties.Settings.Default.APIKey);

            var client = new PushalotClient();

            client.Subscribe(token);

            var message = new Message("Unit Test Message Title", "Unit Test Message Body", null, false, true, null, "Unit Test Source", null);

            await client.SendMessage(message);
        }

        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public async System.Threading.Tasks.Task SendMessage_Minimal()
        {
            var token = new AuthorizationToken(Properties.Settings.Default.APIKey);

            var client = new PushalotClient();

            client.Subscribe(token);

            var message = new Message(null, "Unit Test Message Body", null, false, true, null, null, null);

            await client.SendMessage(message);
        }
    }
}

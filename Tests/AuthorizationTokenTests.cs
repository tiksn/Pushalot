
namespace TIKSN.Pushalot.Tests
{
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
    public class AuthorizationTokenTests
    {
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        [Microsoft.VisualStudio.TestTools.UnitTesting.ExpectedException(typeof(System.ArgumentException))]
        public void AuthorizationToken_WithInvalidLength()
        {
            var token = new AuthorizationToken("123");
        }
    }
}

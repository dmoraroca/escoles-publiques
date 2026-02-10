using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Moq;
using Web.Hubs;
using Xunit;

namespace UnitTest.Hubs
{
    public class SchoolHubTests
    {
        [Fact]
        public async Task BroadcastSchoolCreated_SendsMessage()
        {
            var clients = new Mock<IHubCallerClients>();
            var proxy = new Mock<IClientProxy>();
            clients.Setup(c => c.All).Returns(proxy.Object);

            var hub = new SchoolHub { Clients = clients.Object };
            await hub.BroadcastSchoolCreated("C1", "Name", "City");

            proxy.Verify(p => p.SendCoreAsync("SchoolCreated",
                It.Is<object[]>(o => (string)o[0] == "C1" && (string)o[1] == "Name" && (string)o[2] == "City"),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task BroadcastSchoolUpdated_SendsMessage()
        {
            var clients = new Mock<IHubCallerClients>();
            var proxy = new Mock<IClientProxy>();
            clients.Setup(c => c.All).Returns(proxy.Object);

            var hub = new SchoolHub { Clients = clients.Object };
            await hub.BroadcastSchoolUpdated("C1", "Name", "City");

            proxy.Verify(p => p.SendCoreAsync("SchoolUpdated",
                It.Is<object[]>(o => (string)o[0] == "C1"),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task BroadcastSchoolDeleted_SendsMessage()
        {
            var clients = new Mock<IHubCallerClients>();
            var proxy = new Mock<IClientProxy>();
            clients.Setup(c => c.All).Returns(proxy.Object);

            var hub = new SchoolHub { Clients = clients.Object };
            await hub.BroadcastSchoolDeleted("C1");

            proxy.Verify(p => p.SendCoreAsync("SchoolDeleted",
                It.Is<object[]>(o => (string)o[0] == "C1"),
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}

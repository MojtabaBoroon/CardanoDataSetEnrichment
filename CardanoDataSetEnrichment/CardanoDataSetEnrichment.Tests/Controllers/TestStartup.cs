using CardanoDataSetEnrichment.Infrastructure.ExternalServices.Abstractions;
using CardanoDataSetEnrichment.Infrastructure.FileReader.Abstractios;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;

namespace CardanoDataSetEnrichment.Tests.Controllers
{
    public class TestStartup<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        public Mock<ITransactionParser> transactionParserMock { get; private set; }
        public Mock<IGLeifApiClient> gLeifApiClientMock { get; private set; }

        public TestStartup()
        {
            transactionParserMock = new Mock<ITransactionParser>();
            gLeifApiClientMock = new Mock<IGLeifApiClient>();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<ITransactionParser>();
                services.RemoveAll<IGLeifApiClient>();

                services.AddSingleton(transactionParserMock.Object);
                services.AddSingleton(gLeifApiClientMock.Object);
            });
        }
    }
}

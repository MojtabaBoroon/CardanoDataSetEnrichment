using CardanoDataSetEnrichment.Domain.LeiRecordDtos;
using CardanoDataSetEnrichment.Domain.Transactions;
using CardanoDataSetEnrichment.Infrastructure.ExternalServices.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Text;
using System.Net;
using Newtonsoft.Json;
using CardanoDataSetEnrichment.Infrastructure.FileReader.Abstractios;

namespace CardanoDataSetEnrichment.Tests.Controllers
{
    public class LeiRecordsControllerTests : IClassFixture<TestStartup<Program>>
    {
        private readonly TestStartup<Program> _factory;
        private Mock<ITransactionParser> _transactionParserMock;
        private Mock<IGLeifApiClient> _gLeifApiClientMock;
        private readonly HttpClient _client;

        private ICollection<Transaction> _transactions;
        private LeiRecordDto _leiRecord;


        public LeiRecordsControllerTests(TestStartup<Program> factory)
        {
            _factory = factory;
            _transactionParserMock = new Mock<ITransactionParser>();
            _gLeifApiClientMock = new Mock<IGLeifApiClient>();

            _client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddSingleton(_transactionParserMock.Object);
                    services.AddSingleton(_gLeifApiClientMock.Object);
                });
            }).CreateClient();

            _transactions = CreateTransactions();
            _leiRecord = CreateLeiRecord();

        }

        private LeiRecordDto CreateLeiRecord()
        {
            return new LeiRecordDto
            {
                Data = new List<DataDto>
                {
                    new DataDto
                    {
                        Attributes = new AttributesDto
                        {
                            Bic = {"bic1", "bic2"},
                            Entity = new EntityDto
                            {
                                LegalAddress = new LegalAddressDto
                                {
                                    Country = "Country"
                                },
                                LegalName = new LegalNameDto
                                {
                                    Language = "Language",
                                    Name = "Name"
                                }
                            }
                        }
                    }
                }
            };
        }

        private ICollection<Transaction> CreateTransactions()
        {
            return new List<Transaction>
            {
                new Transaction
                {
                    LEI = "LEI1",
                    Notional = 10,
                    Rate = 0.5
                }
            };
        }

        [Fact]
        public async Task GetEnrichedTransactions_ApiIsCalled_GLeifApiClientIsCalled()
        {
            // Arrang
            _gLeifApiClientMock.Setup(x => x.GetTransactioByLeiAsync(It.IsAny<string>())).ReturnsAsync(_leiRecord);
            _transactionParserMock.Setup(x => x.Parse()).Returns(_transactions);
            var requestBody = new StringContent("", Encoding.UTF8, "application/json");

            // Act
            var response = await _client.GetAsync("/api/LeiRecords");

            //Assert
            _gLeifApiClientMock.Verify(x => x.GetTransactioByLeiAsync(It.IsAny<string>()), Times.Once());
        }

        [Fact]
        public async Task GetEnrichedTransactions_InternalApiIsCalled_TransactionParserIsCalled()
        {
            // Arrang
            _gLeifApiClientMock.Setup(x => x.GetTransactioByLeiAsync(It.IsAny<string>())).ReturnsAsync(_leiRecord);
            _transactionParserMock.Setup(x => x.Parse()).Returns(_transactions);
            var requestBody = new StringContent("", Encoding.UTF8, "application/json");

            // Act
            var response = await _client.GetAsync("/api/LeiRecords");

            //Assert
            _transactionParserMock.Verify(x => x.Parse(), Times.Once());
        }

        [Fact]
        public async Task GetEnrichedTransactions_InputIsValid_TransactionsIsReturned()
        {
            // Arrang
            _gLeifApiClientMock.Setup(x => x.GetTransactioByLeiAsync(It.IsAny<string>())).ReturnsAsync(_leiRecord);
            _transactionParserMock.Setup(x => x.Parse()).Returns(_transactions);
            var requestBody = new StringContent("", Encoding.UTF8, "application/json");

            // Act
            var response = await _client.GetAsync("/api/LeiRecords");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<List<Transaction>>(responseContent);
            Assert.Equal(1, result.Count);
            Assert.Equal(_leiRecord.Data.FirstOrDefault().Attributes.Bic.FirstOrDefault(), result.FirstOrDefault().Bic.FirstOrDefault());
            Assert.Equal(_leiRecord.Data.FirstOrDefault().Attributes.Entity.LegalName.Name, result.FirstOrDefault().LegalName.Name);
            Assert.Equal(_leiRecord.Data.FirstOrDefault().Attributes.Entity.LegalName.Language, result.FirstOrDefault().LegalName.Language);

        }

        [Theory]
        [InlineData("GB", -5)]
        [InlineData("NL", 10)]
        public async Task GetEnrichedTransactions_CountryIsSpecified_TransactionCostsIsReturned(string country, int transactionCosts)
        {
            // Arrang
            _leiRecord.Data.FirstOrDefault().Attributes.Entity.LegalAddress.Country = country;

            _gLeifApiClientMock.Setup(x => x.GetTransactioByLeiAsync(It.IsAny<string>())).ReturnsAsync(_leiRecord);
            _transactionParserMock.Setup(x => x.Parse()).Returns(_transactions);
            var requestBody = new StringContent("", Encoding.UTF8, "application/json");

            // Act
            var response = await _client.GetAsync("/api/LeiRecords");

            //Assert
            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<List<Transaction>>(responseContent);
            Assert.Equal(transactionCosts, result.FirstOrDefault().TransactionCosts);
        }
    }
}

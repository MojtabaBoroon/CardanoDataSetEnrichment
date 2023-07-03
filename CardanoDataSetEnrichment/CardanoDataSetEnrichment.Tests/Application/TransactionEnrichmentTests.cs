using CardanoDataSetEnrichment.Application;
using CardanoDataSetEnrichment.Domain.LeiRecordDtos;
using CardanoDataSetEnrichment.Domain.Transactions;
using CardanoDataSetEnrichment.Infrastructure.ExternalServices.Abstractions;
using CardanoDataSetEnrichment.Infrastructure.FileReader.Abstractios;
using Moq;

namespace CardanoDataSetEnrichment.Tests.Application
{
    public class TransactionEnrichmentTests
    {
        private Mock<IGLeifApiClient> _gLeifApiClientMock;
        private Mock<ITransactionParser> _transactionParserMock;

        private LeiRecordDto _leiRecord;
        private ICollection<Transaction> _transactions;
        public TransactionEnrichmentTests()
        {
            _gLeifApiClientMock = new Mock<IGLeifApiClient>();
            _transactionParserMock = new Mock<ITransactionParser>();
            _leiRecord = CreateLeiRecord();
            _transactions = CreateTransactions();
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
        public async Task GetEntichedTransactiosAsync_InputIsValid_GLeifApiClientIsCalled()
        {
            // Arrange
            var transactionEnrichment = new TransactionEnrichment(_gLeifApiClientMock.Object, _transactionParserMock.Object);

            _gLeifApiClientMock.Setup(x => x.GetTransactioByLeiAsync(It.IsAny<string>())).ReturnsAsync(_leiRecord);
            _transactionParserMock.Setup(x => x.Parse()).Returns(_transactions);

            // Act
            var result = await transactionEnrichment.GetEntichedTransactiosAsync();

            // Assert
            _gLeifApiClientMock.Verify(x => x.GetTransactioByLeiAsync(It.IsAny<string>()), Times.Once());
        }

        [Fact]
        public async Task GetEntichedTransactiosAsync_InputIsValid_TransactionParserIsCalled()
        {
            // Arrange
            var transactionEnrichment = new TransactionEnrichment(_gLeifApiClientMock.Object, _transactionParserMock.Object);

            _gLeifApiClientMock.Setup(x => x.GetTransactioByLeiAsync(It.IsAny<string>())).ReturnsAsync(_leiRecord);
            _transactionParserMock.Setup(x => x.Parse()).Returns(_transactions);

            // Act
            var result = await transactionEnrichment.GetEntichedTransactiosAsync();

            // Assert
            _transactionParserMock.Verify(x => x.Parse(), Times.Once());
        }
    }
}

using CardanoDataSetEnrichment.Domain.LeiRecordDtos;

namespace CardanoDataSetEnrichment.Infrastructure.ExternalServices.Abstractions
{
    public interface IGLeifApiClient
    {
        Task<LeiRecordDto> GetTransactioByLeiAsync(string Lei);

    }
}

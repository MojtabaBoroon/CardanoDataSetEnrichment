using CardanoDataSetEnrichment.Domain.LeiRecordDtos;
using CardanoDataSetEnrichment.Infrastructure.ExternalServices.Abstractions;
using Newtonsoft.Json;

namespace CardanoDataSetEnrichment.Infrastructure.ExternalServices
{
    public class GLeifApiClient : IGLeifApiClient
    {
        private static readonly HttpClient _client = new HttpClient();

        public async Task<LeiRecordDto> GetTransactioByLeiAsync(string Lei)
        {
            string apiUrl = $"https://api.gleif.org/api/v1/lei-records?filter[lei]={Lei}";

            var response = await _client.GetAsync(apiUrl);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<LeiRecordDto>(responseContent);

                return result;
            }
            else
            {
                throw new Exception($"Failed to get LeiRecord: {response.StatusCode} {response.ReasonPhrase}");
            }
        }
    }
}

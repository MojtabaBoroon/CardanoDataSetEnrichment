namespace CardanoDataSetEnrichment.Domain.LeiRecordDtos
{
    public class AttributesDto
    {
        public EntityDto Entity { get; set; }
        public ICollection<string> Bic { get; set; } = new List<string>();
    }
}
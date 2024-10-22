namespace CommandService.Dtos
{
    public class PlatformPublishedDto
    {
        public int PlatformId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Event { get; set; } = string.Empty;
    }
}
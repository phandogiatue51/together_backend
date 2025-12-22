namespace Together.DTOs.App
{
    public class UpdateAppDto
    {
        public string? RelevantExperience { get; set; }
        public List<int> SelectedCertificateIds { get; set; } = new();
    }
}

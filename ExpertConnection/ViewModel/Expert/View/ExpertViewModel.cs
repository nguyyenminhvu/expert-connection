using ViewModel.CategoryMapping.View;

namespace ViewModel.Expert
{
    public class ExpertViewModel
    {
        public string Id { get; set; } = null!;

        public string Fullname { get; set; } = null!;

        public string CertificateLink { get; set; } = null!;

        public string Introduction { get; set; } = null!;

        public double SummaryRating { get; set; }

        public string WorkRole { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Phone { get; set; } = null!;
        public virtual ICollection<CategoryMappingViewModel> CategoryMappings { get; set; } = new List<CategoryMappingViewModel>();
    }
}

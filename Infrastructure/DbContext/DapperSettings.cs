namespace Infrastructure.DbContext
{
    public class DapperSettings
    {
        public const string SectionName = "ConnectionStrings";

        public string SqlServer { get; set; } = null!;
    }
}

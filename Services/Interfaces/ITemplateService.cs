namespace JobMasterApi.Services.Interfaces
{
    public interface ITemplateService
    {
        Task<string> GetTemplateAsync(string templateName);
        Task<string> RenderTemplateAsync(string templateName, Dictionary<string, string> replacements);
    }

}
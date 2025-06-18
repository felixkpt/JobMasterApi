using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using JobMasterApi.Services.Interfaces;

namespace JobMasterApi.Services
{

    public class TemplateService : ITemplateService
    {
        private readonly IWebHostEnvironment _env;

        public TemplateService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string> GetTemplateAsync(string templateName)
        {
            var templatePath = Path.Combine(_env.ContentRootPath, "Templates", templateName);
            if (!File.Exists(templatePath))
            {
                throw new FileNotFoundException($"Template file not found: {templatePath}");
            }
            
            return await File.ReadAllTextAsync(templatePath);
        }

        public async Task<string> RenderTemplateAsync(string templateName, Dictionary<string, string> replacements)
        {
            var template = await GetTemplateAsync(templateName);
            
            foreach (var replacement in replacements)
            {
                template = template.Replace($"{{{{{replacement.Key}}}}}", replacement.Value);
            }
            
            return template;
        }
    }
}
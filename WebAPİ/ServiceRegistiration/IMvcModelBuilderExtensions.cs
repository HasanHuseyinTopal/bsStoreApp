using WebAPİ.Utulities.Formatters;

namespace WebAPİ.ServiceRegistiration
{
    public static class IMvcModelBuilderExtensions
    {
        public static IMvcBuilder AddCustomCsvFormatter(this IMvcBuilder builder)
        {
           return builder.AddMvcOptions(config =>
            {
                config.OutputFormatters.Add(new CsvOutPutFormatter());
            });
        }
    }
}

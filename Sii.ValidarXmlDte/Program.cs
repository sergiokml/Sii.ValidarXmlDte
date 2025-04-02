using System.Text;
using System.Xml;
using System.Xml.Schema;
using Sii.ValidarXmlDte.Helper;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
WebApplication app = builder.Build();

app.UseHttpsRedirection();

app.MapPost("/api/dte/validar", async (HttpRequest request) =>
{
    try
    {
        using StreamReader reader = new(request.Body, Encoding.GetEncoding("ISO-8859-1"));
        string xmlContent = await reader.ReadToEndAsync();

        XmlDocument xml = new();
        xml.LoadXml(xmlContent);

        XmlNodeList dtes = xml.GetElementsByTagName("DTE");

        if (dtes.Count == 0)
            return Results.BadRequest("No se encontraron elementos DTE en el XML");

        List<object> result = [];

        foreach (XmlNode dteNode in dtes)
        {
            XmlDocument dteDoc = new();
            dteDoc.LoadXml(dteNode.OuterXml);

            ValidateXsd validator = new();
            validator.ValidatingSchemas(dteDoc);

            result.Add(new
            {
                id = dteDoc.DocumentElement?.GetElementsByTagName("Documento")?[0]?.Attributes?["ID"]?.Value,
                errors = validator.ValidateMsg
                    .Where(x => x.Item1 == XmlSeverityType.Error)
                    .Select(x => x.Item2)
                    .ToList(),
                warnings = validator.ValidateMsg
                    .Where(x => x.Item1 == XmlSeverityType.Warning)
                    .Select(x => x.Item2)
                    .ToList()
            });
        }

        return Results.Ok(result);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

await app.RunAsync();

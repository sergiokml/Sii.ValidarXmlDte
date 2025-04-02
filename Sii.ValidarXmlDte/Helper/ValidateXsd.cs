using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Sii.ValidarXmlDte.Helper;

public class ValidateXsd
{
    public List<Tuple<XmlSeverityType, string>> ValidateMsg = [];
    private const string np = "http://www.sii.cl/SiiDte";
    private const string nps = "http://www.w3.org/2000/09/xmldsig#";

    public void ValidatingSchemas(XmlDocument signedXml)
    {
        ValidateMsg.Clear();

        XmlReaderSettings schemaSettings = GetSchemaSettings(signedXml);
        schemaSettings.ValidationEventHandler += ValidationEventHandler!;

        try
        {
            using XmlReader reader = XmlReader.Create(new XmlNodeReader(signedXml), schemaSettings);
            while (reader.Read()) { }
        }
        finally
        {
            schemaSettings.ValidationEventHandler -= ValidationEventHandler!;
        }
    }

    public static T OnDeserializing<T>(XDocument xmlDoc) where T : class
    {
        switch (xmlDoc?.Root?.Name.LocalName)
        {
            case "EnvioDTE":
                return DeserializeXDoc<T>(xmlDoc);
            case "DTE":
                return DeserializeXDoc<T>(xmlDoc, new XmlRootAttribute("DTE") { Namespace = np });
            case "EnvioBOLETA":
                return DeserializeXDoc<T>(xmlDoc);
            case "RespuestaDTE":
                return DeserializeXDoc<T>(xmlDoc);
            case "RESULTADO_ENVIO":
                return DeserializeXDoc<T>(xmlDoc);
            case "EnvioRecibos":
                return DeserializeXDoc<T>(xmlDoc);
            case "RECEPCIONDTE":
                return DeserializeXDoc<T>(xmlDoc);
            default:
                break;
        }
        return default!;
    }

    private static XmlReaderSettings GetSchemaSettings(XmlDocument xmlDocument)
    {
        XmlReaderSettings settings = new() { ValidationType = ValidationType.Schema };

        string basePath = Path.Combine(AppContext.BaseDirectory, "Schemas");

        switch (xmlDocument?.DocumentElement?.LocalName)
        {
            case "EnvioDTE":
                AddSchemas(
                    settings,
                    (np, Path.Combine(basePath, "EnvioDTE_v10.xsd")),
                    (np, Path.Combine(basePath, "DTE_v10.xsd")),
                    (np, Path.Combine(basePath, "SiiTypes_v10.xsd")),
                    (nps, Path.Combine(basePath, "xmldsignature_v10.xsd"))
                );
                break;
            case "DTE":
                AddSchemas(
                    settings,
                    (np, Path.Combine(basePath, "DTE_v10.xsd")),
                    (np, Path.Combine(basePath, "SiiTypes_v10.xsd")),
                    (nps, Path.Combine(basePath, "xmldsignature_v10.xsd"))
                );
                break;
            case "EnvioBOLETA":
                AddSchemas(
                    settings,
                    (np, Path.Combine(basePath, "EnvioBOLETA_v11.xsd")),
                    (nps, Path.Combine(basePath, "xmldsignature_v10.xsd"))
                );
                break;
            case "RespuestaDTE":
                AddSchemas(
                    settings,
                    (np, Path.Combine(basePath, "RespuestaEnvioDTE_v10.xsd")),
                    (np, Path.Combine(basePath, "SiiTypes_v10.xsd")),
                    (nps, Path.Combine(basePath, "xmldsignature_v10.xsd"))
                );
                break;
            case "RESULTADO_ENVIO":
                AddSchemas(settings, (np, Path.Combine(basePath, "RespSII_v10.xsd")));
                break;
            case "EnvioRecibos":
                AddSchemas(
                    settings,
                    (np, Path.Combine(basePath, "EnvioRecibos_v10.xsd")),
                    (np, Path.Combine(basePath, "Recibos_v10.xsd")),
                    (np, Path.Combine(basePath, "SiiTypes_v10.xsd")),
                    (nps, Path.Combine(basePath, "xmldsignature_v10.xsd"))
                );
                break;
            default:
                throw new Exception($"No schema found for: '{xmlDocument?.DocumentElement?.LocalName}'");
        }

        return settings;
    }

    private static void AddSchemas(XmlReaderSettings settings, params (string Namespace, string Path)[] schemas)
    {
        foreach ((string ns, string path) in schemas)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"XSD file not found: {path}");
            settings.Schemas.Add(ns, XmlReader.Create(path));
        }
    }

    private void ValidationEventHandler(object sender, ValidationEventArgs args)
    {
        ValidateMsg.Add(new Tuple<XmlSeverityType, string>(args.Severity, args.Message));
    }

    private static T DeserializeXDoc<T>(XDocument sdoc) where T : class
    {
        using MemoryStream ms = new();
        sdoc.Save(ms);
        ms.Seek(0, SeekOrigin.Begin);
        XmlSerializer xmlSerializer = new(typeof(T));
        return (T)xmlSerializer.Deserialize(ms)!;
    }

    private static T DeserializeXDoc<T>(XDocument sdoc, XmlRootAttribute xRoot) where T : class
    {
        using MemoryStream ms = new();
        sdoc.Save(ms);
        ms.Seek(0, SeekOrigin.Begin);
        XmlSerializer xmlSerializer = new(typeof(T), xRoot);
        return (T)xmlSerializer.Deserialize(ms)!;
    }
}

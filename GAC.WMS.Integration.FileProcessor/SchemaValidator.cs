using System.Xml;
using System.Xml.Schema;

namespace GAC.WMS.Integration.FileProcessor
{
    public static class SchemaValidator
    {
        public static void ValidateXml(string xmlPath, string schemaPath)
        {
            var settings = new XmlReaderSettings
            {
                ValidationType = ValidationType.Schema
            };

            // Load the schema
            settings.Schemas.Add(null, schemaPath);

            // Set up validation event handler
            settings.ValidationEventHandler += (sender, args) =>
            {
                throw new XmlSchemaValidationException(
                    $"XML validation error: {args.Message} at line {args.Exception.LineNumber}, position {args.Exception.LinePosition}");
            };

            // Validate the XML
            using (var reader = XmlReader.Create(xmlPath, settings))
            {
                while (reader.Read()) { }
            }
        }

        public static string GetSchemaPath(string rootElementName, string baseSchemaPath)
        {
            return rootElementName switch
            {
                "Customer" => Path.Combine(baseSchemaPath, "Customer.xsd"),
                "Product" => Path.Combine(baseSchemaPath, "Product.xsd"),
                "PurchaseOrder" => Path.Combine(baseSchemaPath, "PurchaseOrder.xsd"),
                "SalesOrder" => Path.Combine(baseSchemaPath, "SalesOrder.xsd"),
                _ => throw new FileNotFoundException($"No schema found for {rootElementName}")
            };
        }
    }
}

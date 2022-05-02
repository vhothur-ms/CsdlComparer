
using System.Collections.Generic;
using System.Xml;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Csdl;

namespace CsdlComparer
{
    public class EdmModelReader
    {
        public IEdmModel Get(string filepath)
        {
            return ReadModel(filepath);

            /*
            //EdmItemCollection collection = new EdmItemCollection(filepath);

            StringReader metadataReader;
            using (var sr = new StreamReader(filepath))
            {
                // Read the stream as a string, and write the string to the console.
                metadataReader = new StringReader(sr.ReadToEnd());
            }

            XmlReaderSettings xmlSettings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Prohibit,
                XmlResolver = null
            };

            XmlReader reader = XmlReader.Create(metadataReader, xmlSettings);

            List<string> skipList = new List<string>() { "edmx:Edmx", "edmx:DataServices" };

            while(reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "edmx:Edmx")
                    {
                        reader.MoveToContent();
                        reader.ReadToDescendant("edmx:DataServices");
                    }
                    if (reader.Name == "edmx:DataServices")
                    {
                        reader.MoveToContent();
                        reader.ReadToDescendant("Schema");
                        break;
                    }
                }
            }
            XmlReader schema = reader.ReadSubtree();

            */

            //EdmxReader doesn't seem to work with the latest version of .NET
            //bool success = EdmxReader.TryParse(schema, out IEdmModel edmModel, out IEnumerable<EdmValidationError> edmErrors);

            /*
            EdmItemCollection collection = new EdmItemCollection(new List<XmlReader> { schema });
            MetadataWorkspace workspace = new MetadataWorkspace();
            workspace.RegisterItemCollection(collection);
            EdmType contentType;
            workspace.TryGetType("Person", "SchoolModel", DataSpace.CSpace, out contentType);
            if (contentType.MetadataProperties.Contains("http://CustomNamespace.com:CustomAttribute"))
            {
                MetadataProperty annotationProperty =
                    contentType.MetadataProperties["http://CustomNamespace.com:CustomAttribute"];
                object annotationValue = annotationProperty.Value;
                Console.WriteLine(annotationValue.ToString());
            }

            // Doing this outside of a using as FxCop always complains about DtdProcessing otherwise.
            metadataReader.Dispose();
            reader.Dispose();
            return null;
            */
        }

        private static IEdmModel ReadModel(string fileName)
        {
            using (var reader = XmlReader.Create(fileName))
            {
                IEdmModel model;
                IEnumerable<EdmError> errors;
                if (CsdlReader.TryParse(reader, out model, out errors))
                {
                    return model;
                }
                return null;
            }
        }
    }
}

using System.Text;
using System.Text.Json;
using System.Xml;
using System.Xml.Linq;
using UndertaleModLib;

namespace gmeditdialectgen;

public class Program
{
    public static void Main(string[] args)
    {
        var config = new ConfigFile() {
            parent = "gml2",
            name = "Will You Snail?",
            indexingMode = "directory",
            projectRegex = "^(.+?)\\.wys$",
            apiFiles = new string[] { "api.gml" },
            assetFiles = new string[] { "assets.gml" }
        };

        if (Directory.Exists("dialect"))
            Directory.Delete("dialect", true);
        Directory.CreateDirectory("dialect");

        var stream = File.OpenRead(args[0]);
        var data = UndertaleIO.Read(stream, Console.WriteLine, _ => { });
        stream.Dispose();

        var assets = new StringBuilder();
        var code = new StringBuilder();

        foreach (var obj in data.GameObjects)
        {
            assets.AppendLine(obj.Name.ToString());
        }
        foreach (var snd in data.Sounds)
        {
            assets.AppendLine(snd.Name.ToString());
        }
        foreach (var sprite in data.Sprites)
        {
            assets.AppendLine(sprite.Name.ToString());
        }
        foreach (var obj in data.GameObjects)
        {
            assets.AppendLine(obj.Name.ToString());
        }

        foreach (var codeEntry in data.Code)
        {
            string codeArgs = "";
            for (int i = 0; i < codeEntry.ArgumentsCount; i++)
            {
                codeArgs += $"argument{i}{(i != codeEntry.ArgumentsCount - 1 ? ", " : "")}";
            }

            code.AppendLine($"{codeEntry.Name.ToString()}({codeArgs})");
        }

        var fs = File.OpenRead(args[1]);
        XmlReader reader = XmlReader.Create(fs);
        XDocument doc = XDocument.Load(reader); 
        reader.Close();

        IEnumerable<XElement> functions = doc.Descendants("Functions").Elements("Function");

        foreach (var function in functions)
        {            
            string codeArgs = "";
            var parameters = function.Elements("Parameter");
            for (int i = 0; i < parameters.Count(); i++)
            {
                codeArgs += $"{parameters.ElementAt(i).Attribute("Name").Value}{(i != parameters.Count() - 1 ? ", " : "")}";
            }
            code.AppendLine($"{function.Attribute("Name").Value}({codeArgs})");
        }

        IEnumerable<XElement> constants = doc.Descendants("Constants").Elements("Constant");
        foreach (var constant in constants)
        {            
            code.AppendLine(constant.Attribute("Name").Value);
        }

        File.WriteAllText(Path.Combine("dialect", "api.gml"), code.ToString().Replace("gml_Script_", "").Replace("gml_GlobalScript_", "").Replace("gml_Object_", "").Replace("\"", ""));
        File.WriteAllText(Path.Combine("dialect", "assets.gml"), assets.ToString().Replace("\"", ""));
        var jso = new JsonSerializerOptions
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        File.WriteAllText(Path.Combine("dialect", "config.json"), JsonSerializer.Serialize(config, jso));
    }
}

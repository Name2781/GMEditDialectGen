public class ConfigFile
{
    public string parent { get; set; }
    public string name { get; set; }
    public string indexingMode { get; set; }
    public string projectRegex { get; set; }
    public string[] apiFiles { get; set; }
    public string[] assetFiles { get; set; }
}

/*
{
	"parent": "gml2",
	"name": "Cool Game™",
	
	"indexingMode": "local",
	"projectRegex": "^(.+?)\\.coolgame$",
	
	"apiFiles": ["api.gml"],
	"assetFiles": ["assets.gml"]
}
*/
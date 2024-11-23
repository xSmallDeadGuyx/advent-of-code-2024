using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace AoC2024InputContent;

[ContentImporter(".txt", DisplayName = "AoCInputImporter", DefaultProcessor = "AoCInputProcessor")]
public class AoCInputImporter : ContentImporter<string>
{
    public override string Import(string filename, ContentImporterContext context)
    {
        return File.ReadAllText(filename);
    }
}

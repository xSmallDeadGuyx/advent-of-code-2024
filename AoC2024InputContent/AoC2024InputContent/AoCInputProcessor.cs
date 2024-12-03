using Microsoft.Xna.Framework.Content.Pipeline;

namespace AoC2024InputContent;

[ContentProcessor(DisplayName = "AoCInputProcessor")]
class AoCInputProcessor : ContentProcessor<string, string>
{
    public override string Process(string input, ContentProcessorContext context)
    {
        return input;
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System.Diagnostics;

namespace AoC2024;

public class Day1 : DayBase
{
    public Day1(AoCGame game) : base(game) {}

    public override string GetName()
    {
        return "day1";
    }

    public override void Draw(GameTime gameTime, ref SpriteBatch spriteBatch)
    {
        spriteBatch.DrawString(_game.hack20, "Day 1 drawing! Input = " + _input, new Vector2(20, 20), Color.Black);
    }

    public override void Update(GameTime gameTime)
    {
        Debug.WriteLine("Input = " + _input);
    }
}
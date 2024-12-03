using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.ComponentModel;

namespace AoC2024;

public abstract class DayBase
{
    protected AoCGame _game;
    protected string _input;


    public DayBase(AoCGame game)
    {
        _game = game;
    }

    public abstract string GetName();

    public void SetInput(string input)
    {
        _input = input;
    }

    public virtual void Init() {}

    public virtual void LoadContent(ContentManager content) {}

    public abstract void Update(GameTime gameTime);
    public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDeviceManager graphics);

    protected string ReadNextInputLine()
    {
        if (_input.Length == 0)
        {
            return "";
        }

        string line;

        int index = _input.IndexOf("\n");
        if (index < 0)
        {
            line = _input;
            _input = "";
        }
        else
        {
            line = _input.Substring(0, index - 1);
            _input = _input.Substring(index + 1);
        }

        return line;
    }

    protected string[] SplitOnWhitespace(string str)
    {
        return str.Split(new char[0], System.StringSplitOptions.RemoveEmptyEntries);
    }
}

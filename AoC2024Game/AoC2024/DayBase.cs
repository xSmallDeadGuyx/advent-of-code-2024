using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.ComponentModel;
using System;
using System.IO;

namespace AoC2024;

public abstract class DayBase
{
    protected AoCGame _game;
    protected string _input;

    private StringReader _inputReader;


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

    protected string? ReadNextInputLine()
    {
        if (_input.Length == 0)
        {
            return "";
        }

        if (_inputReader == null)
        {
            _inputReader = new StringReader(_input);
        }

        return _inputReader.ReadLine();
    }

    protected string[] SplitOnWhitespace(string str)
    {
        return str.Split(new char[0], System.StringSplitOptions.RemoveEmptyEntries);
    }
}

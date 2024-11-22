using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AoC2024;

public class AoCGame : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private DayBase[] _days;

    private DayBase _activeDay = null;

    public SpriteFont arial12;
    public SpriteFont arial20;

    public AoCGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        _days = new DayBase[]
        {
            new Day1(this)
        };
    }

    protected override void Initialize()
    {
        foreach (DayBase day in _days)
        {
            day.Init();
        }

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        arial12 = Content.Load<SpriteFont>("Arial12");
        arial20 = Content.Load<SpriteFont>("Arial20");

        foreach (DayBase day in _days)
        {
            day.LoadContent(Content);
        }
    }

    private int IndexToAscii(int i)
    {
        return (int)'A' + i;
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            Exit();
            return;
        }

        if (_activeDay is null)
        {
            for(int i = 0; i < _days.Length; ++i)
            {
                if (Keyboard.GetState().IsKeyDown((Keys)IndexToAscii(i)))
                {
                    _activeDay = _days[i];
                    break;
                }
            }
        }
        else
        {
            _activeDay.Update(gameTime);
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();

        if (_activeDay is null)
        {
            for(int i = 0; i < _days.Length; ++i)
            {
                string letter = ((char)IndexToAscii(i)).ToString();
                _spriteBatch.DrawString(arial12, "Press " + letter + " to run day " + (i + 1), new Vector2(20, 20 + i * 14), Color.Black);
            }
        }
        else
        {
            _activeDay.Draw(gameTime, ref _spriteBatch);
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}

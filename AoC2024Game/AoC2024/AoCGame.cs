using AoC2024InputContent;
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

    public SpriteFont hack12;
    public SpriteFont hack20;

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

        hack12 = Content.Load<SpriteFont>("Hack12");
        hack20 = Content.Load<SpriteFont>("Hack20");

        foreach (DayBase day in _days)
        {
            day.SetInput(Content.Load<string>("Input\\" + day.GetName()));
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
                    _activeDay.Init();
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
                _spriteBatch.DrawString(hack12, "Press " + letter + " to run day " + _days[i].GetName(), new Vector2(20, 20 + i * 14), Color.Black);
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

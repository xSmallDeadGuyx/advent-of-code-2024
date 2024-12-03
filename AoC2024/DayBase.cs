using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace AoC2024;

public abstract class DayBase
{
    protected AoCGame _game;

    public DayBase(AoCGame game)
    {
        _game = game;
    }

    public virtual void Init() {}

    public virtual void LoadContent(ContentManager content) {}

    public abstract void Update(GameTime gameTime);
    public abstract void Draw(GameTime gameTime, ref SpriteBatch spriteBatch);
}

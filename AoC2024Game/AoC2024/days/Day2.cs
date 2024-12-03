using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;

namespace AoC2024;

public class Day2 : DayBase
{
    public Day2(AoCGame game) : base(game) {}

    public override string GetName()
    {
        return "day2";
    }

    private float _timeBetweenCalcs = 1;
    private float _currentTimeLeft = 0;

    private int[] _currentLine = new int[0];
    private int _currentMin;
    private int _currentMax;
    private bool _currentValid = false;

    private int _removedLevel = -1;

    private long _part1Sum = 0;
    private long _part2Sum = 0;

    private bool _finished = false;

    private int FindErrorCase(int[] line)
    {
        int lastVal = line[0];
        int overallDirection = 0;
        for (int j = 1; j < line.Length; ++j)
        {
            int val = line[j];

            if (val < _currentMin)
            {
                _currentMin = val;
            }

            if (val > _currentMax)
            {
                _currentMax = val;
            }

            int diff = val - lastVal;

            if (diff == 0 || Math.Abs(diff) > 3)
            {
                return j;
            }

            int currentDirection = Math.Sign(diff);
            if (j == 1)
            {
                overallDirection = currentDirection;
            }
            else if (overallDirection != currentDirection)
            {
                return j;
            }

            lastVal = val;
        }

        return -1;
    }

    public override void Update(GameTime gameTime)
    {
        if (_finished)
        {
            return;
        }

        _currentTimeLeft -= gameTime.GetElapsedSeconds();
        if (_currentTimeLeft < 0)
        {
            int extraIterations = Math.Max(0, Math.Min((int)(-_currentTimeLeft / _timeBetweenCalcs), 3));

            _timeBetweenCalcs *= 0.9f;
            _currentTimeLeft = _timeBetweenCalcs;

            for (int i = 0; i <= extraIterations; ++i)
            {
                string line = ReadNextInputLine();
                if (line == null)
                {
                    _finished = true;
                    return;
                }

                _currentLine = SplitOnWhitespace(line).Select(x => Int32.Parse(x)).ToArray();

                _currentMin = _currentLine.Min();
                _currentMax = _currentLine.Max();
                _removedLevel = -1;

                int error = FindErrorCase(_currentLine);

                if (error == -1)
                {
                    _currentValid = true;
                    _part1Sum++;
                    _part2Sum++;
                }
                else
                {
                    _currentValid = false;

                    // When an invalid level is found, the issue is in that level or the previous one.
                    List<int> possibleErrors = new List<int>{error, error - 1};

                    // Except for when the error is detected on the 3rd level, where the issue could be an invalid direction set by the initial 2 levels.
                    // In this case, the 2nd level is already added above so just need to add the 1st.
                    if (error == 2)
                    {
                       possibleErrors.Add(0);
                    }

                    foreach (int j in possibleErrors)
                    {
                        if (FindErrorCase(_currentLine.Select((x, idx) => (x, idx)).Where((x, idx) => idx != j).Select(x => x.x).ToArray()) == -1)
                        {
                            _removedLevel = j;
                            _currentValid = true;
                            _part2Sum++;
                            break;
                        }
                    }
                }
            }
        }
    }


    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDeviceManager graphics)
    {
        if (_currentLine.Length > 0)
        {
            float diff = _currentMax - _currentMin;
            float xInterval = (graphics.PreferredBackBufferWidth - 180) / (float)_currentLine.Length;
            float yScale = diff > 0 ? (graphics.PreferredBackBufferHeight - 80) / diff : 1;

            if (_removedLevel != -1)
            {
                float x = 20 + xInterval * (_removedLevel + 0.5f);
                float y = graphics.PreferredBackBufferHeight - 20 - (_currentLine[_removedLevel] - _currentMin) * yScale;
                ShapeExtensions.DrawCircle(spriteBatch, new CircleF(new Vector2(x, y), 10), 12, Color.Red, 4);
            }

            for (int i = 1; i < _currentLine.Length; ++i)
            {
                if (_removedLevel == i)
                {
                    continue;
                }

                int prevOffset = 1;

                if (_removedLevel == i - 1)
                {
                    if (i == 1)
                    {
                        continue;
                    }

                    prevOffset = 2;
                }

                float x1 = 20 + xInterval * (i + 0.5f - prevOffset);
                float x2 = 20 + xInterval * (i + 0.5f);
                float y1 = graphics.PreferredBackBufferHeight - 20 - (_currentLine[i - prevOffset] - _currentMin) * yScale;
                float y2 = graphics.PreferredBackBufferHeight - 20 - (_currentLine[i] - _currentMin) * yScale;

                ShapeExtensions.DrawLine(spriteBatch, x1, y1, x2, y2, prevOffset == 2 ? Color.Blue : Color.Black, 4);
            }

            bool part1Valid = _currentValid && _removedLevel == -1;
            bool part2Valid = _currentValid;
            spriteBatch.Draw(part1Valid ? _game.YesTexture : _game.NoTexture, new Rectangle(graphics.PreferredBackBufferWidth - 140, graphics.PreferredBackBufferHeight / 2 - 170, 120, 120), Color.White);
            spriteBatch.DrawString(_game.Hack20Font, "P1 = " + _part1Sum, new Vector2(graphics.PreferredBackBufferWidth - 140, graphics.PreferredBackBufferHeight / 2 - 50), Color.Black);
            spriteBatch.Draw(part2Valid ? _game.YesTexture : _game.NoTexture, new Rectangle(graphics.PreferredBackBufferWidth - 140, graphics.PreferredBackBufferHeight / 2 + 10, 120, 120), Color.White);
            spriteBatch.DrawString(_game.Hack20Font, "P2 = " + _part2Sum, new Vector2(graphics.PreferredBackBufferWidth - 140, graphics.PreferredBackBufferHeight / 2 + 140), Color.Black);
        }
    }
}
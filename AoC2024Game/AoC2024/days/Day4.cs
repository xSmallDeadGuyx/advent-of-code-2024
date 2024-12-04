using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;

namespace AoC2024;

public class Day4 : DayBase
{
    public Day4(AoCGame game) : base(game) {}

    public override string GetName()
    {
        return "day4";
    }

    private int _maxExtraIterations = 49;
    private float _timeBetweenTicks = 0.15f;
    private float _currentTimeLeft = 0.15f;

    private struct Xmas
    {
        public Vector2i Pos;
        public Vector2i Dir;

        public Xmas(Vector2i pos, Vector2i dir)
        {
            Pos = pos;
            Dir = dir;
        }
    }

    private string _part1Str = "XMAS";
    private int _columns;
    private int _rows;

    private string[] _inputLines;

    private Vector2 _textSize;

    private List<Xmas> _xmases = new List<Xmas>();
    private List<Vector2i> _masCrosses = new List<Vector2i>();

    private Vector2i _pos = Vector2i.Zero;

    private long _part1Sum = 0;
    private long _part2Sum = 0;

    private bool _finished = false;

    public override void Init()
    {
        _textSize = _game.Hack12Font.MeasureString(_input);

        _inputLines = _input.Split(new string[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);

        _rows = _inputLines.Length;
        _columns = _inputLines[0].Length;
    }

    private char? GetCharAt(Vector2i pos)
    {
        if (pos.Y >= 0 && pos.Y < _inputLines.Length)
        {
            if (pos.X >= 0 && pos.X < _inputLines[pos.Y].Length)
            {
                return _inputLines[pos.Y][pos.X];
            }
        }

        return null;
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
            int extraIterations = Math.Max(0, Math.Min((int)(-_currentTimeLeft / _timeBetweenTicks), _maxExtraIterations));

            if (_timeBetweenTicks > 1 / (60.0f * _maxExtraIterations))
            {
                _timeBetweenTicks *= 0.99f;
            }
            _currentTimeLeft = _timeBetweenTicks;

            for (int i = 0; i <= extraIterations; ++i)
            {
                if (GetCharAt(_pos) == _part1Str[0])
                {
                    foreach (Vector2i dir in Vector2i.AllDirections)
                    {
                        bool found = true;
                        for (int j = 1; j < _part1Str.Length; ++j)
                        {
                            if (GetCharAt(_pos + (dir * j)) != _part1Str[j])
                            {
                                found = false;
                                break;
                            }
                        }

                        if (found)
                        {
                            _part1Sum++;
                            _xmases.Add(new Xmas(_pos, dir));
                        }
                    }
                }
                else if (GetCharAt(_pos) == 'A')
                {
                    bool isCross = true;
                    foreach (Vector2i dir in new Vector2i[] {new Vector2i(-1, 1), new Vector2i(1, 1)})
                    {
                        char? prev = GetCharAt(_pos - dir);
                        char? next = GetCharAt(_pos + dir);

                        if (!(prev == 'M' && next == 'S') && !(prev == 'S' && next == 'M'))
                        {
                            isCross = false;
                            break;
                        }
                    }

                    if (isCross)
                    {
                        _part2Sum++;
                        _masCrosses.Add(_pos);
                    }
                }

                _pos.X++;
                if (_pos.X >= _columns)
                {
                    _pos.X = 0;
                    _pos.Y++;
                }

                if (_pos.Y >= _rows)
                {
                    _finished = true;
                    _pos = new Vector2i(_columns / 2, _rows / 2);
                    return;
                }
            }
        }
    }

    private Vector2 GetCharScreenPos(GraphicsDeviceManager graphics, Vector2i charPos, bool middleOfChar = true)
    {
        float offset = middleOfChar ? 0.5f : 0f;

        float xPerc = (-_pos.X + charPos.X + offset) / (float)_columns;
        float yPerc = (-_pos.Y + charPos.Y + (_finished ? 0f : offset)) / (float)_rows; // HACK because of weird alignment bug when finished

        float screenX = graphics.PreferredBackBufferWidth / 2.0f + _textSize.X * xPerc;
        float screenY = graphics.PreferredBackBufferHeight / 2.0f + _textSize.Y * yPerc;

        return new Vector2(screenX, screenY);
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDeviceManager graphics)
    {
        spriteBatch.DrawString(_game.Hack12Font, _input, GetCharScreenPos(graphics, Vector2i.Zero, false), new Color(30, 30, 30));

        if (!_finished)
        {
            foreach (Vector2i dir in Vector2i.AllDirections)
            {
                Vector2 start = GetCharScreenPos(graphics, _pos);
                Vector2 end = GetCharScreenPos(graphics, _pos + (dir * (_part1Str.Length - 1)));
                ShapeExtensions.DrawLine(spriteBatch, start, end, Color.Yellow, 2);
            }
        }

        foreach (Xmas xmas in _xmases)
        {
            Vector2 start = GetCharScreenPos(graphics, xmas.Pos);
            Vector2 end = GetCharScreenPos(graphics, xmas.Pos + (xmas.Dir * (_part1Str.Length - 1)));
            ShapeExtensions.DrawLine(spriteBatch, start, end, Color.Orange, 3);
        }

        foreach (Vector2i cross in _masCrosses)
        {
            ShapeExtensions.DrawLine(spriteBatch, GetCharScreenPos(graphics, cross - new Vector2i(1, 1)), GetCharScreenPos(graphics, cross + new Vector2i(1, 1)), Color.Purple, 3);
            ShapeExtensions.DrawLine(spriteBatch, GetCharScreenPos(graphics, cross - new Vector2i(-1, 1)), GetCharScreenPos(graphics, cross + new Vector2i(-1, 1)), Color.Purple, 3);
        }

        spriteBatch.DrawString(_game.Hack20Font, "XMAS count = " + _part1Sum, new Vector2(20, 20), Color.Black);
        spriteBatch.DrawString(_game.Hack20Font, "X-MAS count = " + _part2Sum, new Vector2(20, 60), Color.Black);
    }
}

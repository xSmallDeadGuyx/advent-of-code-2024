using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AoC2024;

public class Day3 : DayBase
{
    public Day3(AoCGame game) : base(game) {}

    public override string GetName()
    {
        return "day3";
    }

    private int _charsPerTick = 20;

    private Regex _regex;

    private float _pauseOnMul = 0.02f;
    private float _pauseLeft = 0.0f;

    private string _lastCommand = "";

    private bool _enabled = true;

    private long _part1Sum = 0;
    private long _part2Sum = 0;

    private bool _finished = false;

    public override void Init()
    {
        _regex = new Regex(@"^(do\(\)|don't\(\)|mul\((\d+),(\d+)\))");
    }

    public override void Update(GameTime gameTime)
    {
        if (_finished)
        {
            return;
        }

        _pauseLeft -= gameTime.GetElapsedSeconds();
        if (_pauseLeft > 0)
        {
            return;
        }

        for (int i = 0; i < _charsPerTick; ++i)
        {
            if (_input.Length == 0)
            {
                _finished = true;
                return;
            }

            Match match = _regex.Match(_input);

            if (!match.Success)
            {
                _input = _input.Substring(1);
            }
            else
            {
                switch (match.Groups[1].Value)
                {
                case "do()":
                    _enabled = true;
                    break;
                case "don't()":
                    _enabled = false;
                    break;
                default:
                    {
                        int left = Int32.Parse(match.Groups[2].Value);
                        int right = Int32.Parse(match.Groups[3].Value);
                        int mul = left * right;
                        
                        _part1Sum += mul;
                        if (_enabled)
                        {
                            _part2Sum += mul;
                        }

                        _pauseLeft = _pauseOnMul;
                        break;
                    }
                
                }
                
                _input = _input.Substring(match.Length);
                _lastCommand = match.Value;

                break;
            }
        }
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDeviceManager graphics)
    {
        Vector2 inputSize = _game.Hack12Font.MeasureString(_input);
        spriteBatch.DrawString(_game.Hack12Font, _input, new Vector2(160, 20), Color.Black);

        spriteBatch.Draw(_enabled ? _game.YesTexture : _game.NoTexture, new Rectangle(20, 20, 120, 120), Color.White);

        int yOff = Math.Max(120, (int)inputSize.Y);

        if (_lastCommand.Length > 0)
        {
            spriteBatch.DrawString(_game.Hack20Font, _lastCommand, new Vector2(20, 40 + yOff), Color.Black);
        }

        spriteBatch.DrawString(_game.Hack20Font, "All mul = " + _part1Sum, new Vector2(20, 80 + yOff), Color.Black);
        spriteBatch.DrawString(_game.Hack20Font, "Enabled mul = " + _part2Sum, new Vector2(20, 120 + yOff), Color.Black);
    }
}
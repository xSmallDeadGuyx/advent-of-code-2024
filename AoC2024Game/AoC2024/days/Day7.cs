using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Particles;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace AoC2024;

public class Day7 : DayBase
{
    public Day7(AoCGame game) : base(game) {}

    public override string GetName()
    {
        return "day7";
    }

    private int _maxExtraIterations = 2;
    private const float _initialTimeBetweenTicks = 0.2f;
    private const float _tickTimeMulti = 0.98f;
    private float _timeBetweenTicks = _initialTimeBetweenTicks;
    private float _currentTimeLeft = 0f;

    private Func<long, long, long>[] _operations1 = {
        (long a, long b) => a + b,
        (long a, long b) => a * b
    };

    private Func<long, long, long>[] _operations2 = {
        (long a, long b) => a + b,
        (long a, long b) => a * b,
        (long a, long b) => long.Parse(a + "" + b)
    };

    private string[] _opStrs = { "+", "*", "||" };


    private long _part1Sum = 0;
    private long _part2Sum = 0;

    private bool _finished = false;

    // Drawing data
    struct LineRenderData {
        public LineRenderData(Color c, string s)
        {
            Color = c;
            Line = s;
        }
        public Color Color;
        public string Line;
    }

    private LineRenderData? _currentLine = null;
    private List<LineRenderData> _doneLines = new List<LineRenderData>();

    private string TrySolve(long target, List<long> nums, Func<long, long, long>[] ops, string line)
    {
        if (nums.Count == 1)
        {
            return nums[0] == target ? line : null;
        }

        for (int i = 0; i < ops.Length; ++i)
        {
            List<long> newNums = new List<long>{ops[i](nums[0], nums[1])};
            newNums.AddRange(nums.Skip(2));
            string newLine = line + (line.Length > 0 ? " " : "") + nums[0] + " " + _opStrs[i] + " " + nums[1];

            string result = TrySolve(target, newNums, ops, newLine);
            if (result != null)
            {
                return result;
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
                _timeBetweenTicks *= _tickTimeMulti;
            }
            _currentTimeLeft = _timeBetweenTicks;

            for (int i = 0; i <= extraIterations; ++i)
            {
                if (_currentLine != null)
                {
                    _doneLines.Insert(0, _currentLine.Value);
                    _currentLine = null;
                }

                string line = ReadNextInputLine();
                if (line == null)
                {
                    _finished = true;
                    return;
                }

                List<long> allNums = SplitOnWhitespace(line).Select(x => long.Parse(x.Trim(':'))).ToList();

                long target = allNums[0];

                List<long> nums = allNums.Skip(1).ToList();


                string p1Res = TrySolve(target, nums, _operations1, "");
                if (p1Res != null)
                {
                    _part1Sum += target;
                    _part2Sum += target;
                    _currentLine = new LineRenderData(Color.Orange, p1Res + " = " + target);
                }
                else
                {
                    string p2Res = TrySolve(target, nums, _operations2, "");
                    if (p2Res != null)
                    {
                        _part2Sum += target;
                        _currentLine = new LineRenderData(Color.Purple, p2Res + " = " + target);
                    }
                    else
                    {
                        _currentLine = new LineRenderData(Color.Red, string.Join(' ', nums) + " != " + target);
                    }
                }
            }
        }
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDeviceManager graphics)
    {
        spriteBatch.DrawString(_game.Hack20Font, "2 op correct = " + _part1Sum, new Vector2(20, 20), Color.Black);
        spriteBatch.DrawString(_game.Hack20Font, "3 op correct = " + _part2Sum, new Vector2(20, 60), Color.Black);
        
        int y = 100;
        if (_currentLine != null)
        {
            spriteBatch.DrawString(_game.Hack20Font, _currentLine.Value.Line, new Vector2(20, y), _currentLine.Value.Color);
            y += 40;
        }

        foreach (LineRenderData line in _doneLines)
        {
            spriteBatch.DrawString(_game.Hack12Font, line.Line, new Vector2(20, y), line.Color);
            y += _game.Hack12Font.LineSpacing;
        }
    }
}

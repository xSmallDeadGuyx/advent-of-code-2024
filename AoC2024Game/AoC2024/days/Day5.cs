using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace AoC2024;

public class Day5 : DayBase
{
    public Day5(AoCGame game) : base(game) {}

    public override string GetName()
    {
        return "day5";
    }

    private int _maxExtraIterations = 3;
    private const float _initialTimeBetweenTicks = 1f;
    private float _timeBetweenTicks = _initialTimeBetweenTicks;
    private float _currentTimeLeft = 0f;

    private Dictionary<int, List<int>> _rules = new Dictionary<int, List<int>>();

    private long _part1Sum = 0;
    private long _part2Sum = 0;

    private bool _finished = false;

    // Drawing data
    private struct LineRenderData
    {
        public LineRenderData(string incorrect, string first, string middle, string end)
        {
            Incorrect = incorrect;
            First = first;
            Middle = middle;
            End = end;
        }

        public string Incorrect;
        public string First;
        public string Middle;
        public string End;
    }

    private List<LineRenderData> _previousLines = new List<LineRenderData>();
    private LineRenderData? _currentLine = null;
    private bool _corrected = false;

    public override void Init()
    {
        bool parsedRules = false;
        while (!parsedRules)
        {
            string line = ReadNextInputLine();
            if (line.Length == 0)
            {
                parsedRules = true;
            }
            else
            {
                int[] nums = line.Split('|').Select(x => Int32.Parse(x)).ToArray();
                if (!_rules.ContainsKey(nums[0]))
                {
                    _rules.Add(nums[0], new List<int>{nums[1]});
                }
                else
                {
                    _rules[nums[0]].Add(nums[1]);
                }
            }
        }
    }

    private LineRenderData MakeLineRenderData(string incorrect, List<int> nums)
    {
        int midIdx = nums.Count / 2;
        string first = string.Join(",", nums.Slice(0, midIdx)) + ",";
        string middle = nums[nums.Count / 2].ToString();
        string end = "," + string.Join(",", nums.Slice(midIdx + 1, midIdx));

        return new LineRenderData(incorrect, first, middle, end);
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
                _timeBetweenTicks *= 0.92f;
            }
            _currentTimeLeft = _timeBetweenTicks;

            for (int i = 0; i <= extraIterations; ++i)
            {
                if (_currentLine != null)
                {
                    _previousLines.Add(_currentLine.Value);
                }

                string line = ReadNextInputLine();
                if (line == null)
                {
                    _currentLine = null;
                    _finished = true;
                    return;
                }

                Dictionary<int, List<int>> constraints = new Dictionary<int, List<int>>();

                List<int> nums = line.Split(',').Select(x => Int32.Parse(x)).ToList();

                // Gather all constraints relevant to this line and check for initial validity.
                bool initiallyValid = true;
                foreach (KeyValuePair<int, List<int>> rule in _rules)
                {
                    int iA = nums.IndexOf(rule.Key);

                    if (iA >= 0)
                    {
                        foreach (int b in rule.Value)
                        {
                            int iB = nums.IndexOf(b);
                            if (iB >= 0)
                            {
                                if (!constraints.ContainsKey(rule.Key))
                                {
                                    constraints.Add(rule.Key, new List<int>());
                                }

                                if (!constraints.ContainsKey(b))
                                {
                                    constraints.Add(b, new List<int>());
                                }

                                constraints[b].Add(rule.Key);

                                if (iA > iB)
                                {
                                    initiallyValid = false;
                                }
                            }
                        }
                    }
                }

                _corrected = !initiallyValid;

                if (initiallyValid)
                {
                    _currentLine = MakeLineRenderData(null, nums);
                    _part1Sum += nums[nums.Count / 2];
                }
                else
                {
                    // Build a new number list by following the constraints:
                    //  1. There will always be 1 (or more if evil input) numbers that have no numbers before it.
                    //  2. Pick one and use it as the next number.
                    //  3. Remove it from the "before" list of all the other numbers.
                    nums = new List<int>();
                    while (constraints.Count > 0)
                    {
                        int nextNum = constraints.Where(kvp => kvp.Value.Count == 0).First().Key;

                        nums.Add(nextNum);
                        constraints.Remove(nextNum);
                        foreach (int otherNum in constraints.Keys)
                        {
                            constraints[otherNum].Remove(nextNum);
                        }
                    }

                    _currentLine = MakeLineRenderData(line + " => ", nums);
                    _part2Sum += nums[nums.Count / 2];
                }
            }
        }
    }

    private void DrawLine(SpriteBatch spriteBatch, SpriteFont font, LineRenderData lineRenderData, Vector2 pos)
    {
        if (lineRenderData.Incorrect != null)
        {
            spriteBatch.DrawString(font, lineRenderData.Incorrect, pos, Color.Red);
            pos.X += font.MeasureString(lineRenderData.Incorrect).X;
        }

        spriteBatch.DrawString(font, lineRenderData.First, pos, new Color(50, 50, 50));
        pos.X += font.MeasureString(lineRenderData.First).X;


        spriteBatch.DrawString(font, lineRenderData.Middle, pos - new Vector2(0, font.LineSpacing * 0.05f), Color.Black, 0f, Vector2.Zero, 1.1f, SpriteEffects.None, 0f);
        pos.X += font.MeasureString(lineRenderData.Middle).X * 1.1f;

        spriteBatch.DrawString(font, lineRenderData.End, pos, new Color(50, 50, 50));
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDeviceManager graphics)
    {
        spriteBatch.DrawString(_game.Hack20Font, "Valid sum = " + _part1Sum, new Vector2(20, 20), Color.Black);
        spriteBatch.DrawString(_game.Hack20Font, "Corrected sum = " + _part2Sum, new Vector2(20, 60), Color.Black);

        int y = 100;

        if (_currentLine != null)
        {
            spriteBatch.Draw(!_corrected ? _game.YesTexture : _game.NoTexture, new Rectangle(20, y, 80, 80), Color.White);
            DrawLine(spriteBatch, _game.Hack20Font, _currentLine.Value, new Vector2(120, y + 40 - _game.Hack20Font.LineSpacing / 2));

            y += 100;
        }

        for (int i = _previousLines.Count - 1; i >= 0; --i)
        {
            LineRenderData line = _previousLines[i];
            DrawLine(spriteBatch, _game.Hack12Font, line, new Vector2(20, y));

            y += _game.Hack12Font.LineSpacing;
        }
    }
}

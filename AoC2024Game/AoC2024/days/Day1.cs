using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC2024;

public class Day1 : DayBase
{
    public Day1(AoCGame game) : base(game) {}

    public override string GetName()
    {
        return "day1";
    }

    private int _linesPerTick = 3;

    private bool _parsed = false;
    private List<int> _pointsA = new List<int>();
    private List<int> _pointsB = new List<int>();

    private int _sumIdx = 0;
    private long _sum = 0;

    private long _similarity = 0;

    private bool _finished = false;

    // Drawing data
    private int _minNum = Int32.MaxValue;
    private int _maxNum = 0;

    private void InsertSorted(ref List<int> points, int num)
    {
        for (int i = 0; i < points.Count; ++i)
        {
            if (points[i] >= num)
            {
                points.Insert(i, num);
                return;
            }
        }

        points.Add(num);
    }

    public override void Update(GameTime gameTime)
    {
        if (_finished)
        {
            return;
        }

        for (int i = 0; i < _linesPerTick; ++i)
        {
            if (!_parsed)
            {
                string line = ReadNextInputLine();
                if (line.Length == 0)
                {
                    _parsed = true;
                }
                else
                {
                    int[] nums = SplitOnWhitespace(line).Select(x => Int32.Parse(x)).ToArray();
                    InsertSorted(ref _pointsA, nums[0]);
                    InsertSorted(ref _pointsB, nums[1]);

                    _minNum = Math.Min(_pointsA.First(), _pointsB.First());
                    _maxNum = Math.Max(_pointsA.Last(), _pointsB.Last());
                }
            }
            else
            {
                if (_sumIdx < _pointsA.Count)
                {
                    _sum += Math.Abs(_pointsA[_sumIdx] - _pointsB[_sumIdx]);
                    _sumIdx++;
                }
                else
                {
                    int iA = 0;
                    int iB = 0;

                    int lastSim = -1;
                    int lastA = -1;
                    while (iA < _pointsA.Count)
                    {
                        if (_pointsA[iA] == lastA)
                        {
                            _similarity += lastSim;
                        }
                        else
                        {
                            int numInB = 0;
                            
                            // Skip lower
                            while(_pointsB[iB] < _pointsA[iA])
                            {
                                iB++;
                            }

                            // Count similar
                            while(_pointsB[iB] == _pointsA[iA])
                            {
                                numInB++;
                                iB++;
                            }

                            lastA = _pointsA[iA];
                            lastSim = _pointsA[iA] * numInB;
                            _similarity += lastSim;
                        }

                        iA++;
                    }

                    _finished = true;
                    return;
                }
            }
        }
    }


    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDeviceManager graphics)
    {
        if (!_parsed)
        {
            spriteBatch.DrawString(_game.Hack20Font, "Parsing", new Vector2(20, 20), Color.Black);
        }
        else
        {
            spriteBatch.DrawString(_game.Hack20Font, "Sum = " + _sum, new Vector2(20, 20), Color.Black);
            if (_finished)
            {
                spriteBatch.DrawString(_game.Hack20Font, "Similarity = " + _similarity, new Vector2(20, graphics.PreferredBackBufferHeight - 60), Color.Black);
            }
        }

        float yInterval = _pointsA.Count > 0 ? (graphics.PreferredBackBufferHeight - 80) / (float)_pointsA.Count : 0;
        float xScale = _maxNum > _minNum ? (graphics.PreferredBackBufferWidth - 40) / (float)(_maxNum - _minNum) : 1;
        for (int i = 0; i < _pointsA.Count; ++i)
        {
            var xA = 20 + (_pointsA[i] - _minNum) * xScale;
            var xB = 20 + (_pointsB[i] - _minNum) * xScale;
            var y = 60 + i * yInterval;

            ShapeExtensions.DrawCircle(spriteBatch, new CircleF(new Vector2(xA, y), 2), 8, Color.Red);
            ShapeExtensions.DrawCircle(spriteBatch, new CircleF(new Vector2(xB, y), 2), 8, Color.Blue);

            if (_parsed && _sumIdx >= i)
            {
                ShapeExtensions.DrawLine(spriteBatch, new Vector2(xA, y), new Vector2(xB, y), _pointsA[i] > _pointsB[i] ? Color.Red : Color.Blue);
            }
        }
    }
}
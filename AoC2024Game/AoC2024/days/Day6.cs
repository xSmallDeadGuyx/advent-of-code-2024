using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Particles;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace AoC2024;

public class Day6 : DayBase
{
    public Day6(AoCGame game) : base(game) {}

    public override string GetName()
    {
        return "day6";
    }

    private int _maxExtraIterations = 4;
    private const float _initialTimeBetweenTicks = 0.2f;
    private const float _tickTimeMulti = 0.98f;
    private float _timeBetweenTicks = _initialTimeBetweenTicks;
    private float _currentTimeLeft = 0f;

    private List<Vector2i> _walls;
    private bool[,] _visited;

    private int _columns;
    private int _rows;
    private Vector2i _pos;
    private Vector2i _dir = new Vector2i(0, -1);

    private long _part1Sum = 0;
    private long _part2Sum = 0;

    private bool _finished = false;

    // Drawing data
    private Dictionary<Vector2i, int> _loopParts = new Dictionary<Vector2i, int>(); // point + recency of loop
    private List<Vector2i> _visitedAsList; // Too slow for path checking but good for drawing
    private List<Vector2i> _loopers = new List<Vector2i>();


    private IEnumerable<Vector2i> FindInLines(string[] lines, char toFind)
    {
        return lines.SelectMany((line, y) => line.Select((c, x) => (c, x, y)).Where(t => t.c == toFind).Select(t => new Vector2i(t.x, t.y)));
    }

    public override void Init()
    {
        string[] lines = _input.Split(new string[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
        _rows = lines.Length;
        _columns = lines[0].Length;

        _walls = FindInLines(lines, '#').ToList();
        _pos = FindInLines(lines, '^').First();
        
        _visited = new bool[_rows, _columns];
        _visitedAsList = new List<Vector2i>{_pos};
        _part1Sum = 1; // Initial pos
    }

    private List<Vector2i> FindLoop(Vector2i startPos, Vector2i startDir)
    {
        Vector2i newWall = startPos + startDir;
        
        Vector2i pos = startPos;
        Vector2i dir = startDir;

        List<Vector2i> returnList = new List<Vector2i>{pos};

        List<Vector2i>[,] visitedDirs = new List<Vector2i>[_rows, _columns];
        visitedDirs[pos.Y, pos.X] = new List<Vector2i>{dir};

        // Iterate until we find a loop or leave the space.
        while (true)
        {
            Vector2i nextPos = pos + dir;
            if (nextPos.X < 0 || nextPos.Y < 0 || nextPos.X >= _columns || nextPos.Y >= _rows)
            {
                // No loop, we went out of bounds.
                return null;
            }

            // Prevent infinite looping if new obstacle boxed in.
            int i = 0;
            while(i < 4 && (_walls.Contains(nextPos) || nextPos.Equals(newWall)))
            {
                dir = dir.Rotate90Clockwise();
                nextPos = pos + dir;
                ++i;
            }

            if (i == 4)
            {
                // Boxed in, successful loop.
                return returnList;
            }

            pos = nextPos;

            if (visitedDirs[pos.Y, pos.X] == null)
            {
                visitedDirs[pos.Y, pos.X] = new List<Vector2i>{dir};
                returnList.Add(pos);
            }
            else
            {
                if (visitedDirs[pos.Y, pos.X].Contains(dir))
                {
                    // Loop
                    return returnList;
                }
                
                visitedDirs[pos.Y, pos.X].Add(dir);
            }
        }
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
                Vector2i nextPos = _pos + _dir;
                if (nextPos.X < 0 || nextPos.Y < 0 || nextPos.X >= _columns || nextPos.Y >= _rows)
                {
                    _pos = nextPos; // Just for drawing.

                    _finished = true;
                    return;
                }

                while (_walls.Contains(nextPos))
                {
                    _dir = _dir.Rotate90Clockwise();
                    nextPos = _pos + _dir;
                }

                // Only check for loops if not already over the path, because we'd have blocked getting here
                if (!_visited[nextPos.Y, nextPos.X])
                {
                    // Check for possible loops before performing the move, knowing there's no wall ahead.
                    List<Vector2i> loop = FindLoop(_pos, _dir);
                    if (loop != null)
                    {
                        _part2Sum++;
                        _loopers.Add(nextPos);
                        
                        foreach (Vector2i cell in loop)
                        {
                            if (_loopParts.ContainsKey(cell))
                            {
                                _loopParts[cell] = (int)_part2Sum;
                            }
                            else
                            {
                                _loopParts.Add(cell, (int)_part2Sum);
                            }
                        }
                    }
                }

                _pos = nextPos;
                if (!_visited[nextPos.Y, nextPos.X])
                {
                    _visited[nextPos.Y, nextPos.X] = true;
                    _visitedAsList.Add(_pos);
                    _part1Sum++;
                }
            }
        }
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDeviceManager graphics)
    {
        spriteBatch.DrawString(_game.Hack20Font, "Visited = " + _part1Sum, new Vector2(20, 20), Color.Black);
        spriteBatch.DrawString(_game.Hack20Font, "Corrected sum = " + _part2Sum, new Vector2(20, 60), Color.Black);

        int cellSize = Math.Min((graphics.PreferredBackBufferHeight - 140) / _rows, (graphics.PreferredBackBufferWidth - 40) / _columns);

        int startX = (int)(graphics.PreferredBackBufferWidth / 2f - (cellSize * _columns) / 2f);
        int startY = (int)(100 + (graphics.PreferredBackBufferHeight - 140) / 2f - (cellSize * _rows) / 2f);

        spriteBatch.Draw(_game.SquareTexture, new Rectangle(startX, startY, cellSize * _columns, cellSize * _rows), Color.White);

        var DrawCell = (Vector2i v, Color c) => {
            spriteBatch.Draw(_game.SquareTexture, new Rectangle(startX + v.X * cellSize, startY + v.Y * cellSize, cellSize, cellSize), c);
        };

        foreach (Vector2i wall in _walls)
        {
            DrawCell(wall, Color.Black);
        }

        foreach (Vector2i visited in _visitedAsList)
        {
            DrawCell(visited, Color.Yellow);
        }

        foreach (KeyValuePair<Vector2i, int> kvp in _loopParts)
        {
            float alpha = kvp.Value / (float)_part2Sum;
            DrawCell(kvp.Key, new Color(1f, 1f - 0.5f * alpha, 1f - alpha)); // Blend to orange
        }

        foreach (Vector2i looper in _loopers)
        {
            DrawCell(looper, Color.Red);
        }

        DrawCell(_pos, Color.Purple);
    }
}

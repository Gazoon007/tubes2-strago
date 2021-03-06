using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Flags]
public enum WallState
{
    // 0000 -> tidak ada dinding
    // 1111 -> kiri, kanan, atas, bawah untuk setiap cell dindingnya tertutup
    LEFT = 1, // 0001
    RIGHT = 2, // 0010
    UP = 4, // 0100
    DOWN = 8, // 1000

    VISITED = 128, // 1000 0000
}

public struct Position
{
    public int X;
    public int Y;
}

public struct Neighbour
{
    public Position Position;
    public WallState SharedWall;
}

public static class MazeGenerator
{

    private static WallState GetOppositeWall(WallState wall)
    {
        switch (wall)
        {
            case WallState.RIGHT: return WallState.LEFT;
            case WallState.LEFT: return WallState.RIGHT;
            case WallState.UP: return WallState.DOWN;
            case WallState.DOWN: return WallState.UP;
            default: return WallState.LEFT;
        }
    }

    private static WallState[,] Backtrack(WallState[,] maze, int width, int height)
    {
        // here we make changes
        var rng = new System.Random(/*seed*/);
        var positionStack = new Stack<Position>();
        var position = new Position { X = rng.Next(0, width), Y = rng.Next(0, height) };

        maze[position.X, position.Y] |= WallState.VISITED;  // 1000 1111
        positionStack.Push(position);

        while (positionStack.Count > 0)
        {
            var current = positionStack.Pop();
            var neighbours = GetUnvisitedNeighbours(current, maze, width, height);

            if (neighbours.Count > 0)
            {
                positionStack.Push(current);

                var randIndex = rng.Next(0, neighbours.Count);
                var randomNeighbour = neighbours[randIndex];

                var nPosition = randomNeighbour.Position;
                maze[current.X, current.Y] &= ~randomNeighbour.SharedWall;
                maze[nPosition.X, nPosition.Y] &= ~GetOppositeWall(randomNeighbour.SharedWall);
                maze[nPosition.X, nPosition.Y] |= WallState.VISITED;

                positionStack.Push(nPosition);
            }
        }

        return maze;
    }

    private static List<Neighbour> GetUnvisitedNeighbours(Position p, WallState[,] maze, int width, int height)
    {
        void AddToList(ICollection<Neighbour> neighbours, Position position, WallState wallState)
        {
            if (!maze[position.X, position.Y].HasFlag(WallState.VISITED))
            {
                neighbours.Add(new Neighbour
                {
                    Position = position,
                    SharedWall = wallState
                });
            }
        }

        var list = new List<Neighbour>();

        if (p.X > 0) // left
            AddToList(list, new Position {X = p.X - 1, Y = p.Y}, WallState.LEFT);

        if (p.Y > 0) // DOWN
            AddToList(list, new Position {X = p.X, Y = p.Y - 1}, WallState.DOWN);

        if (p.Y < height - 1) // UP
            AddToList(list, new Position {X = p.X, Y = p.Y + 1}, WallState.UP);

        if (p.X < width - 1) // RIGHT
            AddToList(list, new Position {X = p.X + 1, Y = p.Y}, WallState.RIGHT);

        return list;
    }

    public static WallState[,] Generate(int width, int height)
    {
        WallState[,] maze = new WallState[width, height];
        WallState initial = WallState.RIGHT | WallState.LEFT | WallState.UP | WallState.DOWN;
        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < height; ++j)
            {
                maze[i, j] = initial;  // 1111
            }
        }
        
        return Backtrack(maze, width, height);
    }
}

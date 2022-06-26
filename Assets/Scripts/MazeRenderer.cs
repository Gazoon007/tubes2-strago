using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeRenderer : MonoBehaviour
{
	[SerializeField] [Range(1, 50)] private int width = 10;

	[SerializeField] [Range(1, 50)] private int height = 10;

	[SerializeField] private float size = 1f;

	[SerializeField] private Transform wallPrefab = null;

	[SerializeField] private Transform floorPrefab = null;

	// Start is called before the first frame update
	void Start()
	{
		var maze = MazeGenerator.Generate(width, height);
		Draw(maze);
	}

	private void Draw(WallState[,] maze)
	{
		var floor = Instantiate(floorPrefab, transform);
		floor.localScale = new Vector3(width, 1, height);

		void PlaceWall(Vector3 position, bool invertAxis)
		{
			var wall = Instantiate(wallPrefab, transform) as Transform;
			wall.position = position;
			wall.localScale = new Vector3(size, wall.localScale.y, wall.localScale.z);
			if (invertAxis) wall.eulerAngles = new Vector3(0, 90, 0);
		}
		
		for (int i = 0; i < width; ++i)
		{
			for (int j = 0; j < height; ++j)
			{
				var cell = maze[i, j];
				var position = new Vector3(-width / 2 + i, 0, -height / 2 + j);

				if (cell.HasFlag(WallState.UP))
					PlaceWall(position + new Vector3(0, 0, size / 2), false);

				if (cell.HasFlag(WallState.LEFT))
					PlaceWall(position + new Vector3(-size / 2, 0, 0), true);

				if (i == width - 1)
					if (cell.HasFlag(WallState.RIGHT))
						PlaceWall(position + new Vector3(+size / 2, 0, 0), true);

				if (j == 0)
					if (cell.HasFlag(WallState.DOWN))
						PlaceWall(position + new Vector3(0, 0, -size / 2), false);
			}
		}
	}

	// Update is called once per frame
	void Update()
	{
	}
}
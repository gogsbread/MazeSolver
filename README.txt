This is a code  for a maze puzzle solver using BFS. The solver solves any walled maze puzzle that defines a start and a finish. Refer to article for general ideas about maze solving algorithms http://en.wikipedia.org/wiki/Maze_solving_algorithm. I wrote the code in C# but you can take the idea and use it for any OO language. The solution files attached work with either VisualStudio or Monodevelop.

Design
=======
	Any maze solving problem can be reduced to a single statement: "How will you find a path from source to destination"? and the answer there-in lies in Graph theory. 

	General steps followed:
	1) Convert the image to an object model 
	2) Run a path finding algorithms against the object model 
	3) Write the results back to the image.

	Most maze images in the web have an average resolution of 440*440 pixels. So, if I were to convert every pixel to a node I would be dealing with < 200,000 nodes(vertices). This is a relatively small dataset and you can use a basic search algorithm such as "Breadth First Search".


	You also have to think about abstracting the maze model from the algorithm that solves the maze. This is very imporatant because any path finding algorithm will alter the state of the maze object model and this action could be unwarranted. Also,future needs may warrant a better algorithm than BFS and hence I painted the puzzle in a OO canvas.
	
	Interfaces:
	----------
		Modelling the maze:
		-------------------
			The following interfaces encapsulate the details of a maze. This model converts an image to an Maze so that any path finding algorithm can be applied.

		IWalledMazeNode:
			Represents a node in the maze. Has Positional and State information.
		IWalledMaze:
			The actual maze. Has all interface methods that a maze solver would need to solve this maze puzzle.

		Modelling the solver:
		---------------------
		IMazeSolver
			This interface encapsulates the algorithm that would run on a maze defined by IWalledMaze. It can maintain its own internal representation of a node and act on those nodes instead of converting the state of the 			original maze.
		

Runtime Analysis:
================

	Since I used BFS the runtime is O(V+E). Sum of total number of vertices and edges.

Usage:
======
	maze source.[bmp,jpg,png] destination.[bmp,jpg,png]

# EllersMaze
A real-time implementation of Eller's Maze Generation Algorithm done in Unity and C#.

### Gameplay
First-person real-time randomly generated maze. The goal of the game is for the player to collect the gem on the other side of the spawn terrain. The player possesses the ability to walk and jump high enough to see the maze layout. The maze is generated row by row, each row triggering when the player walks off the platform of the previous row into the space where the next row should be. 

The player also possesses the ability to fire projectiles ('F'). Firing a projectile requires ammo that can be picked up and is generated in each cell of the maze. 3 projectiles will destroy any inner maze wall beyond the first row. If a projectile is shot in an empty space where the next row should be, the maze is finalized and closed.

### Implementation
The maze is generated row by row using Eller's algorithm, as described on [this page](http://www.neocomputer.org/projects/eller.html), ensuring a simple/perfect maze that is always solvable. See [Assets/Scripts/MazeController.cs](https://github.com/dlrht/EllersMaze/blob/master/Assets/Scripts/MazeController.cs) for the C# code regarding the implementation.

# Generic documentation

## The pixels matrix
The pixel matrix is an integer matrix, whose values correspond to color indices.
- `0` is the index of the background color.
- `1` is the index of the foreground color. These two colors are the minimum required for the game.
- Additional colors have indices starting from `2`.
- The pixels matrix size is given in game pixels, but each game pixel can have a screen size greater than 1, to enhance the old-school feeling of the game making the game more "pixeled". If game pixels size is greater than 1, each game pixel will be painted as a flat-colored square.
- The window size is computed multipling the pixels matrix size by the pixes size.
- The pixels matrix is given as argument to the `OnClear` and `OnDraw` events. Any other logic method (such as the `GameLoop` event logic) will have to live without accessing the matrix.

### Transposed matrix

In memory, the pixels matrix is transposed. When an image matrix is transposed, the image rows correspond to matrix columns, while the first image pixel and the first matrix pixel will both be at the top-left angle. In this situation, moving along the a row of an image will correspond with iterating the element of a matrix column, the image width will correspond with the matrix first dimension, and the image height with the matrix second dimensions. Otherwise, if the image matrix is not transposed, their dimensions have to be swapped every time it's used, accessing matrix pixels before by its Y screen coordinate, then by X.

So, for ease of use, the screen matrix is trasposed in memory. If all pixel matrices used in the game's code are transposed in memory, they will be oriented according to the screen matrix, so they can be drawn on screen and composed one with the other without any other concern.

```
Screen matrix example | Transposed matrix in memory
                      |
 00 01 02 03 04 05    |  00 10 20 30 40
 10 11 12 13 14 15    |  01 11 21 31 41
 20 21 22 23 24 25    |  02 12 22 32 42
 30 31 32 33 34 35    |  03 13 23 33 43
 40 41 42 43 44 45    |  04 14 24 34 44
                      |  05 15 25 35 45
					  |
 Let's consider a coordinate system with origin in
 the top-left angle, an X axis pointing right, and
 a Y axis pointing down:
 
 O ---->  If we try to retrieve the pixel at known
 |     X  coordinates P(x,y), with the first matrix
 |        the pixel will be retrieved as m[y, x],
 |        but after transposition it will be retrieved
 v Y      as m[x, y]. Also, in the screen-oriented
          matrix, the width is the number of columns,
and the height is the number of rows, while, after
transposition, the width will correspond to the number
of rows and the height to the number of columns.
The usage of the transposed matrix is much more
user-friendly.
```

## Colors

The matrix elements are the pixel colors, each identified by its index in the color array. The background color's index is 0 and the foreground color's index is 1. Additional colors start fron index 2. By design, the maximum number of colors for the game is 256. Each color can still be defined in Truecolor space, as a 24 bit color (8 bit red, 8 bit green, 8 bit blue). It's up to the programmer how to correctly handle the additional colors starting from 2 in their game-logic.

# Framework API

## The `GameLogic` class
The `GameLogic` subclass exists to implement the custom game code. It can be used to read `GameConfig` data and to write the game events code.

### The game execution variables
Directly outside the functions code, as instance fields, necessary variables to store the game execution data can be declared.

### The `GameConfig` constants
The following configurations can be retrieved through the `GameConfig` variable, which is accessible in any `GameLogic` function. These values are used by the engine for different purposes, and so they don't exist to be actively used by the user code, just to be configured, so they are meant to be considered as game constants.
- `int PixelsMatrixWidth`: the width of the game matrix in game pixels. *Default:* `50`.
- `int PixelsMatrixHeight`: the height of the game matrix in game pixels. *Default:* `50`.
- `int PixelSize`: the size of each game pixel in screen pixels. Each game pixel is squared and rendered as a flat square. *Default:* `10`.
- `int FrameRate`: the game framerate. The Game Events are called at most once per frame, in a proper order. *Default:* `30`.

- `Color BackgroundColor`: the background color, correspoding to the index `0` when drawed on the pixels matrix. *Default:* `Colors.Black`.
- `Color ForegroundColor`: the foreground color, correspoding to the index `1` when drawed on the pixels matrix. *Default:* `Colors.White`.
- `Color[] AdditionalColors`: additional colors array, whose indices start from `2` when drawed on the pixels matrix. *Default:* `null`.
- `Color InvalidColor`: color drawed when the color index is not valid. *Default:* `Colors.Magenta`. 
- `string Title`: game title, shown by default in the window title bar. *Default:* `"My Retro Game"`.
- `Keys[] KeysPriority`: order by which the keyboard events are served. KeyCodes not appearing here are served in KeyCode order, in any case after these ones. *Default:* `null`.
- `PressReleaseRaceConditionRule PressReleaseRaceConditionPolicy`: policy deciding what happens if key pressed and released events are hit in the same frame for the same KeyCode (this happens the more often the lower the frame rate is). *Available values:* `PressWins`, `ReleaseWins`, `CallBoth`, `CallNone`. *Default:* `PressReleaseRaceConditionRule.CallBoth`.
- `bool forceRandomGeneratorSeed`: forces the initialization of the seed configured in `randomGeneratorSeed` when the game random generator is greated. *Default:* `false`.
- `int randomGeneratorSeed`: contains the seed used to initialize the game random generator if `forceRandomGeneratorSeed` is `true`. *Default:* `0`.

### The game loop events
The API provide multiple events that are called in different moments by the engine thoughout the game execution. They are meant to be used by user code to write the custom game logic. Each event has a specific purpose and it's called in a specific moment of the game loop. **REMEMBER: These functions are already called by the engine: the game code should only complete their body to implement a custom logic.**
- `void OnInitGameConfig(GameConfig GameConfig)`: it's called immeadeately when the execurtable is launched, before actually creating any window. It's meant to customize the `GameConfig` parameter's fields setting proper not default values, as needed.
- `void OnClear(int[,] pixels)`: called at the start of every frame (including the first one, in this case after the `OnStartGame` event), to clear the pixels matrix. Here a cleanup of the previous frame edits should be made. It's adviced, unless performance issues occur, to simply clear the whole matrix here.
- `void OnDraw(int[,] pixels)`: called at the end of every frame, to draw the pixels matrix. Here the matrix should be drawn pixel by pixel according to other game data, such as the game elements position.
- `void OnLoopGame(float deltaTime)`: here the game magic is performed. This method is the hearth of the game logic, and here, frame by frame, the variables storing the game execution data are edited to make the game execute. The main purpose of this method is to let the whole game logic work and to update data that will be then used by the `OnDraw` event to update the pixels matrix. The value of the `deltaTime` argument is the seconds passed between the last frame call and this one. It can be used to normalize speeds, and to perform login related with game time and speed. See *Time statistics* for details about other time statistics.
- `void OnEndGame()`: called just before ending the game. This method is mainly provided as an application exiting callback for the game code to cleanly dispose resources acquired from the system.
 

### Input handling
There are 3 `GameLogic` events that can be implemented (exactly as the game loop events) to write the logic to be executed as response to input events. Only keyboard events are currently implemented. Keyboard events are served according to the keys priority and policies defined in the `GameConfig` configurations (`KeysPriority` and `PressReleaseRaceConditionPolicy` fields).
- `void OnKeyDown(Keys KeyCode)`: called the first frame a key is pressed.
- `void OnKeyPress(Keys KeyCode)`: called if a key is pressed, during each frame following the first one, until the frame the key is released (excluding the frame the key is released).
- `void OnKeyUp(Keys KeyCode)`: called during the frame a Key is released.
 
**IMPORTANT NOTE:** `OnKeyPress` event does not overlap with others, as it is nullified the other key input events (especially by `OnKeyUp`). `OnKeyDown` and `OnKeyUp` can instead overlap during the same frame, in which case the race condition is solved according to the `GameConfig.PressReleaseRaceConditionPolicy` field.
 
### Pause handling
 There are two method than can be used to control the pause state of the game and are available in the scope of any `GameLogic` function:
- `bool IsPaused()`: retrieves whether the game is paused or not. The game engine pauses the game execution during pause and does not call the game loop events. Though it's very important to correctly check the pause state in any state where the game engine do not, especially in the input handling events (as the input could be handled also during pause).
- `void SetPaused (bool paused)`: can be called to pause the game.
 
### Time statistics
The `GameLogic` class provides a number of statistics related with game time:
- `int FrameCount`: the count of ended frames from the start of the game. During the first frame, its value is 0.
- `int FrameCountRunning`: as `FrameCount`, but considers only frames where the game was not paused. 
- `int FrameCountPaused`: as `FrameCount`, but considers only frames where the game was paused.
- `float ElapsedTime`: the total time elapsed from the start of the game, in seconds. It contains decimals measuring second fractions. During the first frame, its value is 0.
- `float ElapsedTimeRunning`: as `ElapsedTime`, but considers only frames where the game was not paused.
- `float ElapsedTimePaused`: as `ElapsedTime`, but considers only frames where the game was paused.

**NOTE:** `void OnLoopGame (float deltaTime)` has an argument storing the amount time (in second) passed between each frame and the previous one. See *The game loop events* section.

### Random numbers generator
The `GameLogic` class provides the reference of a random generator of the [System.Random](https://learn.microsoft.com/en-us/dotnet/api/system.random?view=netframework-4.0) class, meant to be the only random generator used in the whole game. The random generator is available in the scope all functions using the property `RandomGenerator`, but only after the `OnInitGameConfig` event has been called, otherwise `null` is returned. The random generator is initialized according to the `GameConfig.forceRandomGeneratorSeed` and `GameConfig.randomGeneratorSeed` configurations. It's strongly suggested to force a fixed seed when debugging and to note the seed while noting bugs, to be able to reproduce it.

## The GameUtils class

The GameUtils class is a static class containing some helper functions that can be called as `GameUtils.function()` in any game event function. This functions, sometime use other types defined for the helper functions, whose details will be provided later in this documentation.

### Image handling
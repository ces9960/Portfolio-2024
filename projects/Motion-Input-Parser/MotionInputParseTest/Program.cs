#region Project intro
/*This program is a test of a possible solution for interpreting joystick inputs in a 2D fighting game.
 * 
 * In 2D fighting games, special moves are typically performed by inputting a sequence of directions, followed by a button press.  This is commonly referred to as a "motion input", 
 * referencing the fact that in order to perform a special move, a player must perform the associated motion with the joystick before pressing a button.
 * 
 * This input parser assumes the following, as is the case in nearly every 2D fighting game:
 * 1. The game runs at 60 frames per second
 * 2. Inputs are polled once per frame
 * 3. On any given frame, the game accepts 9 possible joystick states (neutral, 4 cardinal directions, 4 diagonals)
 * 
 * Additionally, the following requirements were considered when making this input parser:
 * 1. The input parser must be able to support an arbitrary number of valid motion inputs (there must be no limit on the number of motion inputs that can be checked)
 * 2. Each motion input may have arbitrary complexity (in other words, a motion input may be comprised of any number of individual directions)
 * 3. The complexity of each motion input is completely independent of every other motion input's complexity
 * 4. The length of the input buffer (used to determine the maximum amount of frames that can elapse between directions in a motion input) must be configurable and universal across all motion inputs
 * 5. The input parser must use numpad notation (explained below) to represent directions
 */
#endregion

#region Numpad notation explanation
/* Numpad notation is the de facto standard for representing directional inputs in fighting games.
* In numpad notation, directions are represented as numbers in the same arrangement as a computer keyboard's numpad, assuming the player character is facing towards the right side of the screen.
* 
* This means that numpad notation looks like this for a character facing right:
* 
* 7 8 9
* 4 5 6
* 1 2 3
* 
* If a character is facing left, flip this diagram horizontally.
*/
#endregion

#region Godot implementation of numpad notation
/* To convert directional inputs into numpad notation in Godot, one can use the following calculation:
* 
* (5 + (Input.GetAxis(player_left, player_right) * MathF.Sign(opponentX - playerX)) + (3 * Input.GetAxis(player_up, player_down))
* 
* In Godot, Input.GetAxis returns a value between -1 and +1 depending on the state of the two inputs specified.
* Because 2D fighting games typically don't use analog inputs, Input.GetAxis has three possible values in this case: -1, 0, and +1.
* Numpad notation is based on the direction that the player character is facing, so 6 is always towards the opponent and 4 is always away from the opponent.
* 
* Step by step, this calculation does the following:
* 1. Start at 5 (neutral)
* 2. Add one of the following values based on horizontal input: -1 (away from opponent), 0 (no horizontal input), +1 (towards opponent)
* 3. Add one of the following values based on vertical input: -3 (down), 0 (no vertical input), +3 (up)
* 
*/
#endregion

//a jagged array containing motion inputs in order of priority
int[][] prioArray = new int[][]{
    new int[]{4,1,2,3,6},
    new int[]{6,2,3},
    new int[]{2,3,6},
};


//a jagged array containing various input sequences used to test the input parser.  Comments next to each array state the expected outcome.
int[][] previousInputs = new int[][]
{
    new int[]{6,7,7,7,7,7,7,7,7,7,7,7,7,2,3,6},//236
    new int[]{6,4,1,2,3,6},//41236
    new int[]{6,2,3,6},//623
    new int[]{2,1,4,1,2,3,6},//41236
    new int[]{2,1,4,2,3,6},//236
};

//The maximum amount of frames between each direction before the input is considered invalid
int bufferLength = 3;

//the last input that matches an input in a command
int lastValidInput;

//tests motion input parsing with several input sequences (multiple input sequences are for testing/proof of concept only)
//if fully implemented in a fighting game, the parser would only check one input sequence: the player's input history
for(int i = 0; i < previousInputs.Length; i++)
{
    //tests the current input sequence for a valid motion input based on the priority array
    for (int j = 0; j < prioArray.Length; j++)
    {
        //sets the last valid input to the end of the input history
        lastValidInput = previousInputs[i].Length - 1;
        //check for valid inputs
        if (parseInput(j, i, prioArray[j].Length - 1, previousInputs[i].Length - 1))
        {
            switch (j)//if a valid input is found, print the input in numpad notation and exit the loop
            {
                case 0:
                    Console.WriteLine("41236");
                    break;
                case 1:
                    Console.WriteLine("623");
                    break;
                case 2:
                    Console.WriteLine("236");
                    break;

                default:
                    break;
            }
            break;
        }
    }
}


/* parseInput recursively moves backwards through prioArray[a] and previousInputs[b] to check if previousInputs[b] contains a valid input that matches prioArray[a]
 * 
 * a: position in prioArray
 * b: position in previousInputs
 * c: position in prioArray[a]
 * d: position in previousInputs[b]
 * 
 * In an actual implementation in a fighting game, the argument b would be removed because there would only be one input history per player character.
 * This means that the player character's input history would be treated as previousInputs[b].
 */
bool parseInput(int a, int b, int c, int d)
{
    //checks if the input in history matches the input in command list
    if (prioArray[a][c] == previousInputs[b][d])
    {
        //updates last valid input if a valid input is found
        lastValidInput = d;

        //returns true if beginning of input is reached
        if(c <= 0)
        {
            return true;
        }
        //checks previous input in command
        else
        {
            return parseInput(a, b, c - 1, d);
        }
    }
    //if not matching
    else
    {
        //return false if outside buffer window
        if(lastValidInput - d > bufferLength || d <= 0)
        {
            return false;
        }
        //otherwise, tries again with next input in history
        else
        {
            return parseInput(a, b, c, d - 1);
        }
    }
}
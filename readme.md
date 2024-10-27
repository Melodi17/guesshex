# GuessHex
A simple terminal-based game where you guess the hexadecimal value of a square drawn in the terminal.

## Usage
Simply invoke it from the commandline and start guessing!
```bash
 $ guesshex
 
  ██████████████  ██████████████
  ██████████████  ██████████████
  ██████████████  ██████████████
  ██████████████  ██████████████
  ██████████████  ██████████████
  ██████████████  ██████████████
  ██████████████  ██████████████

       -   = =
1. B 7 4 6 2 3          ██
   -     =
  ------------------------------
   -   -   - =
2. 9 7 5 3 6 6          ██
         -
  ------------------------------
           - =
3. A 7 6 2 7 A          ██
         -
  ------------------------------

4. A 7 6 1 8 E          ██
             -
  ------------------------------

5. A 7 6 1 8 D          ██

  ------------------------------

You won in 5 rounds!
Inaccuracy: 43 [Good]
The color was Tapestry (#A7618D)
```

## Concepts
- The game mostly relies on the terminal's ability to display RGB colors.
I learnt how to do this from [here](https://gist.github.com/fnky/458719343aabd01cfb17a3a4f7296797#rgb-colors).
- The rest is really simple, it just generates a random color and provides hints to the user to guess the color.
    - For the hints, it provides whether the number needs to increase or decrease, and whether by alot or a little. (`> 2`)
- Scores are calculated based on the difference of each digit of the guessed color and the actual color (normalized to positive values) and then summed up.
    ```csharp
    int accuracy = guesses.Select(g =>
                     CalculateHexDiff(targetColor, g) // get hex diff array
                         .Select(x => Math.Abs(x)) // get abs, so -1 and 1 are the same
                          .Sum()) // sum all diffs
                  .Sum(); // sum all rounds
    ```
// using System.Diagnostics.CodeAnalysis;
// using Perfolizer.Mathematics.SignificanceTesting;
// using Sudoku;
// using Puzzle = PuzzleMultiDimensionalArray.Puzzle;

// namespace BacktrackerFive;

// /*
//     Backtracker, based on array, collection, span, and integer data types.
//     Adds use of pre-computed data, spans, bit twiddling, and ref ints relative to baseline.
//     Relies on a helper class for some of the more complicated Sudoku logic.

//     This implementation is built on the following premises:

//     - We can represent the units (rows, columns, boxes) as a set of 9 cell lists, with legal values 0-9.
//     - The order of the cells doesn't actually matter. We just need to know if a given value is present.
//     - Using these lists, we can determine which values are in view to produce candidate lists for a given cell.
//     - Given the use of recursion, the stack represents the puzzle with all the correct final values.
//     - A "solution" array can be created very late, to collect the final puzzle data that the stack contains.
// */

// public static class Backtracker
// {   
//     public static bool Solve(int[,] board, [NotNullWhen(true)] out int[]? solution)
//     {
//         if (!IsValid(board))
//         {
//             solution = null;    
//             return false;
//         }   

//         Puzzle puzzle = new(board);
//         return Solver(puzzle, board, 0, out solution) && IsValid(solution, true);
//     }

//     private static bool Solver(Puzzle puzzle, ReadOnlySpan<int> board, int index, out int[]? solution)
//     {
//         solution = null;
//         if (board[index] > 0)
//         {
//             if (index is 80)
//             {
//                 solution = GetSolution(board[index]);
//                 return true;
//             }

//             if (Solver(puzzle, board, index + 1, out solution))
//             {
//                 if (solution is not null)
//                 {
//                     solution[index] = board[index];
//                 }

//                 return true;
//             }

//             return false;
//         }

//         Cell cell = puzzle.Cells[index];
//         int viewValues = puzzle.GetValuesInView(cell);
//         int oldValue = 0;
//         int valuesMask = 1;

//         for (int i = 1; i < 10; i++)
//         {
//             valuesMask <<= 1;
//             bool found = (viewValues & valuesMask) > 0;

//             if (found)
//             {
//                 continue;
//             }

//             puzzle.UpdateCell(cell, oldValue, i);
//             oldValue = i;

//             if (index is 80)
//             {
//                 solution = GetSolution(i);
//                 return true;
//             }
            
//             if (Solver(puzzle, board, index + 1, out solution))
//             {
//                 if (solution is not null)
//                 {
//                     solution[cell] = i;
//                 }

//                 return true;
//             }
//         }

//         puzzle.UpdateCell(cell, oldValue, 0);
//         return false;
//     }

//     public static int[] GetSolution(int value)
//     {
//         var solution = new int[81];
//         solution[80] = value;
//         return solution;
//     }

//     private static bool IsValid(int[,] board, bool testForEmpties = false)
//     {
//         if (board.Length != 81)
//         {
//             return false;
//         }

//         if (testForEmpties)
//         {
//             foreach (int value in board)
//             {
//                 if (value is 0)
//                 {
//                     return false;
//                 }
//             }

//             return false;
//         }

//         for (int i = 0; i < 9; i++)
//         {
//             if (IsValidLine(board, rows.Slice(i * 16, 9)) && 
//                 IsValidLine(board, columns.Slice(i * 16, 9)) && 
//                 IsValidLine(board, boxes.Slice(i * 16, 9)))
//             {
//                 continue;
//             }

//             return false;
//         }

//         return true;
//     }

//     private static bool IsValidLine(ReadOnlySpan<int> board, ReadOnlySpan<int> indices)
//     {
//         int bitMask = 0;
//         foreach (int value in indices)
//         {
//             if (value is 0)
//             {
//                 continue;
//             }

//             int bit = 1 << value;
//             bitMask ^= bit;
//             if ((bitMask & bit) == 0)
//             {
//                 return false;
//             }
//         }

//         return true;
//     }
// }

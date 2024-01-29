// using System.Diagnostics.CodeAnalysis;

// namespace Sudoku;

// public class LastCandidateSolver : ISolver
// {
//     public bool TrySolve(Puzzle puzzle, [NotNullWhen(true)] out Solution? solution)
//     {
//         for (int i = 0; i < 9; i++)
//         {
//             if (TrySolveRow(i, puzzle, out solution) ||
//                 TrySolveColumn(i, puzzle, out solution) ||
//                 TrySolveBox(i, puzzle, out solution ))
//             {
//                 return true;
//             }
//         }
        
//         solution = default;
//         return false;
//     }

//     private bool TrySolveRow(int index, Puzzle puzzle, [NotNullWhen(true)] out Solution? solution)
//     {
//         solution = default;

//         if (puzzle.SolvedForRow(index) != 8)
//         {
//             return false;
//         }

//         var (zeroIndex, missingValue) = FindLastCandidate(puzzle.GetCellsForRow(index));

//         if (missingValue is -1)
//         {
//             return false;
//         }

//         solution = new(index, zeroIndex, missingValue, nameof(LastCandidateSolver));
//         return true;
//     }

//     private bool TrySolveColumn(int index, Puzzle puzzle, [NotNullWhen(true)] out Solution? solution)
//     {
//         solution = default;

//         if (puzzle.SolvedForRow(index) != 8)
//         {
//             return false;
//         }

//         var (zeroIndex, missingValue) = FindLastCandidate(puzzle.GetCellsForColumn(index));

//         if (missingValue is -1)
//         {
//             solution = default;
//             return false;
//         }

//         solution = new(zeroIndex, index, missingValue, nameof(LastCandidateSolver));
//         return true;
//     }

//     private bool TrySolveBox(int index, Puzzle puzzle, [NotNullWhen(true)] out Solution? solution)
//     {
//         solution = default;

//         if (puzzle.SolvedForRow(index) != 8)
//         {
//             return false;
//         }

//         var (zeroIndex, missingValue) = FindLastCandidate(puzzle.GetCellsForBox(index));

//         if (missingValue is -1)
//         {
//             return false;
//         }

//         solution = new(index, zeroIndex, missingValue, nameof(LastCandidateSolver));
//         return true;
//     }

//     public static (int Index, int Value) FindLastCandidate(IEnumerable<int> group)
//     {
//         bool[] values = new bool[10];
//         int counter = 0;
//         int zeroIndex = -1;

//         foreach (var value in group)
//         {
//             if (values[value])
//             {
//                 return (-1, 1);
//             }
//             else if (value is 0)
//             {
//                 zeroIndex = counter;
//             }

//             values[value] = true;
//             counter++;
//         }

//         int missingValue = 0;

//         for (int i = 0; i < values.Length; i++)
//         {
//             if (!values[i])
//             {
//                 missingValue = i;
//                 break;
//             }
//         }

//         return (zeroIndex, missingValue);
//     }
// }
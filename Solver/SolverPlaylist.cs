namespace Sudoku;

public class SolverPlaylist(IReadOnlyList<ISolver> solvers)
{
    private readonly IReadOnlyList<ISolver> _solvers = solvers;
    private readonly Stack<ISolver> _priorityList = new(1);

    public IEnumerable<ISolver> Play()
    {
        int count = 0;
        while (count < _solvers.Count)
        {
            if (_priorityList.TryPop(out ISolver? result))
            {
                yield return result;
                continue;
            }
            
            yield return _solvers[count];
            count++;
        }
    }

    public void Add(ISolver solver) => _priorityList.Push(solver);
}
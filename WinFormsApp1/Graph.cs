namespace WinFormsApp1;

public class Graph
{
    private readonly List<GraphVertex> _vertices = [];
    private readonly List<GraphEdge> _edges = [];

    private int _lastId = 0;

    public int LastId => _lastId;

    public IReadOnlyList<GraphVertex> Vertices => _vertices;
    public IReadOnlyList<GraphEdge> Edges => _edges;
    

    public GraphVertex AddVertex()
    {
        var vertex = new GraphVertex(_lastId++);
        _vertices.Add(vertex);
        return vertex;
    }

    public bool RemoveVertex(int id)
    {
        var v = _vertices.FirstOrDefault(x => x.Id == id);
        if (v == null)
        {
            return false;
        }

        _edges.RemoveAll(e => e.From == id || e.To == id);
        _vertices.Remove(v);
        return true;
    }

    public bool HasVertex(int id) => _vertices.Any(x => x.Id == id);

    public bool AddEdge(int fromId, int toId)
    {
        if (fromId == toId) return false;
        if (!HasVertex(fromId) || !HasVertex(toId)) return false;
        if (HasEdge(fromId, toId)) return false;

        _edges.Add(new GraphEdge(fromId, toId));
        return true;
    }

    public IEnumerable<int> Neighbors(int id) =>
        _edges
            .Where(e => e.From == id || e.To == id)
            .Select(e => e.From == id ? e.To : e.From);

    public IEnumerable<GraphEdge> IncidentEdges(int id) =>
        _edges.Where(e => e.From == id || e.To == id);

    public bool HasEdge(int id1, int id2) =>
        _edges.Any(e => (e.From == id1 && e.To == id2) || (e.To == id1 && e.From == id2));
}
namespace WinFormsApp1;

public class BfsSettings : AlgorithmSettings
{
    public List<(int From, int To)> Edges { get; set; } = [];

    public List<(int Id, int X, int Y)> Vertices { get; set; } = [];

    public int StartVertexId { get; set; } = -1;

    public BfsSettings()
    {
        Name = "Обход графа в ширину (BFS)";
        Description =
            "BFS обходит граф уровень за уровнем, начиная от стартовой вершины.\n" +
            "Использует очередь. Гарантирует кратчайший путь в невзвешенном графе.\n\n";
    }
}
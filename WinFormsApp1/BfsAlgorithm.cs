using Microsoft.Extensions.Logging;

namespace WinFormsApp1;

public class BfsAlgorithm : IBfsAlgorithm
{
    private readonly ILogger<BfsAlgorithm> _logger;

    public BfsAlgorithm(ILogger<BfsAlgorithm> logger)
    {
        _logger = logger;
    }

    public List<BfsStep> Run(AlgorithmSettings settings)
    {
        if (settings is not BfsSettings bfsSettings)
        {
            _logger.LogError("Переданы настройки неверного типа: {Type}", settings.GetType().Name);
            throw new ArgumentException("Ожидались BfsSettings");
        }

        _logger.LogInformation("Запуск BFS. Вершин: {V}, рёбер: {E}, старт: {S}",
            bfsSettings.Vertices.Count, bfsSettings.Edges.Count, bfsSettings.StartVertexId);

        var graph = BuildGraph(bfsSettings);
        var steps = graph.BfsSteps(bfsSettings.StartVertexId).ToList();

        _logger.LogInformation("BFS завершён. Шагов: {Steps}", steps.Count);
        return steps;
    }

    public void SaveToFile(string filePath, List<BfsStep> steps, BfsSettings settings)
    {
        try
        {
            _logger.LogInformation("Сохранение в файл: {Path}", filePath);

            using var writer = new StreamWriter(filePath);

            writer.WriteLine("VERTICES");
            foreach (var v in settings.Vertices)
            {
                writer.WriteLine($"{v.Id};{v.X};{v.Y}");
            }

            writer.WriteLine("EDGES");
            foreach (var (from, to) in settings.Edges)
                writer.WriteLine($"{from};{to}");

            writer.WriteLine("START");
            writer.WriteLine(settings.StartVertexId);

            writer.WriteLine("STEPS");
            foreach (var step in steps)
                writer.WriteLine($"{step.Id};{step.Depth};{string.Join(",", step.Neighbors)}");

            _logger.LogInformation("Сохранение завершено успешно");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при сохранении файла: {Path}", filePath);
            throw;
        }
    }

    public BfsSettings LoadFromFile(string filePath)
    {
        try
        {
            _logger.LogInformation("Загрузка из файла: {Path}", filePath);

            var settings = new BfsSettings();
            settings.Vertices = new List<(int Id, int X, int Y)>();
            var lines = File.ReadAllLines(filePath);
            var section = "";

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                if (line is "VERTICES" or "EDGES" or "START" or "STEPS")
                {
                    section = line;
                    continue;
                }

                switch (section)
                {
                    case "VERTICES":
                        var vParts = line.Split(';');
                        if (vParts.Length == 3)
                        {
                            settings.Vertices.Add((
                                int.Parse(vParts[0]),
                                int.Parse(vParts[1]),
                                int.Parse(vParts[2])
                            ));
                        }
                        else if (line.Contains(','))
                        {
                            var ids = line.Split(',').Select(int.Parse);
                            foreach (var id in ids)
                                settings.Vertices.Add((id, 0, 0)); 
                        }

                        break;

                    case "EDGES":
                        var eParts = line.Split(';');
                        if (eParts.Length == 2)
                            settings.Edges.Add((int.Parse(eParts[0]), int.Parse(eParts[1])));
                        break;

                    case "START":
                        if (int.TryParse(line, out int startId))
                            settings.StartVertexId = startId;
                        break;
                }
            }

            settings.CollectionSize = settings.Vertices.Count;
            _logger.LogInformation("Загрузка завершена. Вершин: {V}, рёбер: {E}",
                settings.Vertices.Count, settings.Edges.Count);

            return settings;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при загрузке файла: {Path}", filePath);
            throw;
        }
    }

    private static Graph BuildGraph(BfsSettings settings)
    {
        var graph = new Graph();

        foreach (var vertex in settings.Vertices)
            graph.AddVertex(vertex.Id);

        foreach (var (from, to) in settings.Edges)
            graph.AddEdge(from, to);

        return graph;
    }
}
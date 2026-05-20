namespace WinFormsApp1;

public interface IBfsAlgorithm
{
    List<BfsStep> Run(AlgorithmSettings settings);

    void SaveToFile(string filePath, List<BfsStep> steps, BfsSettings settings);

    BfsSettings LoadFromFile(string filePath);
}
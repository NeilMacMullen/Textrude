namespace TextrudeInteractive
{
    public record TextrudeProject
    {
        public string Description { get; init; } = string.Empty;
        public EngineInputSet EngineInput { get; init; } = EngineInputSet.EmptyYaml;
    }
}
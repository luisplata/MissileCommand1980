namespace Model
{
    public sealed class SectorConfig
    {
        public readonly int Sector;
        public readonly int TotalCount;
        public readonly float SpeedMultiplier;

        public SectorConfig(int sector, int totalCount, float speedMultiplier)
        {
            Sector = sector;
            TotalCount = totalCount;
            SpeedMultiplier = speedMultiplier;
        }
    }

    public static class SectorConfigs
    {
        public static readonly SectorConfig[] All =
        {
            new SectorConfig(1, 8, 1f),
            new SectorConfig(2, 11, 1f),
            new SectorConfig(3, 15, 1f),
            new SectorConfig(4, 19, 1f),
            new SectorConfig(5, 20, 1.2f)
        };
    }
}
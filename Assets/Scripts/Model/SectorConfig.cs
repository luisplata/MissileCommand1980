namespace Model
{
    public sealed class SectorConfig
    {
        public readonly int Sector;
        public readonly int Normals;
        public readonly int Mirvs;
        public readonly int Inteligentes;

        public int TotalCount => Normals + Mirvs + Inteligentes;

        public SectorConfig(int sector, int normals, int mirvs, int inteligentes)
        {
            Sector = sector;
            Normals = normals;
            Mirvs = mirvs;
            Inteligentes = inteligentes;
        }
    }

    public static class SectorConfigs
    {
        public static readonly SectorConfig[] All =
        {
            new SectorConfig(1, 8, 0, 0),
            new SectorConfig(2, 10, 1, 0),
            new SectorConfig(3, 12, 2, 1),
            new SectorConfig(4, 14, 3, 2),
            new SectorConfig(5, 14, 4, 2)
        };
    }
}
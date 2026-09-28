namespace Model
{
    public class RunSession
    {
        public delegate void RunEvent();
        public RunEvent RunStarted;
        public RunEvent RunOver;
        public RunEvent RunWon;
        public RunEvent SectorStarted;
        public RunEvent CityDied;

        public static RunSession Current;

        private readonly bool[] _citiesAlive = new bool[6];
        private bool _over;
        private bool _won;

        public int Sector { get; private set; }
        public int Score { get; private set; }
        public int Kills { get; private set; }
        public int AliveCities { get; private set; }

        public float Life => AliveCities * 100f;

        public RunSession()
        {
            Sector = 1;
            for (var i = 0; i < _citiesAlive.Length; i++)
            {
                _citiesAlive[i] = true;
            }
            AliveCities = _citiesAlive.Length;
            RunStarted?.Invoke();
        }

        public bool CityAlive(int id)
        {
            return id >= 0 && id < _citiesAlive.Length && _citiesAlive[id];
        }

        public void KillCity(int id)
        {
            if (_over || _won) return;
            if (!CityAlive(id)) return;
            _citiesAlive[id] = false;
            AliveCities--;
            CityDied?.Invoke();
            if (AliveCities == 0)
            {
                _over = true;
                RunOver?.Invoke();
            }
        }

        public void CompleteSector()
        {
            if (_over || _won) return;
            if (Sector >= 5)
            {
                if (AliveCities > 0)
                {
                    _won = true;
                    RunWon?.Invoke();
                }
                return;
            }
            Sector++;
            SectorStarted?.Invoke();
        }

        public void AddScore()
        {
            Score++;
        }

        public void AddKill()
        {
            Kills++;
        }
    }
}
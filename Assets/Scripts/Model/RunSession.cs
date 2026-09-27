namespace Model
{
    public class RunSession
    {
        public delegate void RunEvent();
        public RunEvent RunStarted;
        public RunEvent RunOver;

        public static RunSession Current;

        public float Life { get; private set; }
        public int Score { get; private set; }
        public int Wave { get; private set; }

        private readonly float _milestoneEvery;

        public RunSession(float life, int milestoneEvery)
        {
            Life = life;
            Score = 0;
            Wave = 1;
            _milestoneEvery = milestoneEvery;
            RunStarted?.Invoke();
        }

        public void ApplyDamage(float damage)
        {
            if (Life <= 0) return;
            Life -= damage;
            if (Life <= 0)
            {
                RunOver?.Invoke();
            }
        }

        public void AddScore()
        {
            Score++;
        }

        public bool ShouldAddMissile()
        {
            return Score % _milestoneEvery == 0;
        }
    }
}
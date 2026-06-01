
using System;

namespace MusicalTails.Model.Core
{
    public partial class InputHandler
    {
        public int TotalScore { get; private set; }
        public event Action<int> OnScoreChanged;

        public void AddScore(int points)
        {
            TotalScore += points;
            OnScoreChanged?.Invoke(TotalScore); 
        }
    }
}
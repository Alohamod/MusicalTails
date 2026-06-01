namespace MusicalTails.Model.Core
{
    public partial class InputHandler
    {
        public bool IsGameOver { get; private set; }
        public void HandleMiss() => IsGameOver = true; 
    }
}

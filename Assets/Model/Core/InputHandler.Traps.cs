namespace MusicalTails.Model.Core
{
    public partial class InputHandler
    {
        public void HandleTrapInteraction(BaseButton button)
        {
            if (button is TrapButton)
            {
                IsGameOver = true;
            }
        }
    }
}
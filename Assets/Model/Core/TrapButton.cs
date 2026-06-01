
namespace MusicalTails.Model.Core
{
    public class TrapButton : BaseButton
    {
        public TrapButton(int lane, float spawnTime) : base(lane, spawnTime) { }
        public override void ExecuteAction() { }
        public override int GetScore() => 0; 
    }
}
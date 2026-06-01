namespace MusicalTails.Model.Core
{
    public class Short : BaseButton
    {
        public Short(int lane, float spawnTime) : base(lane, spawnTime) { }

        public override void ExecuteAction()
        {
            
        }
        public override int GetScore() => 10;

    }
}

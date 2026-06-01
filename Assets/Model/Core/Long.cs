namespace MusicalTails.Model.Core
{
    public class Long : BaseButton
    {
        public Long(int lane, float spawnTime) : base(lane, spawnTime) { }

        public override void ExecuteAction()
        {

        }
        public override int GetScore() => 15;

    }
}

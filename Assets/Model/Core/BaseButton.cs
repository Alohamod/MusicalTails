
namespace MusicalTails.Model.Core 
{
    public abstract class BaseButton :  IMusicalButton
    {

        public int Lane {  get; private set; }
        public float SpawnTime { get; private set; }


        public BaseButton(int lane, float spawnTime)
        {
            Lane = lane;
            SpawnTime = spawnTime;
        }

        public abstract void ExecuteAction();

        public abstract int GetScore();
    }
}

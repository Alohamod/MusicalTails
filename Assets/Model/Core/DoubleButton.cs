using MusicalTails.Model.Core;
using UnityEngine;

namespace MusicalTails.Model.Core
{
    public class DoubleButton : BaseButton
    {
        public int HitsRequired { get; private set; } = 2;
        public int HitsReceived { get; private set; } = 0;
        public bool IsPartiallyHit => HitsReceived == 1;

        public DoubleButton(int lane, float spawnTime) : base(lane, spawnTime) { }

        public override void ExecuteAction() { HitsReceived++; }
        public override int GetScore() => 30;

        public bool IsFinished => HitsReceived >= HitsRequired;
    }
}
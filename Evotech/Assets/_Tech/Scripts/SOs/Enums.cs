namespace Core.Data
{
    public class Enums
    {
        public enum WeaponType
        {
            None = -1,
            OneHandSword = 0,
        }

        public enum UnitType
        {
            None = -1,
            PlayerAlly = 0,
            EnemyRed = 1,
            EnemyNeutral = 2,
            Player = 3,
        }

        public enum OutlineType
        {
            None = -1,
            PlayerAlly = 0,
            Enemy = 1,
            Player = 3,
        }

        public enum UnitRelation
        {
            None = -1,
            Ally = 0,
            Enemy = 1,
        }

        public enum SpiritArmorPointType
        {
            None = -1,
            Head = 0,
            Collar = 1,
            LShoulder = 2,
            RShoulder = 3,
            LElbow = 4,
            RElbow = 5,
            LHand = 6,
            RHand = 7,
            Torso0 = 8,
            Torso1 = 9,
        }

        public enum PointedLineType
        {
            None = -1,
            Attack = 0,
        }
    }
}
using System;

namespace park
{
    public class Park
    {
        public List<Spot> Spots { get; set; }

        public int GetAllSpotCount() => Spots.Count;

        public int GetUnusedSpotCount()
        {
            return Spots.FindAll(spot => !spot.IsOccupied).Count;
        }

        public List<Spot> GetUnusedSpots()
        {
            return Spots.FindAll(spot => !spot.IsOccupied).ToList();
        }

        public bool IsParkFull()
        {
            return GetUnusedSpotCount() == 0;
        }

        public bool IsParkEmpty()
        {
            return GetUnusedSpotCount() == GetAllSpotCount();
        }

        public bool IsParkFullBySpotType(SpotType _type)
        {
            return Spots
                .FindAll(spot =>
                    spot.GetType() == _type && spot.OccupiedBy != null)
                .Count ==
            0;
        }
    }

    public class Motorcycle : Automobile
    {
        public override bool CanPark(Park park)
        {
            if (park.IsParkFull())
            {
                return false;
            }

            return true;
        }

        public override void Park(Park park)
        {
            if (park.IsParkFull())
            {
                throw Exception("Can't park in a full parking lot");
            }
        }
    }

    public class Van : Automobile
    {
        public override bool CanPark(Park park)
        {
            if (park.IsParkFull())
            {
                return false;
            }

            List<Spot> freeSpots = park.GetUnusedSpots();

            var possibleLargeSpot =
                freeSpots.Find(spot => spot.SpotType == SpotType.Large);

            var possibleNormalSpots =
                freeSpots.FindAll(spot => spot.SpotType == SpotType.Normal);

            if (possibleLargeSpot == null && possibleNormalSpots.Count < 3)
            {
                return false;
            }

            return true;
        }

        public override void Park(Park park)
        {
            if (park.IsParkFull())
            {
                throw Exception("Can't park in a full parking lot");
            }

            List<Spot> freeSpots = park.GetUnusedSpots();

            var possibleLargeSpot =
                freeSpots.Find(spot => spot.SpotType == SpotType.Large);

            if (possibleLargeSpot != null)
            {
                var newSpot = possibleLargeSpot;

                Occuping = new List<Spot>();

                Occuping.Add (newSpot);

                newSpot.OccupiedBy = this;

                return;
            }

            var possibleNormalSpots =
                freeSpots.FindAll(spot => spot.SpotType == SpotType.Normal);

            if (possibleNormalSpots.Count >= 3)
            {
                var newSpots = new List<Spot>();

                for (int i = 0; i < 3; i++)
                {
                    var spot = possibleNormalSpots[i];
                    newSpots.Add (spot);
                    spot.OccupiedBy = this;
                }

                Occuping = newSpots;

                return;
            }

            throw Exception("Couldn't find a fitting parking spot");
        }
    }

    public class Car : Automobile
    {
        public override bool CanPark(Park park)
        {
            if (park.IsParkFull())
            {
                return false;
            }

            List<Spot> freeSpots = park.GetUnusedSpots();

            var possibleSpots =
                freeSpots
                    .Find(spot =>
                        spot.SpotType == SpotType.Normal ||
                        spot.SpotType == SpotType.Large);

            if (possibleSpots == null)
            {
                return false;
            }

            return true;
        }

        public override void Park(Park park)
        {
            if (park.IsParkFull())
            {
                throw Exception("Can't park in a full parking lot");
            }

            List<Spot> freeSpots = park.GetUnusedSpots();

            var possibleSpots =
                freeSpots
                    .FindAll(spot =>
                        spot.SpotType == SpotType.Normal ||
                        spot.SpotType == SpotType.Large);

            if (possibleSpots.Count == 0)
            {
                throw Exception("Couldn't find a fitting parking spot");
            }

            var newSpot = possibleSpots[0];

            Occuping = new List<Spot>();

            Occuping.Add (newSpot);

            newSpot.OccupiedBy = this;
        }
    }

    public abstract class Automobile
    {
        public List<Spot> Occuping = new List<Spot>();

        public abstract bool CanPark(Park park);

        public abstract void Park(Park park);

        public void Depart()
        {
            foreach (Spot spot in Occuping)
            {
                spot.OccupiedBy = null;
            }

            Occuping = new List<Spot>();
        }
    }

    public class Spot
    {
        public Automobile OccupiedBy { get; set; }

        public SpotType SpotType { get; set; }
    }

    public enum SpotType
    {
        Small = 0,
        Normal = 1,
        Large = 2
    }
}

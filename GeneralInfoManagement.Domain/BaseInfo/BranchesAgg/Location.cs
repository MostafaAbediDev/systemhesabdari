namespace GeneralInfoManagement.Domain.BaseInfo.BranchesAgg
{
    public class Location
    {
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }

        protected Location() { }

        public Location(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }
    }

}

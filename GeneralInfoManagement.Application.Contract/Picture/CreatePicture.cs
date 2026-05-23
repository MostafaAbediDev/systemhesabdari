namespace GeneralInfoManagement.Application.Contract.Picture
{
    public class CreatePicture
    {
        public long OwnerId { get; set; }
        public PictureOwnerTypeDTO OwnerType { get; set; }
        public string Url { get; set; }
    }
}

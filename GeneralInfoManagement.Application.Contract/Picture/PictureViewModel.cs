namespace GeneralInfoManagement.Application.Contract.Picture
{
    public class PictureViewModel
    {
        public long Id { get; set; }
        public string Url { get; set; }
        public bool IsMain { get; set; }
        public string CreationDate { get; set; }
    }
}

using _0_FrameWork.Domain;

namespace CodeManagement.Domain.CodeAgg
{
    public class Codes : EntityBase
    {
        public string Value { get; private set; }

        public long OwnerId { get; private set; }

        public CodeOwnerType OwnerType { get; private set; }

        protected Codes() { }

        public Codes(string value, long ownerId, CodeOwnerType ownerType)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Code value is required");

            Value = value;
            OwnerId = ownerId;
            OwnerType = ownerType;
        }

        public void ChangeValue(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Code value is required");

            Value = value;
        }
    }

}

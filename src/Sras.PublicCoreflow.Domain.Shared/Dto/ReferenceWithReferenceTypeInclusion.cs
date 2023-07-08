namespace Sras.PublicCoreflow.ConferenceManagement
{
    // for Website & social links
    // for other IDs

    public class ReferenceWithReferenceTypeInclusion : Reference
    {
        public int ReferenceTypeId { get; set; }
        public string ReferenceTypeName { get; set; }
        public bool? IsRequired { get; set; }
    }
}

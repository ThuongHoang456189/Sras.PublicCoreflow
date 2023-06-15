using System;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class RegistrationResponseDto
    {
        public bool IsSuccessful { get; set; }
        public string? Message { get; set; }
        public Guid? RegistrationId { get; set; }
        public Guid? OrderId { get; set; }
        public OrderDto? Order { get; set; }
    }
}

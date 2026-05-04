using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Kmums.Models.Contact
{
    public class ContactModel
    {
        public int Id { get; set; }

        [Required]
        [DisplayName("Your email address")]
        [DataType(DataType.EmailAddress)]
        public string Sender { get; set; }
        public string Receiver { get; set; }

        [Required]
        [DisplayName("Message title")]
        public string Title { get; set; }

        [Required]
        [DisplayName("Message")]
        public string Body { get; set; }
        public string Sent { get; set; }
    }
}

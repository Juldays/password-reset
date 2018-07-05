using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Requests
{
    public class ResetUpdateRequest
    {
        [Required]
        [MaxLength(100)]
        public string Password { get; set; }

        [Required]
        public Guid Token { get; set; }
    }
}

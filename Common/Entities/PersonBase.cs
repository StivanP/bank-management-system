using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Common.Entities
{
    public abstract class PersonBase
    {
        [Required, MaxLength(50)]
        public string FirstName { get; set; } = null!;

        [Required, MaxLength(50)]
        public string LastName { get; set; } = null!;

        [Required, MaxLength(100)]
        public string Email { get; set; } = null!;

        [Required, MaxLength(255)]
        public string Password { get; set; } = null!;

        [Required, MaxLength(255)]
        public string Address { get; set; } = null!;
    }
}

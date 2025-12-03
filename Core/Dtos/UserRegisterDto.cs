using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos
{
    public class UserRegisterDto
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string DisplayName { get; set; }
    }
}

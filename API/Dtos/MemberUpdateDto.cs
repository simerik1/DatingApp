using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Dtos
{
    public class MemberUpdateDto
    {
        public string? Interests { get; set; }
        public string? Introduction { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? LookingFor { get; set; }

    }
}
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManagement
{
    public class IdentityLib
    {
        public class TaskIdentityUser : IdentityUser
        {
            public string FullName { get; set; }
            public string AccountRole { get; set; }
            public int? UserID { get; set; }
            public bool MustChangePwd { get; set; }
            public bool Approved { get; set; }
            public string ApprovedBy { get; set; }
            public DateTime DateApproved { get; set; }
        }
        public class TaskIdentityRole : IdentityRole
        {
            public string RoleId { get; set; }
            public int? AccessLevel { get; set; }
        }
    }
}

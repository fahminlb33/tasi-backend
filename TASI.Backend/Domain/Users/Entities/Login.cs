using System;
using System.ComponentModel.DataAnnotations;
using TASI.Backend.Infrastructure.Database;

namespace TASI.Backend.Domain.Users.Entities
{
    public class Login : IDaoEntity
    {
        [Key]
        public int LoginId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        
        public User User { get; set; }
        
        public DateTime ModifiedDate { get; set; }
    }
}

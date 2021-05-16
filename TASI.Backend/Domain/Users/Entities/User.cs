using System;
using System.ComponentModel.DataAnnotations;
using TASI.Backend.Infrastructure.Database;

namespace TASI.Backend.Domain.Users.Entities
{
    public class User : IDaoEntity
    {
        [Key]
        public int UserId { get; set; }
        public string FullName { get; set; }
        public UserRole Role { get; set; }
        
        public Login Login { get; set; }
        
        public DateTime ModifiedDate { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace SQLModel.App1.Models{

    public class Product{
        [Key]
        public string Name {get;set;}
        public string Image {get;set;}
        public string Category {get;set;}
        public float Price {get;set;}

    }

    public class User{
        [Key]
        public int Id {get;set;}
        public string Email {get;set;}
        public string Password {get;set;}
        public string Role {get;set;}
    }
    
    public class Order{
        [Key]
        public int Id {get;set;}
        public string UserEmail {get;set;}
        public string OrderJson {get;set;}
    }
}

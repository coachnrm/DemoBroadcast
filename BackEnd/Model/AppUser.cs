using System;
using Microsoft.AspNetCore.Identity;

namespace BackEnd.Model;

public class AppUser : IdentityUser
{
    public string Name {get; set;}
}

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SearchSharp.Demo.EF.Tables;

[Table("UserAccounts")]
public class UserAccount {
    [Key]
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; }  = string.Empty;
    public bool IsEnabled { get; set; } = true;

    public virtual ICollection<UserAddress> Addresses { get; set; } = Array.Empty<UserAddress>();

    public override string ToString() => $"{Id} [isEnabled:{IsEnabled}]=> {Name} - {Email}" + ((Addresses != null && Addresses.Count() != 0) ? 
        $" Addresses:>[{Addresses.Select(addr => addr.Id.ToString()).Aggregate((prev, next) => $"{prev},{next}")}]" : string.Empty);
}
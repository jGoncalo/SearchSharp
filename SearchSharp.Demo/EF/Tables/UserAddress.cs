using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SearchSharp.Demo.EF.Tables;

[Table("UserAddresses")]
public class UserAddress {
    [Key]
    public Guid Id { get; set; }
    [ForeignKey("fk_userId")]
    public Guid UserId { get; set; }
    public string Street { get; set; } = string.Empty;
    public string Zip { get; set; }  = string.Empty;

    public virtual UserAccount User { get; set; } = new UserAccount();

    public override string ToString() => $"{Id} [userId:{UserId}]=> {Street} - {Zip}";
}
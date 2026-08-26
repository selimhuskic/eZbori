using System.ComponentModel.DataAnnotations.Schema;

namespace Application.Models;

public record RefreshToken(
    int UserId,
    string Token,
    DateTime CreatedAt,
    DateTime ExpiryDate)
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public User User { get; set; } = null!;

    public RefreshToken UpdateToken(string Token, DateTime CreatedAt, DateTime ExpiryDate)
    {
        return this with
        {
            Token = Token,
            CreatedAt = CreatedAt,
            ExpiryDate = ExpiryDate
        };
    }
};

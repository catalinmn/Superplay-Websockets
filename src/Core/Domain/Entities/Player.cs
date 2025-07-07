namespace Core.Domain.Entities;

public class Player
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string DeviceId { get; set; }

    [Required]
    public int Coins { get; set; } = 100;

    [Required]
    public int Rolls { get; set; } = 10;

    public Player(string deviceId)
    {
        DeviceId = deviceId;
    }

    public int AddResources(ResourceType type, int amount)
    {
        if (amount == 0) return 0;

        switch (type)
        {
            case ResourceType.Coins:
                if (Coins + amount < 0)
                    throw new InsufficientResourcesException(
                        type,
                        $"Not enough coins. Current: {Coins}, Required: {-amount}");
                Coins += amount;
                return Coins;

            case ResourceType.Rolls:
                if (Rolls + amount < 0)
                    throw new InsufficientResourcesException(
                        type,
                        $"Not enough rolls. Current: {Rolls}, Required: {-amount}");
                Rolls += amount;
                return Rolls;

            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
}
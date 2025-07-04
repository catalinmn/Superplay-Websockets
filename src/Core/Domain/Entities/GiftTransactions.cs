using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Domain.Enums;

namespace Core.Domain.Entities;

public class GiftTransaction
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Required]
    public Guid SenderId { get; set; }

    [Required]
    public Guid ReceiverId { get; set; }

    [Required]
    public ResourceType ResourceType { get; set; }

    [Required]
    public int Amount { get; set; }

    [Required]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public GiftTransaction(
        Guid senderId,
        Guid receiverId,
        ResourceType resourceType,
        int amount)
    {
        SenderId = senderId;
        ReceiverId = receiverId;
        ResourceType = resourceType;
        Amount = amount;
    }
}
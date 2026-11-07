using MedicalSmartBox.API.Transport.Domain.Enums;
using MedicalSmartBox.API.Transport.Domain.ValueObjects;

namespace MedicalSmartBox.API.Transport.Domain.Aggregates;

public class TransportOrder
{
    public Guid Id { get; private set; }
    public string OrderCode { get; private set; }
    public Guid OriginHospitalId { get; private set; }
    public Guid DestinationHospitalId { get; private set; }
    public PriorityLevel Priority { get; private set; }
    public TransportOrderStatus Status { get; private set; }
    public CargoType CargoType { get; private set; }
    public string CargoDescription { get; private set; }
    public IschemiaTimeLimit MaxIschemiaLimit { get; private set; }
    public Guid CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; private set; }

    public TransportOrder(string orderCode, Guid originHospitalId, Guid destinationHospitalId, PriorityLevel priority, CargoType cargoType, string cargoDescription, IschemiaTimeLimit maxIschemiaLimit, Guid createdByUserId)
    {
        if(string.IsNullOrWhiteSpace(orderCode))
            throw new ArgumentException("Order code cannot be null or empty.", nameof(orderCode));
        
        if(OriginHospitalId==Guid.Empty)
            throw new ArgumentException("Origin hospital ID cannot be empty.", nameof(originHospitalId));
        
        if(destinationHospitalId==Guid.Empty)
            throw new ArgumentException("Destination hospital ID cannot be empty.", nameof(destinationHospitalId));
        
        if(originHospitalId==destinationHospitalId)
            throw new ArgumentException("Origin and destination hospital IDs cannot be the same.");
        
        if(string.IsNullOrWhiteSpace(cargoDescription))
            throw new ArgumentException("Cargo description cannot be null or empty.", nameof(cargoDescription));
        
        if(maxIschemiaLimit is null)
            throw new ArgumentNullException(nameof(maxIschemiaLimit), "Max ischemia limit cannot be 0.");
        
        if(createdByUserId == Guid.Empty)
            throw new ArgumentException("Creator user ID is required.", nameof(createdByUserId));
        
        Id = Guid.NewGuid();
        OrderCode = orderCode;
        OriginHospitalId = originHospitalId;
        DestinationHospitalId = destinationHospitalId;
        Priority = priority;
        Status = TransportOrderStatus.Pending;
        CargoType = cargoType;
        CargoDescription = cargoDescription;
        MaxIschemiaLimit = maxIschemiaLimit;
        CreatedByUserId = createdByUserId;
        CreatedAt = DateTime.UtcNow;
    }
    
    public bool ValidateIschemiaFeasibility(int estMinutes)
    {
        if (estMinutes < 0)
            throw new ArgumentException("Estimated minutes cannot be negative.");

        double estimatedHours = estMinutes / 60.0;

        return estimatedHours <= MaxIschemiaLimit.MaxSafeHours;
    }

    public void CancelOrder(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Cancellation reason is required.");

        if (Status == TransportOrderStatus.Completed)
            throw new InvalidOperationException(
                "A completed transport order cannot be cancelled.");

        if (Status == TransportOrderStatus.Cancelled)
            throw new InvalidOperationException(
                "This transport order is already cancelled.");

        Status = TransportOrderStatus.Cancelled;
    }
    
}
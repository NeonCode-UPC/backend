using MedicalSmartBox.API.Shared.Application.Patterns;
using MedicalSmartBox.API.Shared.Domain.Repositories;
using MedicalSmartBox.API.SmartContainers.Application.CommandServices;
using MedicalSmartBox.API.SmartContainers.Application.Errors;
using MedicalSmartBox.API.SmartContainers.Domain.Model.Aggregates;
using MedicalSmartBox.API.SmartContainers.Domain.Model.Commands;
using MedicalSmartBox.API.SmartContainers.Domain.Model.ValueObjects;
using MedicalSmartBox.API.SmartContainers.Domain.Repositories;

namespace MedicalSmartBox.API.SmartContainers.Application.Internal.CommandServices;

/// <summary>
///     Application command service orchestrating SmartContainer write operations and persistence.
/// </summary>
public class SmartContainerCommandService(
    ISmartContainerRepository repository,
    IUnitOfWork unitOfWork)
    : ISmartContainerCommandService
{
    public async Task<Result<SmartContainer, SmartContainerError>> Handle(
        CreateSmartContainerCommand command,
        CancellationToken cancellationToken = default)
    {
        var serial = new SerialNumber(command.SerialNumber);
        if (await repository.ExistsBySerialNumberAsync(serial, cancellationToken))
            return new Result<SmartContainer, SmartContainerError>.Failure(SmartContainerError.DuplicateSerialNumber);

        var thresholds = new ThermalThresholds(
            command.MinTemperatureCelsius,
            command.MaxTemperatureCelsius,
            command.WarningTemperatureCelsius,
            command.ExcursionToleranceSeconds);

        var container = new SmartContainer(
            command.SerialNumber,
            command.FormFactor,
            command.TareWeightGrams,
            command.InitialTemperature,
            thresholds);

        await repository.AddAsync(container, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);

        return new Result<SmartContainer, SmartContainerError>.Success(container);
    }

    public async Task<Result<SmartContainer, SmartContainerError>> Handle(
        PairContainerToAmbulanceCommand command,
        CancellationToken cancellationToken = default)
    {
        var container = await repository.FindByIdAsync(command.ContainerId.Value, cancellationToken);
        if (container is null)
            return new Result<SmartContainer, SmartContainerError>.Failure(SmartContainerError.NotFound);

        try
        {
            container.PairToAmbulance(command.AmbulancePlate);
            repository.Update(container);
            await unitOfWork.CompleteAsync(cancellationToken);
            return new Result<SmartContainer, SmartContainerError>.Success(container);
        }
        catch (InvalidOperationException)
        {
            return new Result<SmartContainer, SmartContainerError>.Failure(SmartContainerError.AlreadyPaired);
        }
    }

    public async Task<Result<SmartContainer, SmartContainerError>> Handle(
        UnlinkContainerCommand command,
        CancellationToken cancellationToken = default)
    {
        var container = await repository.FindByIdAsync(command.ContainerId.Value, cancellationToken);
        if (container is null)
            return new Result<SmartContainer, SmartContainerError>.Failure(SmartContainerError.NotFound);

        container.UnlinkFromAmbulance();
        repository.Update(container);
        await unitOfWork.CompleteAsync(cancellationToken);

        return new Result<SmartContainer, SmartContainerError>.Success(container);
    }

    public async Task<Result<SmartContainer, SmartContainerError>> Handle(
        RecordWeightMovementCommand command,
        CancellationToken cancellationToken = default)
    {
        var container = await repository.FindByIdAsync(command.ContainerId.Value, cancellationToken);
        if (container is null)
            return new Result<SmartContainer, SmartContainerError>.Failure(SmartContainerError.NotFound);

        container.RecordWeightMovement(command.NetWeightGrams);
        repository.Update(container);
        await unitOfWork.CompleteAsync(cancellationToken);

        return new Result<SmartContainer, SmartContainerError>.Success(container);
    }
}

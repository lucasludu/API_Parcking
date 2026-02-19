using Application.Features._ticket.Commands.CreateTicketCommands;
using Application.Interfaces;
using Ardalis.Specification;
using Domain.Entities;
using Domain.Enums;
using FluentAssertions;
using Moq;

namespace Application.UnitTests.Features.Tickets.Commands
{
    public class CreateTicketCommandHandlerTests
    {
        private readonly Mock<IRepositoryAsync<Ticket>> _ticketRepositoryMock;
        private readonly Mock<IDateTimeService> _dateTimeServiceMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly Mock<IRepositoryAsync<Lugar>> _lugarRepositoryMock;
        private readonly Mock<INotifier> _notifierMock;
        private readonly CreateTicketCommandHandler _handler;

        public CreateTicketCommandHandlerTests()
        {
            _ticketRepositoryMock = new Mock<IRepositoryAsync<Ticket>>();
            _dateTimeServiceMock = new Mock<IDateTimeService>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _lugarRepositoryMock = new Mock<IRepositoryAsync<Lugar>>();
            _notifierMock = new Mock<INotifier>();

            _handler = new CreateTicketCommandHandler(
                _ticketRepositoryMock.Object,
                _dateTimeServiceMock.Object,
                _currentUserServiceMock.Object,
                _lugarRepositoryMock.Object,
                _notifierMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldCreateTicket_WhenValidationPasses()
        {
            // Arrange
            var command = new CreateTicketCommand(new Models.Request._ticket.CreateTicketRequest
            {
                CocheraId = Guid.NewGuid(),
                LugarId = Guid.NewGuid(),
                Patente = "ABC1234",
                TipoVehiculo = TipoVehiculo.Auto
            });

            _lugarRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Lugar { Id = command.Request.LugarId!.Value });

            _ticketRepositoryMock.Setup(x => x.AnyAsync(It.IsAny<ISpecification<Ticket>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _dateTimeServiceMock.Setup(x => x.NowUtc).Returns(DateTime.UtcNow);
            _currentUserServiceMock.Setup(x => x.UserId).Returns("user-123");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeTrue();
            _ticketRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Ticket>(), It.IsAny<CancellationToken>()), Times.Once);
            _ticketRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _notifierMock.Verify(x => x.NotifyDashboardUpdate(command.Request.CocheraId), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldFail_WhenLugarDoesNotExist()
        {
            // Arrange
            var command = new CreateTicketCommand(new Models.Request._ticket.CreateTicketRequest
            {
                CocheraId = Guid.NewGuid(),
                LugarId = Guid.NewGuid(),
                Patente = "ABC1234"
            });

            _lugarRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Lugar)null!);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeFalse();
            result.Message.Should().Be("El lugar especificado no existe.");
        }

        [Fact]
        public async Task Handle_ShouldFail_WhenLugarIsOccupied()
        {
            // Arrange
            var command = new CreateTicketCommand(new Models.Request._ticket.CreateTicketRequest
            {
                CocheraId = Guid.NewGuid(),
                LugarId = Guid.NewGuid(),
                Patente = "ABC1234"
            });

            _lugarRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Lugar { Id = command.Request.LugarId!.Value });

            _ticketRepositoryMock.Setup(x => x.AnyAsync(It.IsAny<ISpecification<Ticket>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true); // Occupied

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeFalse();
            result.Message.Should().Be("El lugar ya se encuentra ocupado.");
        }
    }
}

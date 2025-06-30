using Azure.Core;
using OdontFlow.Application.BussinesProcess.Auth.Command;
using OdontFlow.Application.BussinesProcess.Auth.Query;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.Application.Services.Contracts;
using OdontFlow.CrossCutting.Exceptions;
using OdontFlow.Domain.DTOs;
using OdontFlow.Domain.DTOs.User;
using OdontFlow.Domain.Entities;
using OdontFlow.Domain.ViewModel.User;
using OdontFlow.Infraestructure.Authentication;

namespace OdontFlow.Application.Services
{
    public class AuthService(IMediator mediator, IJwtProvider jwt) : IAuthService
    {
        private const string INITIAL_PASSWORD = "G4mm@1234!";
        public async Task<AuthResult> ChangeInitialPassword(Guid userId, ResetFirstPasswordRequest request)
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            request.NewPassword = hashedPassword;
            var handler = mediator.GetCommandHandler<ChangeFirstTimePasswordCommand, User>();
            var user = await handler.Handle(new ChangeFirstTimePasswordCommand(userId, request));
            if (user is null)
                throw new Exception("No se pudo actualizar el password del usuario.");
            var token = jwt.GenerateToken(user);
            return new AuthResult { Token = token, ChangePassword = false };
        }

        public  async Task<IEnumerable<UserViewModel>> GetUsersAsync()
        {
            var handler = mediator.GetQueryHandler<GetUsersQuery, IEnumerable<UserViewModel>>();
            var result = await handler.Handle(new GetUsersQuery());
            return result;
        }

        public async Task<AuthResult> LoginAsync(LoginRequest request)
        {
            var handler = mediator.GetQueryHandler<GetUserByEmailQuery, User>();
            var user = await handler.Handle(new GetUserByEmailQuery(request));
            if (user == null) throw new NotFoundException("Usuario no encontrado");
            if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                throw new Exception("Credenciales inválidas");

            var token = jwt.GenerateToken(user);            
            return new AuthResult { Token = token, ChangePassword = user.ChangePassword };
        }

        public async Task<AuthResult> RegisterAsync(RegisterRequest request)
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(INITIAL_PASSWORD);
            request.Password = hashedPassword;
            var handler = mediator.GetCommandHandler<CreateRegisterCommand, User>();
            var user = await handler.Handle(new CreateRegisterCommand(request));
            if (user is null)
                throw new Exception("No se pudo create registrar el usuario.");

            var token = jwt.GenerateToken(user);
            return new AuthResult { Token = token, ChangePassword = true };
        }


        public async Task<UserViewModel> UpdateAsync(UpdateUserDTO request)
        {
            var handler = mediator.GetCommandHandler<UpdateRegisterCommand, UserViewModel>();
            var user = await handler.Handle(new UpdateRegisterCommand(request));
            if (user is null)
                throw new Exception("No se pudo actualizar el usuario.");

            return user;

        }
    }
}

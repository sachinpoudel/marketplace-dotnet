using FluentValidation;

namespace MarketPlace.Application.Features.Auth.Command.RefreshToken;


public class RefreshTokenValidator: AbstractValidator<RefreshTokenCommand> {
    public RefreshTokenValidator() {
        RuleFor(x => x.RefreshToken).NotEmpty();
    
    } 
}
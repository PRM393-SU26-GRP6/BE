using CourtManager.Domain.Entities;

namespace CourtManager.Application.Interfaces;

public interface IEmailVerificationTokenRepository
{
    Task AddAsync(EmailVerificationToken token, CancellationToken cancellationToken = default);
    Task<EmailVerificationToken?> GetValidTokenAsync(string email, string otp, CancellationToken cancellationToken = default);
    Task UpdateAsync(EmailVerificationToken token, CancellationToken cancellationToken = default);
}

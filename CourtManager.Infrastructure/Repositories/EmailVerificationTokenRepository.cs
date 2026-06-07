using Microsoft.EntityFrameworkCore;
using CourtManager.Application.Interfaces;
using CourtManager.Domain.Entities;

namespace CourtManager.Infrastructure.Repositories;

public class EmailVerificationTokenRepository : IEmailVerificationTokenRepository
{
    private readonly ApplicationDbContext _dbContext;

    public EmailVerificationTokenRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(EmailVerificationToken token, CancellationToken cancellationToken = default)
    {
        _dbContext.EmailVerificationTokens.Add(token);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<EmailVerificationToken?> GetValidTokenAsync(string email, string otp, CancellationToken cancellationToken = default)
    {
        return await _dbContext.EmailVerificationTokens
            .FirstOrDefaultAsync(t =>
                t.Email == email &&
                t.Token == otp &&
                !t.IsUsed &&
                t.ExpiresAt > DateTime.UtcNow,
                cancellationToken);
    }

    public async Task UpdateAsync(EmailVerificationToken token, CancellationToken cancellationToken = default)
    {
        _dbContext.EmailVerificationTokens.Update(token);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}

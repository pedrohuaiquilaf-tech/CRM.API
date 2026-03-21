namespace CRM.API.Auth;

using CRM.Domain.Common;
using CRM.Domain.Entities;
using CRM.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly IRepository<User> _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly JwtService _jwtService;

    public AuthController(IRepository<User> userRepository, IUnitOfWork unitOfWork, JwtService jwtService)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _jwtService = jwtService;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest("Email and password are required.");

        if (request.Role is not (Roles.Admin or Roles.Sales or Roles.Viewer))
            return BadRequest($"Role must be one of: {Roles.Admin}, {Roles.Sales}, {Roles.Viewer}.");

        var existing = await _userRepository.FindAsync(u => u.Email == request.Email, cancellationToken);
        if (existing.Count > 0)
            return Conflict("A user with this email already exists.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = JwtService.HashPassword(request.Password),
            Role = request.Role,
            CreatedAt = DateTime.UtcNow,
        };

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var (token, expiresAt) = _jwtService.GenerateToken(user.Id, user.Email, user.Role);
        return CreatedAtAction(null, new AuthResponse(token, expiresAt, user.Role));
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.FindAsync(u => u.Email == request.Email, cancellationToken);
        var user = users.FirstOrDefault();

        if (user is null || !JwtService.VerifyPassword(request.Password, user.PasswordHash))
            return Unauthorized("Invalid email or password.");

        var (token, expiresAt) = _jwtService.GenerateToken(user.Id, user.Email, user.Role);
        return Ok(new AuthResponse(token, expiresAt, user.Role));
    }
}

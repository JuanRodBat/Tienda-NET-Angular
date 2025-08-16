using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Company.Project.Data;
using Company.Project.Entitys.Models;
using Company.Project.Front.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Company.Project.Front.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _ctx;
    private readonly IConfiguration _cfg;
    private readonly IPasswordHasher<Cliente> _hasher;

    public AuthController(AppDbContext ctx, IConfiguration cfg)
    {
        _ctx = ctx;
        _cfg = cfg;
        _hasher = new PasswordHasher<Cliente>();
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] ClienteCreateDto dto)
    {
        if (await _ctx.Clientes.AnyAsync(c => c.Email == dto.Email))
            return BadRequest("Email ya registrado.");

        var cli = new Cliente
        {
            Nombre = dto.Nombre,
            Apellidos = dto.Apellidos,
            Direccion = dto.Direccion,
            Email = dto.Email,
            FechaRegistro = DateTime.UtcNow
        };
        cli.PasswordHash = _hasher.HashPassword(cli, dto.Password);

        _ctx.Clientes.Add(cli);
        await _ctx.SaveChangesAsync();
        return Ok(new { cli.Id });
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest req)
    {
        var c = await _ctx.Clientes.FirstOrDefaultAsync(x => x.Email == req.Email);
        if (c is null) return Unauthorized("Credenciales inválidas.");

        var vr = _hasher.VerifyHashedPassword(c, c.PasswordHash, req.Password);
        if (vr == PasswordVerificationResult.Failed) return Unauthorized("Credenciales inválidas.");

        var key = Encoding.UTF8.GetBytes(_cfg["Jwt:Key"]!);
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("cid", c.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{c.Nombre} {c.Apellidos}")
            }),
            Expires = DateTime.UtcNow.AddHours(8),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        });

        return new LoginResponse(tokenHandler.WriteToken(token), c.Id, $"{c.Nombre} {c.Apellidos}");
    }
}

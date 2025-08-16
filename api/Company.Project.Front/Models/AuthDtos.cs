namespace Company.Project.Front.Models;

public record LoginRequest(string Email, string Password);
public record LoginResponse(string Token, int ClienteId, string NombreCompleto);

public record ClienteCreateDto(string Nombre, string Apellidos, string? Direccion, string Email, string Password);

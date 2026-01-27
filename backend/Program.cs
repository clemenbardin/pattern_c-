using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MonApp.API.Services;
using MonApp.API.Middleware;
using MonApp.API.Models;

var builder = WebApplication.CreateBuilder(args);

// Configuration
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<DataService>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? "VotreCleSecreteTresLongueEtSecuriseePourJWT123456789";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Middleware
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<JwtMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Routes
app.MapControllers();

// Endpoints d'authentification
app.MapPost("/api/auth/login", (LoginRequest request, AuthService authService, HttpContext context) =>
{
    var response = authService.Login(request);
    if (response == null)
        return Results.Unauthorized();

    // Cookie optionnel
    context.Response.Cookies.Append("auth_token", response.Token, new CookieOptions
    {
        HttpOnly = true,
        Secure = false, // true en production avec HTTPS
        SameSite = SameSiteMode.Lax,
        Expires = DateTimeOffset.UtcNow.AddHours(24)
    });

    return Results.Ok(response);
});

app.MapPost("/api/auth/logout", (HttpContext context) =>
{
    context.Response.Cookies.Delete("auth_token");
    return Results.Ok(new { message = "Déconnexion réussie" });
});

app.MapGet("/api/auth/me", (HttpContext context) =>
{
    if (context.User.Identity?.IsAuthenticated != true)
        return Results.Unauthorized();

    return Results.Ok(new
    {
        Username = context.User.Identity.Name,
        Role = context.User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value
    });
});

// Endpoints pour les clients
app.MapGet("/api/clients", (DataService dataService) => Results.Ok(dataService.GetAllClients()))
   .RequireAuthorization();

app.MapGet("/api/clients/{id}", (int id, DataService dataService) =>
{
    var client = dataService.GetClientById(id);
    return client != null ? Results.Ok(client) : Results.NotFound();
}).RequireAuthorization();

app.MapPost("/api/clients", (CreateClientDto dto, DataService dataService) =>
{
    var client = dataService.CreateClient(dto);
    return Results.Created($"/api/clients/{client.Id}", client);
}).RequireAuthorization();

app.MapPut("/api/clients/{id}", (int id, CreateClientDto dto, DataService dataService) =>
{
    var client = dataService.UpdateClient(id, dto);
    return client != null ? Results.Ok(client) : Results.NotFound();
}).RequireAuthorization();

app.MapDelete("/api/clients/{id}", (int id, DataService dataService) =>
{
    var success = dataService.DeleteClient(id);
    return success ? Results.NoContent() : Results.NotFound();
}).RequireAuthorization();

// Endpoints pour les commandes
app.MapGet("/api/commandes", (DataService dataService) => Results.Ok(dataService.GetAllCommandes()))
   .RequireAuthorization();

app.MapGet("/api/commandes/client/{clientId}", (int clientId, DataService dataService) =>
    Results.Ok(dataService.GetCommandesByClient(clientId)))
   .RequireAuthorization();

app.MapGet("/api/commandes/{id}", (int id, DataService dataService) =>
{
    var commande = dataService.GetCommandeById(id);
    return commande != null ? Results.Ok(commande) : Results.NotFound();
}).RequireAuthorization();

app.MapPost("/api/commandes", (CreateCommandeDto dto, DataService dataService) =>
{
    try
    {
        var commande = dataService.CreateCommande(dto);
        return Results.Created($"/api/commandes/{commande.Id}", commande);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
}).RequireAuthorization();

app.MapDelete("/api/commandes/{id}", (int id, DataService dataService) =>
{
    var success = dataService.DeleteCommande(id);
    return success ? Results.NoContent() : Results.NotFound();
}).RequireAuthorization();

// Endpoints pour les documents
app.MapPost("/api/documents/generate", (GenerateDocumentRequest request, DataService dataService) =>
{
    try
    {
        var document = dataService.GenerateDocument(request.DocumentType, request.ClientType);
        return Results.Ok(document);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
}).RequireAuthorization();

app.Run();
namespace MonApp.API.Models;

public class ClientDto
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // "Comptant" ou "Credit"
    public int? DureeCreditParDefaut { get; set; }
    public decimal? TauxInteretParDefaut { get; set; }
}

public class CreateClientDto
{
    public string Nom { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int? DureeCreditParDefaut { get; set; }
    public decimal? TauxInteretParDefaut { get; set; }
}

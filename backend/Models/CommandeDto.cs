namespace MonApp.API.Models;

public class CommandeDto
{
    public int Id { get; set; }
    public DateTime DateCommande { get; set; }
    public decimal Montant { get; set; }
    public string Vehicule { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // "Comptant" ou "Credit"
    public int? DureeCredit { get; set; }
    public decimal? TauxInteret { get; set; }
    public decimal? Mensualite { get; set; }
    public int ClientId { get; set; }
}

public class CreateCommandeDto
{
    public decimal Montant { get; set; }
    public string Vehicule { get; set; } = string.Empty;
    public int ClientId { get; set; }
    public int? DureeCredit { get; set; }
    public decimal? TauxInteret { get; set; }
}

public abstract class Client
{
    public string Nom { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public abstract Commande créeCommande();
}
public abstract class Commande {
    public int Id { get; set;}
    public DateTime DateCommande { get; set; } = DateTime.Now;
    public decimal Montant { get; set; }
    public string Vehicule { get; set; } = string.Empty;
    public abstract void TraiterPaiement();
}
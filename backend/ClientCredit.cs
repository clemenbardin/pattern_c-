public class ClientCrédit : Client
{
    public int DureeCreditParDefaut { get; set; } = 36;
    public decimal TauxInteretParDefaut { get; set; } = 3.5m;

    public override Commande créeCommande()
    {
        return new CommandeCrédit
        {
            DureeCredit = DureeCreditParDefaut,
            TauxInteret = TauxInteretParDefaut
        };
    }
}
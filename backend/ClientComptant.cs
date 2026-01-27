public class ClientComptant : Client
{
    public override Commande créeCommande()
    {
        return new CommandeComptant();
    }
}
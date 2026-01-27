public class CommandeComptant : Commande
{
    public override void TraiterPaiement()
    {
        Console.WriteLine($"Paiement au comptant de {Montant}€ pour le véhicule {Vehicule}");
    }
}
using System;

public class CommandeCrédit : Commande
{
    public int DureeCredit { get; set; }
    public decimal TauxInteret { get; set; }

    public override void TraiterPaiement()
    {
        decimal mensualite = CalculerMensualite();
        Console.WriteLine($"Paiement par crédit de {Montant}€ pour le véhicule {Vehicule}");
        Console.WriteLine($"Mensualité: {mensualite:F2}€ sur {DureeCredit} mois");
    }

    private decimal CalculerMensualite()
    {
        // Formule de calcul d'emprunt: M = P * (r * (1 + r)^n) / ((1 + r)^n - 1)
        // où P = principal (Montant), r = taux mensuel, n = nombre de mois
        if (DureeCredit <= 0 || TauxInteret < 0 || Montant <= 0)
            return 0;
        
        decimal tauxMensuel = TauxInteret / 100 / 12;
        decimal puissance = (decimal)Math.Pow((double)(1 + tauxMensuel), DureeCredit);
        
        if (puissance == 1) // Éviter division par zéro
            return Montant / DureeCredit;
        
        return Montant * (tauxMensuel * puissance) / (puissance - 1);
    }
}
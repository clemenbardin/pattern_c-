using System;

namespace BankDocumentSystem
{
    // Nouveau produit abstrait
    public abstract class AttestationFiscale
    {
        protected string Logo { get; } = "LOGO_BANQUE";
        protected string Titre { get; set; }
        
        public abstract string Generate();
        
        protected string AjouterEnTete()
        {
            return $"=== {Logo} ===\n{Titre}\n";
        }
    }

    // Pour particuliers
    public class AttestationFiscaleParticulier : AttestationFiscale
    {
        public AttestationFiscaleParticulier()
        {
            Titre = "ATTESTATION FISCALE PARTICULIER";
        }
        
        public override string Generate()
        {
            return AjouterEnTete() +
                   "Attestation fiscale pour déclaration IR\n" +
                   "Montant des intérêts perçus: 150,00 €\n" +
                   "Année: " + DateTime.Now.Year + "\n";
        }
    }

    // Pour professionnels
    public class AttestationFiscaleProfessionnel : AttestationFiscale
    {
        public AttestationFiscaleProfessionnel()
        {
            Titre = "ATTESTATION FISCALE PROFESSIONNELLE";
        }
        
        public override string Generate()
        {
            return AjouterEnTete() +
                   "Attestation fiscale pour entreprise\n" +
                   "Bénéfices: 50 000,00 €\n" +
                   "TVA déductible: 10 000,00 €\n" +
                   "SIRET: 123 456 789 12345\n" +
                   "Année: " + DateTime.Now.Year + "\n";
        }
    }
}
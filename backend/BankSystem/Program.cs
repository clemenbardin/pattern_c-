using System;

namespace BankDocumentSystem
{
    // 1. Produits abstraits
    public abstract class ReleveIdentiteBancaire
    {
        protected string Logo { get; } = "LOGO_BANQUE";
        protected string Titre { get; set; }
        
        public abstract string Generate();
        
        protected string AjouterEnTete()
        {
            return $"=== {Logo} ===\n{Titre}\n";
        }
    }

    public abstract class AttestationCompte
    {
        protected string Logo { get; } = "LOGO_BANQUE";
        protected string Titre { get; set; }
        
        public abstract string Generate();
        
        protected string AjouterEnTete()
        {
            return $"=== {Logo} ===\n{Titre}\n";
        }
    }

    // 2. Produits concrets pour particuliers
    public class RIBParticulier : ReleveIdentiteBancaire
    {
        public RIBParticulier()
        {
            Titre = "RELEVE D'IDENTITE BANCAIRE SIMPLIFIE";
        }
        
        public override string Generate()
        {
            return AjouterEnTete() +
                   "Client: Particulier\n" +
                   "IBAN: FR76 XXXX XXXX XXXX XXXX XXXX XXX\n" +
                   "BIC: XXXXXXXXXXX\n" +
                   "Titulaire: M. Jean DUPONT\n" +
                   "Date d'émission: " + DateTime.Now.ToString("dd/MM/yyyy") + "\n" +
                   "[Signature électronique sécurisée]\n";
        }
    }

    public class AttestationParticulier : AttestationCompte
    {
        public AttestationParticulier()
        {
            Titre = "ATTESTATION DE COMPTE STANDARDISEE";
        }
        
        public override string Generate()
        {
            return AjouterEnTete() +
                   "Type de client: Particulier\n" +
                   "Le soussigné, la Banque, atteste que M. Jean DUPONT\n" +
                   "est titulaire d'un compte courant depuis le 15/01/2015.\n" +
                   "Date: " + DateTime.Now.ToString("dd/MM/yyyy") + "\n" +
                   "Cachet de la banque\n";
        }
    }

    // 3. Produits concrets pour professionnels
    public class RIBProfessionnel : ReleveIdentiteBancaire
    {
        public string Siret { get; } = "123 456 789 12345";
        
        public RIBProfessionnel()
        {
            Titre = "RELEVE D'IDENTITE BANCAIRE DETAILLE";
        }
        
        public override string Generate()
        {
            return AjouterEnTete() +
                   "Client: Professionnel\n" +
                   "SIRET: " + Siret + "\n" +
                   "IBAN: FR76 XXXX XXXX XXXX XXXX XXXX XXX\n" +
                   "BIC: XXXXXXXXXXX\n" +
                   "Entreprise: SARL TECHNOLOGIE\n" +
                   "Adresse professionnelle: 123 Rue des Entrepreneurs\n" +
                   "Date d'émission: " + DateTime.Now.ToString("dd/MM/yyyy") + "\n" +
                   "[Signature électronique sécurisée Niveau 2]\n";
        }
    }

    public class AttestationProfessionnel : AttestationCompte
    {
        public string MentionsLegales { get; } = 
            "Conformément à l'article L. 123-456 du Code de commerce";
        
        public AttestationProfessionnel()
        {
            Titre = "ATTESTATION DE COMPTE AVEC MENTIONS LEGALES";
        }
        
        public override string Generate()
        {
            return AjouterEnTete() +
                   "Type de client: Professionnel\n" +
                   "Entreprise: SARL TECHNOLOGIE\n" +
                   "SIRET: 123 456 789 12345\n" +
                   "La Banque atteste que l'entreprise ci-dessus\n" +
                   "dispose d'un compte professionnel depuis le 20/03/2010.\n" +
                   "Mentions légales: " + MentionsLegales + "\n" +
                   "Date: " + DateTime.Now.ToString("dd/MM/yyyy") + "\n" +
                   "Cachet et signature autorisée\n";
        }
    }

    // 4. Fabrique abstraite
    public interface IBankDocumentFactory
    {
        ReleveIdentiteBancaire CreateRIB();
        AttestationCompte CreateAttestation();
        AttestationFiscale CreateAttestationFiscale();
    }

    // 5. Fabriques concrètes
    public class ParticulierFactory : IBankDocumentFactory
    {
        public ReleveIdentiteBancaire CreateRIB() => new RIBParticulier();
        public AttestationCompte CreateAttestation() => new AttestationParticulier();
        public AttestationFiscale CreateAttestationFiscale() => new AttestationFiscaleParticulier(); // NOUVEAU
    }

    public class ProfessionnelFactory : IBankDocumentFactory
    {
        public ReleveIdentiteBancaire CreateRIB() => new RIBProfessionnel();
        public AttestationCompte CreateAttestation() => new AttestationProfessionnel();
        public AttestationFiscale CreateAttestationFiscale() => new AttestationFiscaleProfessionnel(); // NOUVEAU
    }

    // 6. Exemple d'utilisation
    class Program
    {
        static void Main()
        {
            Console.WriteLine("=== SYSTEME DE PRODUCTION DE DOCUMENTS BANCAIRES ===\n");
            
            // Client particulier
            Console.WriteLine("1. DOCUMENTS POUR PARTICULIER:");
            IBankDocumentFactory factoryParticulier = new ParticulierFactory();
            
            var ribParticulier = factoryParticulier.CreateRIB();
            Console.WriteLine(ribParticulier.Generate());
            
            var attestationParticulier = factoryParticulier.CreateAttestation();
            Console.WriteLine(attestationParticulier.Generate());
            
            // Client professionnel
            Console.WriteLine("\n2. DOCUMENTS POUR PROFESSIONNEL:");
            IBankDocumentFactory factoryPro = new ProfessionnelFactory();
            
            var ribPro = factoryPro.CreateRIB();
            Console.WriteLine(ribPro.Generate());
            
            var attestationPro = factoryPro.CreateAttestation();
            Console.WriteLine(attestationPro.Generate());
            
            // Simulation avec un type de client déterminé dynamiquement
            Console.WriteLine("\n3. SIMULATION DYNAMIQUE:");
            string typeClient = "PROFESSIONNEL"; // Peut venir d'une base de données
            
            IBankDocumentFactory factory = typeClient == "PARTICULIER" 
                ? new ParticulierFactory() 
                : new ProfessionnelFactory();
            
            var documents = new[]
            {
                factory.CreateRIB(),
                factory.CreateAttestation()
            };
            
            foreach (var doc in documents)
            {
                Console.WriteLine(doc.Generate());
                Console.WriteLine("---");
            }
        }
    }
}
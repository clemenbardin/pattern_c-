using MonApp.API.Models;
using BankDocumentSystem;

namespace MonApp.API.Services;

public class DataService
{
    private static int _clientIdCounter = 1;
    private static int _commandeIdCounter = 1;
    
    private readonly List<ClientDto> _clients = new();
    private readonly List<CommandeDto> _commandes = new();

    public DataService()
    {
        // Données de test
        _clients.Add(new ClientDto { Id = 1, Nom = "Jean Dupont", Email = "jean@example.com", Type = "Comptant" });
        _clients.Add(new ClientDto { Id = 2, Nom = "SARL Tech", Email = "tech@example.com", Type = "Credit", DureeCreditParDefaut = 36, TauxInteretParDefaut = 3.5m });
        _clientIdCounter = 3;
        
        _commandes.Add(new CommandeDto { Id = 1, ClientId = 1, Montant = 25000, Vehicule = "Peugeot 208", DateCommande = DateTime.Now.AddDays(-5), Type = "Comptant" });
        _commandeIdCounter = 2;
    }

    // Clients
    public List<ClientDto> GetAllClients() => _clients;
    
    public ClientDto? GetClientById(int id) => _clients.FirstOrDefault(c => c.Id == id);
    
    public ClientDto CreateClient(CreateClientDto dto)
    {
        var client = new ClientDto
        {
            Id = _clientIdCounter++,
            Nom = dto.Nom,
            Email = dto.Email,
            Type = dto.Type,
            DureeCreditParDefaut = dto.DureeCreditParDefaut,
            TauxInteretParDefaut = dto.TauxInteretParDefaut
        };
        _clients.Add(client);
        return client;
    }
    
    public ClientDto? UpdateClient(int id, CreateClientDto dto)
    {
        var client = _clients.FirstOrDefault(c => c.Id == id);
        if (client == null) return null;
        
        client.Nom = dto.Nom;
        client.Email = dto.Email;
        client.Type = dto.Type;
        client.DureeCreditParDefaut = dto.DureeCreditParDefaut;
        client.TauxInteretParDefaut = dto.TauxInteretParDefaut;
        return client;
    }
    
    public bool DeleteClient(int id)
    {
        var client = _clients.FirstOrDefault(c => c.Id == id);
        if (client == null) return false;
        _clients.Remove(client);
        return true;
    }

    // Commandes
    public List<CommandeDto> GetAllCommandes() => _commandes;
    
    public List<CommandeDto> GetCommandesByClient(int clientId) => 
        _commandes.Where(c => c.ClientId == clientId).ToList();
    
    public CommandeDto? GetCommandeById(int id) => _commandes.FirstOrDefault(c => c.Id == id);
    
    public CommandeDto CreateCommande(CreateCommandeDto dto)
    {
        var client = GetClientById(dto.ClientId);
        if (client == null) throw new Exception("Client non trouvé");
        
        decimal? mensualite = null;
        if (client.Type == "Credit")
        {
            int duree = dto.DureeCredit ?? client.DureeCreditParDefaut ?? 36;
            decimal taux = dto.TauxInteret ?? client.TauxInteretParDefaut ?? 3.5m;
            mensualite = CalculateMensualite(dto.Montant, taux, duree);
        }
        
        var commande = new CommandeDto
        {
            Id = _commandeIdCounter++,
            ClientId = dto.ClientId,
            Montant = dto.Montant,
            Vehicule = dto.Vehicule,
            DateCommande = DateTime.Now,
            Type = client.Type,
            DureeCredit = client.Type == "Credit" ? (dto.DureeCredit ?? client.DureeCreditParDefaut) : null,
            TauxInteret = client.Type == "Credit" ? (dto.TauxInteret ?? client.TauxInteretParDefaut) : null,
            Mensualite = mensualite
        };
        _commandes.Add(commande);
        return commande;
    }
    
    public bool DeleteCommande(int id)
    {
        var commande = _commandes.FirstOrDefault(c => c.Id == id);
        if (commande == null) return false;
        _commandes.Remove(commande);
        return true;
    }
    
    private decimal CalculateMensualite(decimal montant, decimal tauxInteret, int duree)
    {
        if (duree <= 0 || tauxInteret < 0 || montant <= 0)
            return 0;
        
        decimal tauxMensuel = tauxInteret / 100 / 12;
        decimal puissance = (decimal)Math.Pow((double)(1 + tauxMensuel), duree);
        
        if (puissance == 1)
            return montant / duree;
        
        return montant * (tauxMensuel * puissance) / (puissance - 1);
    }
    
    // Documents
    public DocumentDto GenerateDocument(string documentType, string clientType)
    {
        IBankDocumentFactory factory = clientType == "Particulier" 
            ? new ParticulierFactory() 
            : new ProfessionnelFactory();
        
        string content = documentType switch
        {
            "RIB" => factory.CreateRIB().Generate(),
            "Attestation" => factory.CreateAttestation().Generate(),
            "AttestationFiscale" => factory.CreateAttestationFiscale().Generate(),
            _ => throw new Exception("Type de document invalide")
        };
        
        return new DocumentDto
        {
            Type = documentType,
            ClientType = clientType,
            Content = content
        };
    }
}

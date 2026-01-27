namespace MonApp.API.Models;

public class DocumentDto
{
    public string Type { get; set; } = string.Empty; // "RIB", "Attestation", "AttestationFiscale"
    public string ClientType { get; set; } = string.Empty; // "Particulier" ou "Professionnel"
    public string Content { get; set; } = string.Empty;
}

public class GenerateDocumentRequest
{
    public string DocumentType { get; set; } = string.Empty;
    public string ClientType { get; set; } = string.Empty;
}

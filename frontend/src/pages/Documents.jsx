import { useState } from 'react';
import { documentService } from '../services/documentService';
import { toast } from 'react-toastify';

export const Documents = () => {
  const [documentType, setDocumentType] = useState('RIB');
  const [clientType, setClientType] = useState('Particulier');
  const [document, setDocument] = useState(null);
  const [loading, setLoading] = useState(false);

  const handleGenerate = async () => {
    setLoading(true);
    try {
      const response = await documentService.generate(documentType, clientType);
      setDocument(response.data);
      toast.success('Document généré avec succès');
    } catch (error) {
      toast.error('Erreur lors de la génération du document');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div>
      <h1 className="text-3xl font-bold text-gray-900 mb-6">Génération de Documents</h1>

      <div className="bg-white shadow rounded-lg p-6 mb-6">
        <div className="grid grid-cols-1 gap-6 sm:grid-cols-2">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Type de document
            </label>
            <select
              className="block w-full border border-gray-300 rounded-md shadow-sm py-2 px-3 focus:outline-none focus:ring-blue-500 focus:border-blue-500"
              value={documentType}
              onChange={(e) => setDocumentType(e.target.value)}
            >
              <option value="RIB">RIB (Relevé d'Identité Bancaire)</option>
              <option value="Attestation">Attestation de Compte</option>
              <option value="AttestationFiscale">Attestation Fiscale</option>
            </select>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Type de client
            </label>
            <select
              className="block w-full border border-gray-300 rounded-md shadow-sm py-2 px-3 focus:outline-none focus:ring-blue-500 focus:border-blue-500"
              value={clientType}
              onChange={(e) => setClientType(e.target.value)}
            >
              <option value="Particulier">Particulier</option>
              <option value="Professionnel">Professionnel</option>
            </select>
          </div>
        </div>

        <button
          onClick={handleGenerate}
          disabled={loading}
          className="mt-6 bg-blue-600 hover:bg-blue-700 text-white px-6 py-2 rounded-md disabled:opacity-50"
        >
          {loading ? 'Génération...' : 'Générer le document'}
        </button>
      </div>

      {document && (
        <div className="bg-white shadow rounded-lg p-6">
          <div className="flex justify-between items-center mb-4">
            <h2 className="text-xl font-semibold text-gray-900">Document généré</h2>
            <button
              onClick={() => {
                const blob = new Blob([document.content], { type: 'text/plain' });
                const url = URL.createObjectURL(blob);
                const a = document.createElement('a');
                a.href = url;
                a.download = `${document.type}_${document.clientType}.txt`;
                a.click();
                URL.revokeObjectURL(url);
                toast.success('Document téléchargé');
              }}
              className="bg-green-600 hover:bg-green-700 text-white px-4 py-2 rounded-md text-sm"
            >
              Télécharger
            </button>
          </div>
          <div className="bg-gray-50 rounded-md p-4">
            <pre className="whitespace-pre-wrap text-sm font-mono text-gray-800">
              {document.content}
            </pre>
          </div>
        </div>
      )}
    </div>
  );
};

import { useEffect, useState } from 'react';
import { commandeService } from '../services/commandeService';
import { clientService } from '../services/clientService';
import { toast } from 'react-toastify';

export const Commandes = () => {
  const [commandes, setCommandes] = useState([]);
  const [clients, setClients] = useState([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [formData, setFormData] = useState({
    montant: '',
    vehicule: '',
    clientId: '',
    dureeCredit: 36,
    tauxInteret: 3.5,
  });

  useEffect(() => {
    fetchData();
  }, []);

  const fetchData = async () => {
    try {
      const [commandesRes, clientsRes] = await Promise.all([
        commandeService.getAll(),
        clientService.getAll(),
      ]);
      setCommandes(commandesRes.data);
      setClients(clientsRes.data);
    } catch (error) {
      toast.error('Erreur lors du chargement');
    } finally {
      setLoading(false);
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      const selectedClient = clients.find((c) => c.id === parseInt(formData.clientId));
      const data = {
        montant: parseFloat(formData.montant),
        vehicule: formData.vehicule,
        clientId: parseInt(formData.clientId),
        dureeCredit: selectedClient?.type === 'Credit' ? parseInt(formData.dureeCredit) : null,
        tauxInteret: selectedClient?.type === 'Credit' ? parseFloat(formData.tauxInteret) : null,
      };

      await commandeService.create(data);
      toast.success('Commande créée avec succès');
      setShowModal(false);
      setFormData({
        montant: '',
        vehicule: '',
        clientId: '',
        dureeCredit: 36,
        tauxInteret: 3.5,
      });
      fetchData();
    } catch (error) {
      toast.error('Erreur lors de la création de la commande');
    }
  };

  const handleDelete = async (id) => {
    if (!window.confirm('Êtes-vous sûr de vouloir supprimer cette commande ?')) return;

    try {
      await commandeService.delete(id);
      toast.success('Commande supprimée avec succès');
      fetchData();
    } catch (error) {
      toast.error('Erreur lors de la suppression');
    }
  };

  if (loading) {
    return <div className="text-center py-12">Chargement...</div>;
  }

  return (
    <div>
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-3xl font-bold text-gray-900">Commandes</h1>
        <button
          onClick={() => setShowModal(true)}
          className="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-md"
        >
          + Nouvelle Commande
        </button>
      </div>

      <div className="bg-white shadow overflow-hidden sm:rounded-md">
        <ul className="divide-y divide-gray-200">
          {commandes.map((commande) => {
            const client = clients.find((c) => c.id === commande.clientId);
            return (
              <li key={commande.id}>
                <div className="px-4 py-4 sm:px-6">
                  <div className="flex items-center justify-between">
                    <div className="flex-1">
                      <div className="flex items-center">
                        <p className="text-sm font-medium text-gray-900">
                          {commande.vehicule}
                        </p>
                        <span className="ml-2 inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-green-100 text-green-800">
                          {commande.type}
                        </span>
                      </div>
                      <p className="mt-1 text-sm text-gray-500">
                        Client: {client?.nom || 'N/A'} | Montant: {commande.montant.toLocaleString('fr-FR')} €
                      </p>
                      <p className="mt-1 text-xs text-gray-400">
                        Date: {new Date(commande.dateCommande).toLocaleDateString('fr-FR')}
                      </p>
                      {commande.type === 'Credit' && commande.mensualite && (
                        <p className="mt-1 text-xs text-blue-600">
                          Mensualité: {commande.mensualite.toFixed(2)} € sur {commande.dureeCredit} mois
                        </p>
                      )}
                    </div>
                    <button
                      onClick={() => handleDelete(commande.id)}
                      className="text-red-600 hover:text-red-900"
                    >
                      Supprimer
                    </button>
                  </div>
                </div>
              </li>
            );
          })}
        </ul>
      </div>

      {showModal && (
        <div className="fixed z-10 inset-0 overflow-y-auto">
          <div className="flex items-end justify-center min-h-screen pt-4 px-4 pb-20 text-center sm:block sm:p-0">
            <div className="fixed inset-0 bg-gray-500 bg-opacity-75 transition-opacity"></div>
            <div className="inline-block align-bottom bg-white rounded-lg text-left overflow-hidden shadow-xl transform transition-all sm:my-8 sm:align-middle sm:max-w-lg sm:w-full">
              <form onSubmit={handleSubmit}>
                <div className="bg-white px-4 pt-5 pb-4 sm:p-6 sm:pb-4">
                  <h3 className="text-lg leading-6 font-medium text-gray-900 mb-4">
                    Nouvelle commande
                  </h3>
                  <div className="space-y-4">
                    <div>
                      <label className="block text-sm font-medium text-gray-700">Client</label>
                      <select
                        required
                        className="mt-1 block w-full border border-gray-300 rounded-md shadow-sm py-2 px-3 focus:outline-none focus:ring-blue-500 focus:border-blue-500"
                        value={formData.clientId}
                        onChange={(e) => setFormData({ ...formData, clientId: e.target.value })}
                      >
                        <option value="">Sélectionner un client</option>
                        {clients.map((client) => (
                          <option key={client.id} value={client.id}>
                            {client.nom} ({client.type})
                          </option>
                        ))}
                      </select>
                    </div>
                    <div>
                      <label className="block text-sm font-medium text-gray-700">Véhicule</label>
                      <input
                        type="text"
                        required
                        className="mt-1 block w-full border border-gray-300 rounded-md shadow-sm py-2 px-3 focus:outline-none focus:ring-blue-500 focus:border-blue-500"
                        value={formData.vehicule}
                        onChange={(e) => setFormData({ ...formData, vehicule: e.target.value })}
                      />
                    </div>
                    <div>
                      <label className="block text-sm font-medium text-gray-700">Montant (€)</label>
                      <input
                        type="number"
                        step="0.01"
                        required
                        className="mt-1 block w-full border border-gray-300 rounded-md shadow-sm py-2 px-3 focus:outline-none focus:ring-blue-500 focus:border-blue-500"
                        value={formData.montant}
                        onChange={(e) => setFormData({ ...formData, montant: e.target.value })}
                      />
                    </div>
                    {formData.clientId && (() => {
                      const selectedClient = clients.find((c) => c.id === parseInt(formData.clientId));
                      if (selectedClient?.type === 'Credit') {
                        return (
                          <>
                            <div>
                              <label className="block text-sm font-medium text-gray-700">
                                Durée crédit (mois)
                              </label>
                              <input
                                type="number"
                                className="mt-1 block w-full border border-gray-300 rounded-md shadow-sm py-2 px-3 focus:outline-none focus:ring-blue-500 focus:border-blue-500"
                                value={formData.dureeCredit}
                                onChange={(e) =>
                                  setFormData({ ...formData, dureeCredit: parseInt(e.target.value) })
                                }
                              />
                            </div>
                            <div>
                              <label className="block text-sm font-medium text-gray-700">
                                Taux d'intérêt (%)
                              </label>
                              <input
                                type="number"
                                step="0.1"
                                className="mt-1 block w-full border border-gray-300 rounded-md shadow-sm py-2 px-3 focus:outline-none focus:ring-blue-500 focus:border-blue-500"
                                value={formData.tauxInteret}
                                onChange={(e) =>
                                  setFormData({ ...formData, tauxInteret: parseFloat(e.target.value) })
                                }
                              />
                            </div>
                          </>
                        );
                      }
                      return null;
                    })()}
                  </div>
                </div>
                <div className="bg-gray-50 px-4 py-3 sm:px-6 sm:flex sm:flex-row-reverse">
                  <button
                    type="submit"
                    className="w-full inline-flex justify-center rounded-md border border-transparent shadow-sm px-4 py-2 bg-blue-600 text-base font-medium text-white hover:bg-blue-700 focus:outline-none sm:ml-3 sm:w-auto sm:text-sm"
                  >
                    Créer
                  </button>
                  <button
                    type="button"
                    onClick={() => setShowModal(false)}
                    className="mt-3 w-full inline-flex justify-center rounded-md border border-gray-300 shadow-sm px-4 py-2 bg-white text-base font-medium text-gray-700 hover:bg-gray-50 focus:outline-none sm:mt-0 sm:ml-3 sm:w-auto sm:text-sm"
                  >
                    Annuler
                  </button>
                </div>
              </form>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

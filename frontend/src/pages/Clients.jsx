import { useEffect, useState } from 'react';
import { clientService } from '../services/clientService';
import { toast } from 'react-toastify';

export const Clients = () => {
  const [clients, setClients] = useState([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [editingClient, setEditingClient] = useState(null);
  const [formData, setFormData] = useState({
    nom: '',
    email: '',
    type: 'Comptant',
    dureeCreditParDefaut: 36,
    tauxInteretParDefaut: 3.5,
  });

  useEffect(() => {
    fetchClients();
  }, []);

  const fetchClients = async () => {
    try {
      const response = await clientService.getAll();
      setClients(response.data);
    } catch (error) {
      toast.error('Erreur lors du chargement des clients');
    } finally {
      setLoading(false);
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      const data = {
        nom: formData.nom,
        email: formData.email,
        type: formData.type,
        dureeCreditParDefaut: formData.type === 'Credit' ? formData.dureeCreditParDefaut : null,
        tauxInteretParDefaut: formData.type === 'Credit' ? formData.tauxInteretParDefaut : null,
      };

      if (editingClient) {
        await clientService.update(editingClient.id, data);
        toast.success('Client modifié avec succès');
      } else {
        await clientService.create(data);
        toast.success('Client créé avec succès');
      }
      setShowModal(false);
      setEditingClient(null);
      setFormData({
        nom: '',
        email: '',
        type: 'Comptant',
        dureeCreditParDefaut: 36,
        tauxInteretParDefaut: 3.5,
      });
      fetchClients();
    } catch (error) {
      toast.error('Erreur lors de l\'enregistrement');
    }
  };

  const handleEdit = (client) => {
    setEditingClient(client);
    setFormData({
      nom: client.nom,
      email: client.email,
      type: client.type,
      dureeCreditParDefaut: client.dureeCreditParDefaut || 36,
      tauxInteretParDefaut: client.tauxInteretParDefaut || 3.5,
    });
    setShowModal(true);
  };

  const handleDelete = async (id) => {
    if (!window.confirm('Êtes-vous sûr de vouloir supprimer ce client ?')) return;

    try {
      await clientService.delete(id);
      toast.success('Client supprimé avec succès');
      fetchClients();
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
        <h1 className="text-3xl font-bold text-gray-900">Clients</h1>
        <button
          onClick={() => {
            setEditingClient(null);
            setFormData({
              nom: '',
              email: '',
              type: 'Comptant',
              dureeCreditParDefaut: 36,
              tauxInteretParDefaut: 3.5,
            });
            setShowModal(true);
          }}
          className="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-md"
        >
          + Nouveau Client
        </button>
      </div>

      <div className="bg-white shadow overflow-hidden sm:rounded-md">
        <ul className="divide-y divide-gray-200">
          {clients.map((client) => (
            <li key={client.id}>
              <div className="px-4 py-4 sm:px-6 flex justify-between items-center">
                <div>
                  <div className="flex items-center">
                    <p className="text-sm font-medium text-gray-900 truncate">
                      {client.nom}
                    </p>
                    <span className="ml-2 inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-blue-100 text-blue-800">
                      {client.type}
                    </span>
                  </div>
                  <p className="mt-1 text-sm text-gray-500">{client.email}</p>
                  {client.type === 'Credit' && (
                    <p className="mt-1 text-xs text-gray-400">
                      Durée: {client.dureeCreditParDefaut} mois | Taux: {client.tauxInteretParDefaut}%
                    </p>
                  )}
                </div>
                <div className="flex space-x-2">
                  <button
                    onClick={() => handleEdit(client)}
                    className="text-blue-600 hover:text-blue-900"
                  >
                    Modifier
                  </button>
                  <button
                    onClick={() => handleDelete(client.id)}
                    className="text-red-600 hover:text-red-900"
                  >
                    Supprimer
                  </button>
                </div>
              </div>
            </li>
          ))}
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
                    {editingClient ? 'Modifier le client' : 'Nouveau client'}
                  </h3>
                  <div className="space-y-4">
                    <div>
                      <label className="block text-sm font-medium text-gray-700">Nom</label>
                      <input
                        type="text"
                        required
                        className="mt-1 block w-full border border-gray-300 rounded-md shadow-sm py-2 px-3 focus:outline-none focus:ring-blue-500 focus:border-blue-500"
                        value={formData.nom}
                        onChange={(e) => setFormData({ ...formData, nom: e.target.value })}
                      />
                    </div>
                    <div>
                      <label className="block text-sm font-medium text-gray-700">Email</label>
                      <input
                        type="email"
                        required
                        className="mt-1 block w-full border border-gray-300 rounded-md shadow-sm py-2 px-3 focus:outline-none focus:ring-blue-500 focus:border-blue-500"
                        value={formData.email}
                        onChange={(e) => setFormData({ ...formData, email: e.target.value })}
                      />
                    </div>
                    <div>
                      <label className="block text-sm font-medium text-gray-700">Type</label>
                      <select
                        className="mt-1 block w-full border border-gray-300 rounded-md shadow-sm py-2 px-3 focus:outline-none focus:ring-blue-500 focus:border-blue-500"
                        value={formData.type}
                        onChange={(e) => setFormData({ ...formData, type: e.target.value })}
                      >
                        <option value="Comptant">Comptant</option>
                        <option value="Credit">Crédit</option>
                      </select>
                    </div>
                    {formData.type === 'Credit' && (
                      <>
                        <div>
                          <label className="block text-sm font-medium text-gray-700">
                            Durée crédit (mois)
                          </label>
                          <input
                            type="number"
                            className="mt-1 block w-full border border-gray-300 rounded-md shadow-sm py-2 px-3 focus:outline-none focus:ring-blue-500 focus:border-blue-500"
                            value={formData.dureeCreditParDefaut}
                            onChange={(e) =>
                              setFormData({ ...formData, dureeCreditParDefaut: parseInt(e.target.value) })
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
                            value={formData.tauxInteretParDefaut}
                            onChange={(e) =>
                              setFormData({ ...formData, tauxInteretParDefaut: parseFloat(e.target.value) })
                            }
                          />
                        </div>
                      </>
                    )}
                  </div>
                </div>
                <div className="bg-gray-50 px-4 py-3 sm:px-6 sm:flex sm:flex-row-reverse">
                  <button
                    type="submit"
                    className="w-full inline-flex justify-center rounded-md border border-transparent shadow-sm px-4 py-2 bg-blue-600 text-base font-medium text-white hover:bg-blue-700 focus:outline-none sm:ml-3 sm:w-auto sm:text-sm"
                  >
                    {editingClient ? 'Modifier' : 'Créer'}
                  </button>
                  <button
                    type="button"
                    onClick={() => {
                      setShowModal(false);
                      setEditingClient(null);
                    }}
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

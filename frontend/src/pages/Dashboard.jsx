import { useEffect, useState } from 'react';
import { clientService } from '../services/clientService';
import { commandeService } from '../services/commandeService';
import { toast } from 'react-toastify';

export const Dashboard = () => {
  const [stats, setStats] = useState({
    clients: 0,
    commandes: 0,
    totalMontant: 0,
  });
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchStats = async () => {
      try {
        const [clientsRes, commandesRes] = await Promise.all([
          clientService.getAll(),
          commandeService.getAll(),
        ]);

        const totalMontant = commandesRes.data.reduce(
          (sum, cmd) => sum + cmd.montant,
          0
        );

        setStats({
          clients: clientsRes.data.length,
          commandes: commandesRes.data.length,
          totalMontant,
        });
      } catch (error) {
        toast.error('Erreur lors du chargement des statistiques');
      } finally {
        setLoading(false);
      }
    };

    fetchStats();
  }, []);

  if (loading) {
    return <div className="text-center py-12">Chargement...</div>;
  }

  return (
    <div>
      <h1 className="text-3xl font-bold text-gray-900 mb-6">Dashboard</h1>
      <div className="grid grid-cols-1 gap-5 sm:grid-cols-3">
        <div className="bg-white overflow-hidden shadow rounded-lg">
          <div className="p-5">
            <div className="flex items-center">
              <div className="flex-shrink-0">
                <div className="w-8 h-8 bg-blue-500 rounded-full flex items-center justify-center">
                  <span className="text-white font-bold">👥</span>
                </div>
              </div>
              <div className="ml-5 w-0 flex-1">
                <dl>
                  <dt className="text-sm font-medium text-gray-500 truncate">
                    Clients
                  </dt>
                  <dd className="text-lg font-medium text-gray-900">
                    {stats.clients}
                  </dd>
                </dl>
              </div>
            </div>
          </div>
        </div>

        <div className="bg-white overflow-hidden shadow rounded-lg">
          <div className="p-5">
            <div className="flex items-center">
              <div className="flex-shrink-0">
                <div className="w-8 h-8 bg-green-500 rounded-full flex items-center justify-center">
                  <span className="text-white font-bold">📦</span>
                </div>
              </div>
              <div className="ml-5 w-0 flex-1">
                <dl>
                  <dt className="text-sm font-medium text-gray-500 truncate">
                    Commandes
                  </dt>
                  <dd className="text-lg font-medium text-gray-900">
                    {stats.commandes}
                  </dd>
                </dl>
              </div>
            </div>
          </div>
        </div>

        <div className="bg-white overflow-hidden shadow rounded-lg">
          <div className="p-5">
            <div className="flex items-center">
              <div className="flex-shrink-0">
                <div className="w-8 h-8 bg-purple-500 rounded-full flex items-center justify-center">
                  <span className="text-white font-bold">€</span>
                </div>
              </div>
              <div className="ml-5 w-0 flex-1">
                <dl>
                  <dt className="text-sm font-medium text-gray-500 truncate">
                    Total Montant
                  </dt>
                  <dd className="text-lg font-medium text-gray-900">
                    {stats.totalMontant.toLocaleString('fr-FR')} €
                  </dd>
                </dl>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

import api from '../config/api';

export const commandeService = {
  getAll: () => api.get('/api/commandes'),
  getById: (id) => api.get(`/api/commandes/${id}`),
  getByClient: (clientId) => api.get(`/api/commandes/client/${clientId}`),
  create: (data) => api.post('/api/commandes', data),
  delete: (id) => api.delete(`/api/commandes/${id}`),
};

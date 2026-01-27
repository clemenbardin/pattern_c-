import api from '../config/api';
import Cookies from 'js-cookie';

export const authService = {
  async login(username, password) {
    const response = await api.post('/api/auth/login', { username, password });
    if (response.data.token) {
      Cookies.set('auth_token', response.data.token, { expires: 1 });
    }
    return response.data;
  },

  async logout() {
    await api.post('/api/auth/logout');
    Cookies.remove('auth_token');
  },

  async getCurrentUser() {
    try {
      const response = await api.get('/api/auth/me');
      return response.data;
    } catch (error) {
      return null;
    }
  },

  isAuthenticated() {
    return !!Cookies.get('auth_token');
  },
};
